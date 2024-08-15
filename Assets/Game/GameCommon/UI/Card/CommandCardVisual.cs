using TMPro;
using UnityEngine;

namespace Game
{
    public class CommandCardVisual : CardVisual
    {
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI nameEngText;

        [SerializeField] private RectTransform typeContainer;

        public override async void Initialize(Card card)
        {
            base.Initialize(card);

            if (!string.IsNullOrEmpty(
                    card.data.config.CardImagePath) && !string.IsNullOrWhiteSpace(card.data.config.CardImagePath))
            {
                background.sprite = await Global.Get<ResourceSystem>().LoadImage(card.data.config.CardImagePath);
                typeContainer.gameObject.SetActive(false);
            }
            else
            {
                background.sprite = await Global.Get<ResourceSystem>()
                    .LoadImage("UI/CardAtlas/command/Command_Porsche_background.png");
                typeContainer.gameObject.SetActive(true);
            }


            descText.text = card.data.config.Desc;
            nameEngText.text = card.data.config.EngName;
        }
    }
}