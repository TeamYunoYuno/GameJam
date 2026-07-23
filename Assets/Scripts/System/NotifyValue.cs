public class NotifyValue<T>
{
    public delegate void ValueChanged(T prev, T now);

    private T _value;

    public NotifyValue()
    {
        _value = default;
    }

    public NotifyValue(T value)
    {
        _value = value;
    }

    public T Value
    {
        get => _value;
        set
        {
            var before = _value;
            _value = value;
            if ((before == null && _value != null) || !before.Equals(_value)) OnValueChanged?.Invoke(before, _value);
        }
    }

    public event ValueChanged OnValueChanged;
}