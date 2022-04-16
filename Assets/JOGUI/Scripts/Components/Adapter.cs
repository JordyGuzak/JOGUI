using System.Collections.Generic;
using System.Linq;
using JOGUI.Utils;
using UnityEngine;

namespace JOGUI
{
    public class Adapter<T1, T2> where T1 : Component //TODO add DataSource interface or base class?
    {
        public delegate void BindItemAction(T1 item, T2 data, int index);
        public delegate void UnbindItemAction(T1 item);

        public int Count => Data.Count;
        public BindItemAction BindItem { get; set; }
        public UnbindItemAction UnbindItem { get; set; }
        public Dictionary<int, T1> ActiveItems { get; private set; }

        public List<T2> Data { get; private set; }
        private ObjectPool<T1> _objectPool;

        public Adapter(T1 prefab, Transform parent)
        {
            Data = new List<T2>();
            ActiveItems = new Dictionary<int, T1>();
            _objectPool = new ObjectPool<T1>(prefab, parent);
        }

        public virtual void Add(T2 data)
        {
            var element = _objectPool.Rent();
            Data.Add(data);
            var index = Data.Count - 1;
            ActiveItems.Add(index, element);
            element.transform.SetSiblingIndex(index);
            BindItem?.Invoke(element, data, index);
        }

        public virtual void Remove(T2 data)
        {
            var index = Data.IndexOf(data);
            if (index == -1) return;
            Data.RemoveAt(index);
            var element = ActiveItems[index];
            UnbindItem?.Invoke(element);
            ActiveItems.Remove(index);
            _objectPool.Return(element);
        }

        public void SetData(IEnumerable<T2> data)
        {
            var d = data.ToList();
            
            for (int i = Data.Count - 1; i > 0 && i > d.Count - 1; i--)
            {
                Remove(Data[i]);
            }
            
            for (int i = 0; i < d.Count; i++)
            {
                if (ActiveItems.TryGetValue(i, out var item))
                {
                    Data[i] = d[i];
                    UnbindItem?.Invoke(item);
                    BindItem?.Invoke(item, d[i], i);
                }
                else
                {
                    Add(d[i]);
                }
            }
        }

        public virtual void Clear()
        {
            Data.Clear();
            foreach (var pair in ActiveItems)
            {
                UnbindItem?.Invoke(pair.Value);
                _objectPool.Return(pair.Value);
            }
            ActiveItems.Clear();
        }
    }
}