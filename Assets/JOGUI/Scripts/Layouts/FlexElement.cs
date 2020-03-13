using System;
using UnityEngine;

namespace JOGUI
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class FlexElement : MonoBehaviour
    {
        [SerializeField] private float _flexGrow;
        public float FlexGrow
        {
            get => _flexGrow;
            set => SetProperty(ref _flexGrow, value);
        }

        [SerializeField] private float _flexShrink = 1f;
        public float FlexShrink
        {
            get => _flexShrink;
            set => SetProperty(ref _flexShrink, value);
        }

        [SerializeField] private Vector2 _flexBasis;
        public Vector2 FlexBasis
        {
            get => _flexBasis;
            set => SetProperty(ref _flexBasis, value);
        }

        private RectTransform _rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        private void OnEnable()
        {
            SetDirty();
        }

        private void OnDisable()
        {
            SetDirty();
        }

        protected void SetProperty<T>(ref T currentValue, T newValue)
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return;
            currentValue = newValue;

           SetDirty();
        }

        private void SetDirty()
        {
            var flexContainer = GetComponentInParent<FlexContainer>();
            
            if (flexContainer)
                flexContainer.SetDirty();
        }
        
#if UNITY_EDITOR
        protected void OnValidate()
        {
            SetDirty();
        }

        protected void Reset()
        {
            SetDirty();
        }
#endif
    }
}