using cfg;
using Game.GamePlay;
using UnityToolkit;

namespace Game
{
    
    public readonly struct OnExecuteCommandSkill : IEvent
    {
        public readonly IBattleTrainer user;
        public readonly ActiveSkillData data;
        public OnExecuteCommandSkill(IBattleTrainer user, ActiveSkillData data)
        {
            this.user = user;
            this.data = data;
        }
    }
    public struct BattleTipEvent : IEvent
    {
        public readonly string tip;

        public BattleTipEvent(string tip)
        {
            this.tip = tip;
        }
    }

    public struct OnBattleApplyDamage : IEvent
    {
        public readonly IBattleTrainer attacker;
        public readonly IBattleTrainer defender;
        public readonly HuluData attackerHuluData;
        public readonly HuluData defenderHuluData;
        public readonly int damage;

        public OnBattleApplyDamage(IBattleTrainer attacker, IBattleTrainer defender, HuluData attackerHuluData,
            HuluData defenderHuluData, int damage)
        {
            this.attacker = attacker;
            this.defender = defender;
            this.attackerHuluData = attackerHuluData;
            this.defenderHuluData = defenderHuluData;
            this.damage = damage;
        }
    }

    public struct OnDefeatPokemon : IEvent
    {
        public readonly IBattleTrainer attacker;
        public readonly IBattleTrainer defender;
        public readonly HuluData attackerHuluData;
        public readonly HuluData defenderHuluData;

        public OnDefeatPokemon(IBattleTrainer attacker, IBattleTrainer defender, HuluData attackerHuluData,
            HuluData defenderHuluData)
        {
            this.attacker = attacker;
            this.defender = defender;
            this.attackerHuluData = attackerHuluData;
            this.defenderHuluData = defenderHuluData;
        }
    }

    public struct OnBattleCardHover : IEvent
    {
        public readonly Card card;
        public readonly bool hovering;

        public OnBattleCardHover(Card card, bool hovering)
        {
            this.card = card;
            this.hovering = hovering;
        }
    }
}