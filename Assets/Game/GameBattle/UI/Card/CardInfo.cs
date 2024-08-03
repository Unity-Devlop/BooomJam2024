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
                string content = $"{obj.card.data.config.Type2}\t{obj.card.data.config.Desc}";
                infoText.text = content;
            }
            else
            {
                infoText.text = "";
            }
            
        }
    }
}