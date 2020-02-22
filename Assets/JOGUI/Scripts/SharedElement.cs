using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SharedElement : MonoBehaviour
{
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

    public Transform OriginalParent { get; set; }
    public int OriginalSiblingIndex { get; set; }

    public string Key;
}
