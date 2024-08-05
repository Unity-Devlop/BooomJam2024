using TMPro;
using UnityEngine;

namespace Game
{
    public class CommandCardVisual : CardVisual
    {
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI nameEngText;

        public override async void Initialize(Card card)
        {
            base.Initialize(card);
            descText.text = card.data.config.Desc;
            nameEngText.text = card.data.config.EngName;
        }
    }
}