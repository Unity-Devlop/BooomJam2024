using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{

    public class CardItem : Card
    {
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            ClickCardEvent e = new ClickCardEvent();
            e.data = data;
            Global.Event.Send<ClickCardEvent>(e);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

        }

        public override void OnBeginDrag(PointerEventData eventData)
        {

        }

        public override void OnDrag(PointerEventData eventData)
        {
           
        }

        public override void OnEndDrag(PointerEventData eventData)
        {

        }
    }
}
