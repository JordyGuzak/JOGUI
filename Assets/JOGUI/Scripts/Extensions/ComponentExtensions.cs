using UnityEngine;

namespace JOGUI.Extensions
{
    public static class ComponentExtensions
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.TryGetComponent<T>(out var c) ? c : component.gameObject.AddComponent<T>();
        }

        public static bool TryGetComponentInChildren<T>(this Component parent, out T component, bool includeInactive = false) where T : Component
        {
            component = parent.GetComponentInChildren<T>(includeInactive);
            return component;
        }
        
        public static bool TryGetComponentInParent<T>(this Component parent, out T component) where T : Component
        {
            component = parent.GetComponentInParent<T>();
            return component;
        }
    }
}

