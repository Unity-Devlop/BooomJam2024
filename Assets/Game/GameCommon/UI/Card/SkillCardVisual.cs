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

        [SerializeField] private TextMeshProUGUI damagePointText;
        [SerializeField] private Image image;
        [SerializeField] private Image descBg;

        public override async void Initialize(Card card)
        {
            base.Initialize(card);
            descText.text = card.data.config.Desc;
            damagePointText.text = card.data.config.DamagePoint.ToString();
            elementText.text = Global.Table.ElementFitTable.Get(card.data.config.Element).Text;

            background.sprite = await Global.Get<ResourceSystem>().LoadSkillCardBg(card.data.config.Element);

            if (string.IsNullOrEmpty(card.data.config.SpecialIconPath) ||
                string.IsNullOrWhiteSpace(card.data.config.SpecialIconPath))
            {
                damagePointText.enabled = true;
                icon.sprite = await Global.Get<ResourceSystem>().LoadSkillCardElementBg(card.data.config.Element);
            }
            else
            {
                damagePointText.enabled = false;
                icon.sprite = await Global.Get<ResourceSystem>().LoadSpecialSkillIcon(card.data.id);
            }

            if (!string.IsNullOrEmpty(
                    card.data.config.SpecialIconPath) && !string.IsNullOrWhiteSpace(card.data.config.SpecialIconPath))
            {
                image.sprite = await Global.Get<ResourceSystem>().LoadSkillCardImage(card.data.id);
            }

            descBg.sprite = await Global.Get<ResourceSystem>().LoadSkillCardDescBg(card.data.config.Element);
            image.enabled = image.sprite != null;
        }
    }
}