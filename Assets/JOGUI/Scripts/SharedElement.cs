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

    private CanvasGroup _canvasGroup;
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            return _canvasGroup;
        }
    }

    public string Name;
    public bool OverrideTransitionSettings = false;
    public float StartDelay = 0f;
    public float Duration = 0.5f;
    public EaseType EaseType = EaseType.EaseInOutCubic;
}
