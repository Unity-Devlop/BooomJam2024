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
        public Button cardBtn;
        public Text titleTxt;
        public Text descriptionTxt;
        public Image cardImg;

        public override void OnSelect(BaseEventData eventData)
        {
            Global.Get<AudioSystem>().PlayOneShot(FMODName.Event.SFX_ui_—°‘Ò≈∆);
            transform.localPosition += transform.up * selectionOffset;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            Global.Get<AudioSystem>().PlayOneShot(FMODName.Event.SFX_ui_—°‘Ò≈∆);
            transform.localPosition -= transform.up * selectionOffset;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {

        }

        public override void OnPointerEnter(PointerEventData eventData)
        {

        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public override void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}
