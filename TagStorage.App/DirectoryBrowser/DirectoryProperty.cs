using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using MouseButton = osuTK.Input.MouseButton;

namespace TagStorage.App.DirectoryBrowser;

public partial class DirectoryProperty : CompositeDrawable
{
    public event Action<DirectoryProperty, DragStartEvent> DragStart;
    public event Action<DirectoryProperty, DragEvent> Drag;
    public event Action<DirectoryProperty, DragEndEvent> DragEnd;

    public BindableNumber<float> DragWidth { get; } = new BindableFloat
    {
        MinValue = 80,
        MaxValue = float.MaxValue
    };

    public readonly DirectoryPropertyType Type;

    public LocalisableString Text
    {
        get => nameText.Text;
        set => nameText.Text = value;
    }

    private readonly SpriteText nameText;

    public DirectoryProperty(DirectoryPropertyType type)
    {
        Type = type;

        AutoSizeAxes = Axes.X;
        AddInternal(nameText = new SpriteText
        {
            Truncate = true,
            RelativeSizeAxes = Axes.Y,
        });
        AddInternal(new WidthDragBox
        {
            DragWidth = DragWidth.GetBoundCopy(),
            Colour = Colour4.DarkGray,
        });

        DragWidth.BindValueChanged(change =>
        {
            nameText.Width = change.NewValue;
        }, true);
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        DragStart?.Invoke(this, e);
        return true;
    }

    protected override void OnDrag(DragEvent e) => Drag?.Invoke(this, e);

    protected override void OnDragEnd(DragEndEvent e) => DragEnd?.Invoke(this, e);

    private partial class WidthDragBox : CompositeDrawable
    {
        private const float hover_border = 5f;
        private const float hover_visible = 2f;

        private Box box;
        public BindableNumber<float> DragWidth { get; set; } = new BindableFloat();

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = hover_border * 2f + hover_visible;
            RelativeSizeAxes = Axes.Y;
            Padding = new MarginPadding { Left = hover_border, Right = hover_border };

            AddInternal(box = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.DarkGray,
            });

            DragWidth.BindValueChanged(change =>
            {
                X = change.NewValue - hover_border - hover_visible * 0.5f;
            }, true);
        }

        private bool isDragged;

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button == MouseButton.Left)
            {
                isDragged = true;
                return true;
            }

            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            isDragged = false;

            if (IsHovered)
            {
                base.OnMouseUp(e);
                return;
            }

            box.Colour = Colour4.DarkGray;
        }

        protected override bool OnHover(HoverEvent e)
        {
            box.Colour = Colour4.White;
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            if (isDragged)
            {
                base.OnHoverLost(e);
                return;
            }

            box.Colour = Colour4.DarkGray;
        }

        protected override bool OnDragStart(DragStartEvent e)
        {
            return true;
        }

        protected override void OnDrag(DragEvent e)
        {
            DragWidth.Value = e.MousePosition.X;
        }
    }
}
