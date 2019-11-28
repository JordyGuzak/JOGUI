using System.Collections.Generic;
using UnityEngine;

namespace JOGUI
{
    [RequireComponent(typeof(UITweenRunner)), RequireComponent(typeof(CanvasGroup))]
    public class UIElement : MonoBehaviour
    {
        [System.Serializable]
        public class SlideInOptions : UITweenOptions
        {
            public ScreenPosition From;
        }

        [System.Serializable]
        public class SlideOutOptions : UITweenOptions
        {
            public ScreenPosition To;
        }

        [System.Serializable]
        public class FadeOptions : UITweenOptions
        {
            [Range(0f, 1f)]
            public float From;

            [Range(0f, 1f)]
            public float To;
        }

        [Header("On Show")]
        [SerializeField] bool _animateOnEnter = true;
        [SerializeField] bool _playSlideInAnimation = true;
        [SerializeField] bool _playFadeInAnimation = true;
        [SerializeField] SlideInOptions _slideInOptions = new SlideInOptions() { From = ScreenPosition.Left, EaseType = EaseType.EaseOutCubic };
        [SerializeField] FadeOptions _fadeInOptions = new FadeOptions() { From = 0f, To = 1f, EaseType = EaseType.EaseOutCubic };

        [Header("On Hide")]
        [SerializeField] bool _animateOnExit = true;
        [SerializeField] bool _playSlideOutAnimation = true;
        [SerializeField] bool _playFadeOutAnimation = true;
        [SerializeField] SlideOutOptions _slideOutOptions = new SlideOutOptions() { To = ScreenPosition.Left, EaseType = EaseType.EaseInCubic };
        [SerializeField] FadeOptions _fadeOutOptions = new FadeOptions() { From = 1f, To = 0f, EaseType = EaseType.EaseInCubic };

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private UITweenRunner _tweenRunner;
        private ITween _slideIn, _slideOut, _fadeIn, _fadeOut;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _tweenRunner = GetComponent<UITweenRunner>();
        }

        private void Start()
        {
            InitializeTweens();
        }

        public void Show(bool instant = false)
        {
            if (instant)
            {
                gameObject.SetActive(true);
            }
            else
            {
                PlayEnterAnimation();
            }
        }

        public void Hide(bool instant = false)
        {
            if (instant)
            {
                gameObject.SetActive(false);
            }
            else
            {
                PlayExitAnimation();
            }
        }

        private void InitializeTweens()
        {
            var rectTransform = (RectTransform)transform;
            var parent = (RectTransform)transform.parent;

            //_slideIn = new UITween<Vector2>(position => rectTransform.anchoredPosition = position)
            //    .SetFrom(GetPositionRelativeToParent(parent, _slideInOptions.From))
            //    .SetTo(rectTransform.anchoredPosition)
            //    .SetOptions(_slideInOptions);

            //_slideOut = new UITween<Vector2>(position => rectTransform.anchoredPosition = position)
            //    .SetFrom(rectTransform.anchoredPosition)
            //    .SetTo(GetPositionRelativeToParent(parent, _slideOutOptions.To))
            //    .SetOptions(_slideOutOptions);

            //_fadeIn = new UITween<float>(alpha => _canvasGroup.alpha = alpha)
            //    .SetFrom(_fadeInOptions.From)
            //    .SetTo(_fadeInOptions.To)
            //    .SetOptions(_fadeInOptions);

            //_fadeOut = new UITween<float>(alpha => _canvasGroup.alpha = alpha)
            //    .SetFrom(_fadeOutOptions.From)
            //    .SetTo(_fadeOutOptions.To)
            //    .SetOptions(_fadeOutOptions);
        }

        private void PlayEnterAnimation()
        {
            var animationsToPlay = new List<ITween>();

            if (_playSlideInAnimation)
                animationsToPlay.Add(_slideIn);

            if (_playFadeInAnimation)
                animationsToPlay.Add(_fadeIn);

            _tweenRunner.Play(animationsToPlay.ToArray());
        }

        private void PlayExitAnimation()
        {
            var animationsToPlay = new List<ITween>();

            if (_playSlideOutAnimation)
                animationsToPlay.Add(_slideOut);

            if (_playFadeOutAnimation)
                animationsToPlay.Add(_fadeOut);

            _tweenRunner.Play(animationsToPlay.ToArray());
        }

        private Vector2 GetPositionRelativeToParent(RectTransform parent, ScreenPosition screenPosition)
        {
            switch (screenPosition)
            {
                case ScreenPosition.Left:
                    return Vector2.left * parent.rect.width;
                case ScreenPosition.Right:
                    return Vector2.right * parent.rect.width;
                case ScreenPosition.Top:
                    return Vector2.up * parent.rect.height;
                case ScreenPosition.Bottom:
                    return Vector2.down * parent.rect.height;
                default:
                    return Vector2.left * parent.rect.width;
            }
        }
    }
}
