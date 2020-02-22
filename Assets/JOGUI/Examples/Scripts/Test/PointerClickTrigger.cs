using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace JOGUI
{
    public class PointerClickTrigger : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent onPointerClick;

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.Invoke();
        }
    }
}
