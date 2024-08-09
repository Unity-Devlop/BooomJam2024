using System;
using cfg;
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
            Global.Event.Listen<OnBattleCardHover>(OnCardHover);
        }

        private void OnDisable()
        {
            Global.Event.UnListen<OnBattleCardHover>(OnCardHover);
        }

        private void OnCardHover(OnBattleCardHover obj)
        {
            if (obj.card.data.id == ActiveSkillEnum.None)
            {
                Global.LogWarning($"{this}卡牌ID为None");
                return;
            }
            if (obj.hovering)
            {
                string content = $"{obj.card.data.config.Desc}";
                infoText.text = content;
            }
            else
            {
                infoText.text = "";
            }
            
        }
    }
}