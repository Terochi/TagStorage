namespace TagStorage.App.Selector;

public interface ISelectable<T>
{
    T GetUnderlyingData();

    bool IsSelected { get; protected set; }

    void OnSelected() => IsSelected = true;
    void OnDeselected() => IsSelected = false;
}
