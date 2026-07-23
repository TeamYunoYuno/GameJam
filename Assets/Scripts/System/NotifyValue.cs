using UnityEngine;

    public class NotifyValue<T>
    {
        private T _value;
        
        public delegate void ValueChanged(T prev, T now);
        public event ValueChanged OnValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                T before = _value;
                _value = value;
                if ((before == null && _value != null) || !before.Equals(_value))
                {
                    OnValueChanged?.Invoke(before, _value);
                }
            }
        }

        public NotifyValue()
        {
            _value = default(T);
        }

        public NotifyValue(T value)
        {
            _value = value;
        }
    }
