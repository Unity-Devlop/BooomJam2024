using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BuffCardVisual : CardVisual
    {
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI elementText;
        [SerializeField] private Image elementBg1;
        [SerializeField] private Image iconImage;

        public override async void Initialize(Card card)
        {
            base.Initialize(card);
            descText.text = card.data.config.Desc;
            elementText.text = Global.Table.ElementFitTable.Get(card.data.config.Element).Text;
            elementBg1.sprite = await Global.Get<ResourceSystem>().LoadSkillCardElementBg(card.data.config.Element);
            iconImage.sprite = await Global.Get<ResourceSystem>().LoadSkillCardImage(card.data.id);
            iconImage.enabled = iconImage.sprite != null;
        }
    }
}