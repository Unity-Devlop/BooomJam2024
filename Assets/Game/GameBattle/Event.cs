using UnityToolkit;

namespace Game.GamePlay
{
    public struct BattleTipEvent : IEvent
    {
        public readonly string tip;

        public BattleTipEvent(string tip)
        {
            this.tip = tip;
        }
    }
}