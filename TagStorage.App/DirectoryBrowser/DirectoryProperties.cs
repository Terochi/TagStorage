using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace TagStorage.App.DirectoryBrowser;

public partial class DirectoryProperties : CompositeDrawable
{
    public BindableList<DirectoryPropertyValue> Properties { get; } = new BindableList<DirectoryPropertyValue>();

    private readonly BindableDictionary<DirectoryPropertyType, float> columnWidths = new BindableDictionary<DirectoryPropertyType, float>();
    private readonly List<DirectoryProperty> propertyDrawables = new List<DirectoryProperty>();

    private readonly FillFlowContainer<DirectoryProperty> propertyContainer;

    public DirectoryProperties()
    {
        AddInternal(propertyContainer = new FillFlowContainer<DirectoryProperty>
        {
            Direction = FillDirection.Horizontal,
            RelativeSizeAxes = Axes.Both,
            LayoutDuration = 160,
            LayoutEasing = Easing.OutQuint,
        });

        Properties.BindCollectionChanged((_, change) =>
        {
            switch (change.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (DirectoryPropertyValue property in change.NewItems!)
                    {
                        addProperty(property);
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    var directoryProperty = propertyDrawables[change.OldStartingIndex];
                    propertyDrawables.RemoveAt(change.OldStartingIndex);
                    propertyDrawables.Insert(change.NewStartingIndex, directoryProperty);

                    sortProperties();

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (DirectoryPropertyValue property in change.OldItems!)
                    {
                        removeProperty(property);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (DirectoryPropertyValue oldProperty in change.OldItems!)
                    {
                        bool found = false;

                        foreach (DirectoryPropertyValue newProperty in change.NewItems!)
                        {
                            if (oldProperty.Type != newProperty.Type) continue;

                            found = true;
                            break;
                        }

                        if (found) continue;

                        removeProperty(oldProperty);
                    }

                    foreach (DirectoryPropertyValue newProperty in change.NewItems!)
                    {
                        bool found = false;

                        foreach (DirectoryPropertyValue oldProperty in change.OldItems!)
                        {
                            if (oldProperty.Type != newProperty.Type) continue;

                            found = true;
                            break;
                        }

                        if (found) continue;

                        addProperty(newProperty);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    propertyContainer.Clear();
                    propertyDrawables.Clear();
                    columnWidths.Clear();

                    if (change.NewItems == null)
                        break;

                    foreach (DirectoryPropertyValue property in change.NewItems)
                    {
                        addProperty(property);
                    }

                    break;
            }
        });

        columnWidths.BindCollectionChanged((_, change) =>
        {
            if (change.NewItems == null) return;

            (DirectoryPropertyType type, float width) = change.NewItems.First();
            int index = propertyDrawables.FindIndex(p => p.Type == type);

            var newPropertyValue = new DirectoryPropertyValue(type, width);
            if (Properties[index] == newPropertyValue)
                return;

            Properties[index] = newPropertyValue;
        });
    }

    private void addProperty(DirectoryPropertyValue property)
    {
        if (columnWidths.ContainsKey(property.Type))
            throw new InvalidOperationException("Cannot create two or more properties of the same type.");

        var directoryProperty = new DirectoryProperty(property.Type)
        {
            RelativeSizeAxes = Axes.Y,
            Text = property.Type.ToString(),
            DragWidth = { Value = property.Width },
        };

        directoryProperty.DragStart += dragStart;
        directoryProperty.Drag += drag;
        directoryProperty.DragEnd += dragEnd;

        directoryProperty.DragWidth.BindValueChanged(change =>
        {
            columnWidths[property.Type] = change.NewValue;
        });

        propertyDrawables.Add(directoryProperty);
        propertyContainer.Add(directoryProperty);
        columnWidths.Add(property.Type, property.Width);

        sortProperties();
    }

    private void removeProperty(DirectoryPropertyValue property)
    {
        DirectoryProperty directoryProperty = propertyDrawables.FirstOrDefault(p => p.Type == property.Type);

        if (directoryProperty == null) return;

        propertyContainer.Remove(directoryProperty, true);
        propertyDrawables.Remove(directoryProperty);
        columnWidths.Remove(property.Type);

        sortProperties();
    }

    private void sortProperties()
    {
        for (int i = 0; i < propertyDrawables.Count; i++)
        {
            propertyContainer.SetLayoutPosition(propertyDrawables[i], i);
        }
    }

    [CanBeNull]
    private DirectoryProperty draggedProperty;

    private int draggedStartIndex;

    private void dragStart(DirectoryProperty property, DragStartEvent e)
    {
        draggedProperty = property;
        draggedStartIndex = propertyDrawables.IndexOf(draggedProperty);
    }

    private void drag(DirectoryProperty property, DragEvent e)
    {
        if (draggedProperty == null)
            return;

        var localPos = ToLocalSpace(e.ScreenSpaceMousePosition);

        int srcIndex = propertyDrawables.IndexOf(draggedProperty);

        float widthAccumulator = 0;
        int dstIndex = 0;

        for (; dstIndex < propertyDrawables.Count; dstIndex++)
        {
            // Get the index of the property before any dragging has been done
            int realIndex = dstIndex;
            if (dstIndex == draggedStartIndex) realIndex = srcIndex;
            if (dstIndex < draggedStartIndex && dstIndex >= srcIndex) realIndex++;
            if (dstIndex > draggedStartIndex && dstIndex <= srcIndex) realIndex--;

            var drawable = propertyDrawables[realIndex];

            if (!drawable.IsLoaded || !drawable.IsPresent)
                continue;

            float width = drawable.BoundingBox.Width;

            if (localPos.X < widthAccumulator + width)
            {
                break;
            }

            widthAccumulator += width;
        }

        dstIndex = Math.Clamp(dstIndex, 0, propertyDrawables.Count - 1);

        if (srcIndex == dstIndex)
            return;

        Properties.Move(srcIndex, dstIndex);
    }

    private void dragEnd(DirectoryProperty property, DragEndEvent e)
    {
        draggedProperty = null;
    }
}

public readonly record struct DirectoryPropertyValue(DirectoryPropertyType Type, float Width = 0)
{
    public float Width { get; } = Width == 0 ? getDefaultWidthForType(Type) : Width;

    private static float getDefaultWidthForType(DirectoryPropertyType type)
    {
        return type switch
        {
            DirectoryPropertyType.Name => 300,
            DirectoryPropertyType.Size => 100,
            DirectoryPropertyType.DateModified => 150,
            DirectoryPropertyType.Tags => 300,
            _ => 100,
        };
    }
}

public enum DirectoryPropertyType
{
    Name,
    Size,
    DateModified,
    Tags,
}
