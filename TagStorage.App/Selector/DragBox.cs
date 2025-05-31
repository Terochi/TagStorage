﻿using System;
using JetBrains.Annotations;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Layout;
using osuTK;

namespace TagStorage.App.Selector;

public partial class DragBox : CompositeDrawable, IStateful<Visibility>
{
    public Drawable Box { get; private set; }

    /// <summary>
    /// Creates a new <see cref="DragBox"/>.
    /// </summary>
    public DragBox()
    {
        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
        Alpha = 0;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = Box = CreateBox();
    }

    protected virtual Drawable CreateBox() => new BoxWithBorders();

    /// <summary>
    /// Handle a forwarded mouse event.
    /// </summary>
    /// <param name="e">The mouse event.</param>
    public virtual void HandleDrag(MouseButtonEvent e)
    {
        Box.Position = Vector2.ComponentMin(e.MouseDownPosition, e.MousePosition);
        Box.Size = Vector2.ComponentMax(e.MouseDownPosition, e.MousePosition) - Box.Position;
    }

    private Visibility state;

    public Visibility State
    {
        get => state;
        set
        {
            if (value == state) return;

            state = value;
            this.FadeTo(state == Visibility.Hidden ? 0 : 1, 250, Easing.OutQuint);
            StateChanged?.Invoke(state);
        }
    }

    public override void Hide() => State = Visibility.Hidden;

    public override void Show() => State = Visibility.Visible;

    [CanBeNull]
    public event Action<Visibility> StateChanged;

    public partial class BoxWithBorders : CompositeDrawable
    {
        private readonly LayoutValue cache = new LayoutValue(Invalidation.RequiredParentSizeToFit);

        public BoxWithBorders()
        {
            AddLayout(cache);
        }

        protected override void Update()
        {
            base.Update();

            if (!cache.IsValid)
            {
                createContent();
                cache.Validate();
            }
        }

        private void createContent()
        {
            if (DrawSize == Vector2.Zero)
            {
                ClearInternal();
                return;
            }

            // Make lines the same width independent of display resolution.
            float lineThickness = DrawWidth > 0
                ? DrawWidth / ScreenSpaceDrawQuad.Width * 2
                : DrawHeight / ScreenSpaceDrawQuad.Height * 2;

            Padding = new MarginPadding(-lineThickness / 2);

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = lineThickness,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = lineThickness,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = lineThickness,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = lineThickness,
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.1f
                }
            };
        }
    }
}
