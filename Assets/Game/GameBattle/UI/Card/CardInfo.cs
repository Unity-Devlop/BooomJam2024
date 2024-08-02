using System;
using Game.GamePlay;
using TMPro;
using UnityEngine;

namespace Game
{
    public class CardInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI infoText;

        private void OnEnable()
        {
            Global.Event.Listen<OnCardHover>(OnCardHover);
        }

        private void OnDisable()
        {
            Global.Event.UnListen<OnCardHover>(OnCardHover);
        }

        private void OnCardHover(OnCardHover obj)
        {
            if (obj.hovering)
            {
                infoText.text = obj.card.data.config.Desc;
            }
            else
            {
                infoText.text = "";
            }
            
        }
    }
}