using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class SkillCardVisual : CardVisual
    {
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI elementText;

        [SerializeField] private Image icon;
        [SerializeField] private Image attachIcon;
        [SerializeField] private TextMeshProUGUI damagePointText;
        // [SerializeField] private Image image;
        [SerializeField] private Image descBg;

        [SerializeField] private RectTransform typeContainer; // 当没有完整的卡牌时 激活这个 程序拼一个假的

        public override async void Initialize(Card card)
        {
            base.Initialize(card);
            descText.text = card.data.config.Desc;
            damagePointText.text = card.data.config.DamagePoint.ToString();
            elementText.text = Global.Table.ElementTable.Get(card.data.config.Element).Text;

            if (!string.IsNullOrEmpty(
                    card.data.config.CardImagePath) && !string.IsNullOrWhiteSpace(card.data.config.CardImagePath))
            {
                background.sprite = await Global.Get<ResourceSystem>().LoadImage(card.data.config.CardImagePath);
                typeContainer.gameObject.SetActive(false);
            }
            else
            {
                background.sprite = await Global.Get<ResourceSystem>().LoadSkillCardBg(card.data.config.Element);
                icon.sprite = await Global.Get<ResourceSystem>().LoadSkillCardElementBg(card.data.config.Element);
                // if (!string.IsNullOrEmpty(
                //         card.data.config.SpecialIconPath) &&
                //     !string.IsNullOrWhiteSpace(card.data.config.SpecialIconPath))
                // {
                //     image.sprite = await Global.Get<ResourceSystem>().LoadSkillCardImage(card.data.id);
                // }

                descBg.sprite = await Global.Get<ResourceSystem>().LoadSkillCardDescBg(card.data.config.Element);
                // image.enabled = image.sprite != null;
                typeContainer.gameObject.SetActive(true);
            }

            if (string.IsNullOrEmpty(card.data.config.SpecialIconPath) ||
                string.IsNullOrWhiteSpace(card.data.config.SpecialIconPath))
            {
                attachIcon.enabled = false;
                damagePointText.enabled = true;
            }
            else
            {
                damagePointText.enabled = false;
                attachIcon.sprite = await Global.Get<ResourceSystem>().LoadSpecialSkillIcon(card.data.id);
                attachIcon.enabled = true;
            }
        }
    }
}