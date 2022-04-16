using UnityEngine;

namespace JOGUI
{
    public class CanvasGroupFadeTarget : IFadeTarget
    {
        private CanvasGroup _canvasGroup;

        public CanvasGroupFadeTarget(CanvasGroup canvasGroup)
        {
            _canvasGroup = canvasGroup;
        }

        public void SetAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        public Object GetOnDestroyLink() => _canvasGroup;
    }
}
