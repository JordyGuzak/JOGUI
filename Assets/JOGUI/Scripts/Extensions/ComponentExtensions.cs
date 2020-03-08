using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JOGUI.Extensions
{
    public static class ComponentExtensions
    {
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            return component.TryGetComponent<T>(out var c) ? c : component.gameObject.AddComponent<T>();
        }
    }
}

