using System.Collections.Generic;
using UnityEngine;

namespace JOGUI.Utils
{
    public class ObjectPool<T> where T : Component
    {
        private T _prefab;
        private Transform _parent;
        private Stack<T> _stack;

        public ObjectPool(T prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _stack = new Stack<T>();
        }

        public T Rent()
        {
            var obj = _stack.Count > 0 ? _stack.Pop() : UnityEngine.Object.Instantiate<T>(_prefab, _parent);
            BeforeRent(obj);
            return obj;
        }

        public void Return(T obj)
        {
            BeforeReturn(obj);
            _stack.Push(obj);
        }

        protected virtual void BeforeRent(T obj)
        {
            obj.gameObject.SetActive(true);
        }

        protected virtual void BeforeReturn(T obj)
        {
            obj.gameObject.SetActive(false);
        }
    }
}
