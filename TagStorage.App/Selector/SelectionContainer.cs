using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace TagStorage.App.Selector;

public abstract partial class SelectionContainer<T> : Container
{
    protected Container<SelectionItem<T>> Items { get; }

    private readonly Dictionary<T, SelectionItem<T>> itemMap = new Dictionary<T, SelectionItem<T>>();

    public readonly BindableList<T> SelectedItems = new BindableList<T>();
    public readonly List<SelectionItem<T>> SelectedBlueprints;

    protected abstract SelectionItem<T> CreateItem(T item);

    protected SelectionContainer()
    {
        SelectedBlueprints = new List<SelectionItem<T>>();

        Masking = true;
        Items = CreateContent();
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        foreach (var item in SelectedBlueprints)
        {
            if (runForBlueprint(item))
                return true;
        }

        foreach (var item in Items.AliveChildren)
        {
            if (runForBlueprint(item))
                return true;
        }

        bool runForBlueprint(SelectionItem<T> blueprint)
        {
            if (!blueprint.IsHovered) return false;

            if (e.ControlPressed && e.Button == MouseButton.Left && !blueprint.IsSelected)
            {
                blueprint.ToggleSelection();
                clickHandled = true;
                return true;
            }

            if (blueprint.IsSelected)
                return false;

            DeselectAll();
            blueprint.Select();
            clickHandled = true;
            return true;
        }

        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        Schedule(() =>
        {
            endClickSelection(e);
            clickHandled = false;
        });

        base.OnMouseUp(e);
    }

    protected void ClearAll()
    {
        itemMap.Clear();
        Items.Clear();
        SelectedItems.Clear();
        SelectedBlueprints.Clear();
    }

    protected void DeselectAll()
    {
        // foreach (var item in selectedItems)
        // {
        // item.Deselect();
        // }

        SelectedItems.Clear();
    }

    public void AddBlueprintFor(T item)
    {
        if (itemMap.ContainsKey(item))
            return;

        var itemDrawable = CreateItem(item);
        if (itemDrawable == null)
            return;

        itemMap[item] = itemDrawable;

        itemDrawable.Selected += OnBlueprintSelected;
        itemDrawable.Deselected += OnBlueprintDeselected;

        Items.Add(itemDrawable);
    }

    public void RemoveBlueprintFor(T item)
    {
        if (!itemMap.Remove(item, out var blueprint))
            return;

        blueprint.Deselect();
        blueprint.Selected -= OnBlueprintSelected;
        blueprint.Deselected -= OnBlueprintDeselected;

        Items.Remove(blueprint, true);
    }

    protected virtual void OnBlueprintSelected(SelectionItem<T> blueprint)
    {
        if (!SelectedItems.Contains(blueprint.Item))
            SelectedItems.Add(blueprint.Item);

        SelectedBlueprints.Add(blueprint);
    }

    protected virtual void OnBlueprintDeselected(SelectionItem<T> blueprint)
    {
        SelectedItems.Remove(blueprint.Item);
        SelectedBlueprints.Remove(blueprint);
    }

    protected abstract Container<SelectionItem<T>> CreateContent();

    [BackgroundDependencyLoader]
    private void load()
    {
        SelectedItems.CollectionChanged += (_, args) =>
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object o in args.NewItems)
                    {
                        if (itemMap.TryGetValue((T)o, out var item))
                            item.Select();
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object o in args.OldItems)
                    {
                        if (itemMap.TryGetValue((T)o, out var blueprint))
                            blueprint.Deselect();
                    }

                    break;
            }
        };

        InternalChildren =
        [
            Items,
            DragBox = new DragBox(),
        ];
    }

    private MouseButtonEvent lastDragEvent;
    protected DragBox DragBox { get; private set; }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button == MouseButton.Right)
            return false;

        lastDragEvent = e;

        DragBox.HandleDrag(e);
        DragBox.Show();
        return true;
    }

    protected override void OnDrag(DragEvent e)
    {
        lastDragEvent = e;
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        lastDragEvent = null;

        DragBox.Hide();
    }

    protected override void Update()
    {
        base.Update();

        if (lastDragEvent != null && DragBox.State == Visibility.Visible)
        {
            lastDragEvent.Target = this;
            DragBox.HandleDrag(lastDragEvent);
            UpdateSelectionFromDragBox();
        }
    }

    private bool clickHandled = false;

    protected override bool OnClick(ClickEvent e)
    {
        if (e.Button == MouseButton.Right)
            return false;

        // store for double-click handling
        var clickedBlueprint = SelectedBlueprints.FirstOrDefault(b => b.IsHovered);

        // Deselection should only occur if no selected blueprints are hovered
        // A special case for when a blueprint was selected via this click is added since OnClick() may occur outside the item and should not trigger deselection
        if (endClickSelection(e) || clickedBlueprint != null)
            return true;

        DeselectAll();
        return base.OnClick(e);
    }

    private bool endClickSelection(MouseButtonEvent e)
    {
        if (clickHandled) return true;

        if (e.Button != MouseButton.Left) return false;

        if (e.ControlPressed)
        {
            // if a selection didn't occur, we may want to trigger a deselection.

            // Iterate from the top of the input stack (blueprints closest to the front of the screen first).
            // Priority is given to already-selected blueprints.
            foreach (SelectionItem<T> blueprint in Items.AliveChildren.Where(b => b.IsHovered))
            {
                if (blueprint.IsSelected)
                {
                    blueprint.ToggleSelection();
                    clickHandled = true;
                    return true;
                }

                return false;
            }

            return false;
        }

        return false;
    }

    private bool doOverlap(Quad q1, Quad q2)
    {
        Vector2 l1 = q1.BottomLeft;
        Vector2 r1 = q1.TopRight;
        Vector2 l2 = q2.BottomLeft;
        Vector2 r2 = q2.TopRight;

        if (l1.X > r2.X || l2.X > r1.X)
            return false;

        if (r1.Y > l2.Y || r2.Y > l1.Y)
            return false;

        return true;
    }

    protected virtual void UpdateSelectionFromDragBox()
    {
        var quad = DragBox.Box.ScreenSpaceDrawQuad;

        foreach (var item in Items.Children)
        {
            var itemQuad = item.ScreenSpaceDrawQuad;

            bool isSelected = doOverlap(itemQuad, quad);
            bool wasSelected = item.IsSelected;

            if (isSelected && !wasSelected)
            {
                item.Select();
            }
            else if (!isSelected && wasSelected)
            {
                item.Deselect();
            }
        }
    }
}
