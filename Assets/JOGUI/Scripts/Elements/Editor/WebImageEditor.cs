using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace JOGUI.Editor
{
    [CustomEditor(typeof(WebImage), true)]
    [CanEditMultipleObjects]
    public class WebImageEditor : ImageEditor
    {
        private SerializedProperty _urlProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _urlProperty = serializedObject.FindProperty("_url");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(_urlProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}