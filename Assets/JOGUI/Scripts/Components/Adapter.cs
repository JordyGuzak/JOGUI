using System.Collections;
using System.Collections.Generic;
using JOGUI.Utils;
using UnityEngine;

namespace JOGUI
{
    public class Adapter<T1, T2> where T1 : Component //TODO add DataSource interface or base class?
    {
        public int Count => _data.Count;
        
        public System.Action<T1, T2> BindItem { get; set; }
        public System.Action<T1> UnbindItem { get; set; }

        private List<T2> _data;
        public Dictionary<int, T1> ActiveItems { get; private set; }
        private ObjectPool<T1> _objectPool;

        public Adapter(T1 prefab, Transform parent)
        {
            _data = new List<T2>();
            ActiveItems = new Dictionary<int, T1>();
            _objectPool = new ObjectPool<T1>(prefab, parent);
        }

        public void Add(T2 data)
        {
            var element = _objectPool.Rent();
            _data.Add(data);
            var index = _data.Count - 1;
            ActiveItems.Add(index, element);
            element.transform.SetSiblingIndex(index);
            BindItem?.Invoke(element, data);
        }

        public void Remove(T2 data)
        {
            var index = _data.IndexOf(data);
            if (index == -1) return;
            _data.RemoveAt(index);
            var element = ActiveItems[index];
            UnbindItem?.Invoke(element);
            ActiveItems.Remove(index);
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
            foreach (var pair in ActiveItems)
            {
                UnbindItem?.Invoke(pair.Value);
                _objectPool.Return(pair.Value);
            }
            ActiveItems.Clear();
        }
    }
}