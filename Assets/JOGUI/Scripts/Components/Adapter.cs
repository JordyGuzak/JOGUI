using System.Collections.Generic;
using JOGUI.Utils;
using UnityEngine;

namespace JOGUI.Components
{
    public class Adapter<T1, T2> where T1 : Component //TODO add DataSource interface or base class?
    {
        public int Count => _data.Count;
        
        public System.Action<T1, T2> BindItem { get; set; }

        private List<T2> _data;
        private Dictionary<int, T1> _activeElements;
        private ObjectPool<T1> _objectPool;

        public Adapter(T1 prefab, Transform parent)
        {
            _data = new List<T2>();
            _activeElements = new Dictionary<int, T1>();
            _objectPool = new ObjectPool<T1>(prefab, parent);
        }

        public void Add(T2 data)
        {
            var element = _objectPool.Rent();
            _data.Add(data);
            _activeElements.Add(_data.Count - 1, element);
            BindItem?.Invoke(element, data);
        }

        public void Remove(T2 data)
        {
            var index = _data.IndexOf(data);
            if (index == -1) return;
            _data.RemoveAt(index);
            var element = _activeElements[index];
            _activeElements.Remove(index);
            _objectPool.Return(element);
        }

        public void SetData(IEnumerable<T2> data)
        {
            if (_data.Count > 0)
                Clear();

            foreach (var d in data)
            {
                Add(d);
            }
        }

        public void Clear()
        {
            _data.Clear();
            foreach (var pair in _activeElements)
            {
                _objectPool.Return(pair.Value);
            }
            _activeElements.Clear();
        }
    }
}