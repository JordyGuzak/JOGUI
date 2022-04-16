using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace JOGUI.Examples
{
    public class Blocker : MonoBehaviour, IPointerClickHandler, IFadeTarget
    {
        public Image blockerImage;
        public UnityEvent onPointerClick;

        public void SetAlpha(float alpha)
        {
            var color = blockerImage.color;
            color.a = alpha;
            blockerImage.color = color;
        }

        public Object GetOnDestroyLink() => this;

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.Invoke();
        }
    }
}
