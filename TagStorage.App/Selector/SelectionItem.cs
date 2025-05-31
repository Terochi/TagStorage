using System;
using JetBrains.Annotations;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;

namespace TagStorage.App.Selector;

public abstract partial class SelectionItem<T> : CompositeDrawable, IStateful<SelectionState>
{
    public readonly T Item;

    public event Action<SelectionItem<T>> Selected;
    public event Action<SelectionItem<T>> Deselected;

    public override bool HandlePositionalInput => IsSelectable;
    public override bool RemoveWhenNotAlive => false;

    private Drawable selection;

    protected SelectionItem(T item)
    {
        Item = item;

        AlwaysPresent = true;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        InternalChildren =
        [
            selection = CreateSelection().With(s => s.Alpha = 0f),
            CreateContent(),
        ];
    }

    protected abstract Drawable CreateSelection();
    protected abstract Drawable CreateContent();

    protected override void LoadComplete()
    {
        base.LoadComplete();
        updateState();
    }

    private SelectionState state;

    [CanBeNull]
    public event Action<SelectionState> StateChanged;

    public SelectionState State
    {
        get => state;
        set
        {
            if (state == value)
                return;

            state = value;

            if (IsLoaded)
                updateState();

            StateChanged?.Invoke(state);
        }
    }

    private void updateState()
    {
        switch (state)
        {
            case SelectionState.Selected:
                OnSelected();
                Selected?.Invoke(this);
                break;

            case SelectionState.NotSelected:
                OnDeselected();
                Deselected?.Invoke(this);
                break;
        }
    }

    protected virtual void OnDeselected()
    {
        selection.Hide();
    }

    protected virtual void OnSelected()
    {
        selection.Show();
    }

    /// <summary>
    /// Selects this <see cref="SelectionItem{T}"/>, causing it to become visible.
    /// </summary>
    public void Select() => State = SelectionState.Selected;

    /// <summary>
    /// Deselects this <see cref="SelectionItem{T}"/>, causing it to become invisible.
    /// </summary>
    public void Deselect() => State = SelectionState.NotSelected;

    public void ToggleSelection() => State = IsSelected ? SelectionState.NotSelected : SelectionState.Selected;

    public bool IsSelected => State == SelectionState.Selected;

    /// <summary>
    /// Whether the <see cref="SelectionItem{T}"/> can be currently selected via a click or a drag box.
    /// </summary>
    public virtual bool IsSelectable => ShouldBeAlive && IsPresent;

    /// <summary>
    /// The screen-space quad that outlines this <see cref="SelectionItem{T}"/> for selections.
    /// </summary>
    public virtual Quad SelectionQuad => ScreenSpaceDrawQuad;
}
