using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;
using osuTK.Input;

namespace TagStorage.App.Selector;

public partial class SelectorContainer<D, T> : Container<D>
    where D : Drawable, ISelectable<T>
{
    protected override Container<D> Content => content;

    private Container<D> content;

    public SelectorContainer()
    {
        Masking = true;
        content = new FillFlowContainer<D>
        {
            Direction = FillDirection.Vertical,
            AutoSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren =
        [
            content,
            DragBox = new DragBox(),
        ];
    }

    private MouseButtonEvent lastDragEvent;
    private bool wasDragStarted;
    protected DragBox DragBox { get; private set; }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button == MouseButton.Right)
            return false;

        lastDragEvent = e;
        wasDragStarted = true;

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

        foreach (var blueprint in content.Children)
        {
            var innerQuad = blueprint.ScreenSpaceDrawQuad;

            bool isSelected = doOverlap(innerQuad, quad);
            bool wasSelected = blueprint.IsSelected;

            if (isSelected && !wasSelected)
            {
                Logger.Log("Selected");
                blueprint.OnSelected();
            }
            else if (!isSelected && wasSelected)
            {
                Logger.Log("Deselected");
                blueprint.OnDeselected();
            }
        }
    }
}
