using UnityToolkit;

namespace Game
{
    public struct BattleTipEvent : IEvent
    {
        public readonly string tip;

        public BattleTipEvent(string tip)
        {
            this.tip = tip;
        }
    }

    public struct OnCardHover : IEvent
    {
        public readonly Card card;
        public readonly bool hovering;
        public OnCardHover(Card card, bool hovering)
        {
            this.card = card;
            this.hovering = hovering;
        }
    }
}