using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class DebugDragButton : Button, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            var rectTransform = transform as RectTransform;
            rectTransform.anchoredPosition += eventData.delta;
        }
    }
}