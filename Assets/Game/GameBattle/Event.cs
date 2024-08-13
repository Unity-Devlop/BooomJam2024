using cfg;
using Game.GamePlay;
using UnityToolkit;

namespace Game
{
    public readonly struct BattleFlowStateEvent
    {
        public readonly IBattleFlow flow;
        public readonly IBattleFlow.BattleFlowStage stage;

        public BattleFlowStateEvent(IBattleFlow flow, IBattleFlow.BattleFlowStage stage)
        {
            this.flow = flow;
            this.stage = stage;
        }
    }

    public readonly struct BattleStateTipEvent : IEvent
    {
        public readonly string tip;

        public BattleStateTipEvent(string tip)
        {
            this.tip = tip;
        }
    }

    public readonly struct OnExecuteSkill : IEvent
    {
        public readonly IBattleTrainer user;
        public readonly ActiveSkillData data;

        public OnExecuteSkill(IBattleTrainer user, ActiveSkillData data)
        {
            this.user = user;
            this.data = data;
        }
    }

    public readonly struct OnExecuteBuff : IEvent
    {
    }

    /// <summary>
    /// 战斗中的记录信息 输出到一个列表中供玩家查看
    /// </summary>
    public struct BattleInfoRecordEvent : IEvent
    {
        public readonly string tip;

        public BattleInfoRecordEvent(string tip)
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