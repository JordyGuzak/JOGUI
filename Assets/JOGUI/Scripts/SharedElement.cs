using UnityEngine;

namespace JOGUI
{
    public class SharedElement : MonoBehaviour
    {
        public string Key;
        public RectTransform RectTransform => _rectTransform ??= GetComponent<RectTransform>();
        public Transform OriginalParent { get; set; }
        public int OriginalSiblingIndex { get; set; }
        
        private RectTransform _rectTransform;
    }
}
