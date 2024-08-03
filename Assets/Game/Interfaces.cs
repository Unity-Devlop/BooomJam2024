using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;

namespace Game.GamePlay
{
    public interface IBattleOperation
    {
    }

    public interface IBattleTrainer
    {
        public bool canFight { get; }
        public TrainerData trainerData { get; }
        public HuluData currentBattleData { get; }
        

        public HashSet<ActiveSkillData> handZone { get; }

        public HuluData Get(int idx)
        {
            return trainerData.datas[idx];
        }

        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard;

        public event Func<ActiveSkillData, UniTask> OnUseCardFromHand;
        public event Func<List<ActiveSkillData>, UniTask> OnDestroyCard;
        public event Func<List<ActiveSkillData>, IBattleTrainer, UniTask> OnDiscardCardFromHand;
        public event Func<List<ActiveSkillData>, UniTask> OnConsumedCard;

        public event Func<List<ActiveSkillData>, List<ActiveSkillData>, UniTask> OnDiscardToDraw;

        public event Func<UniTask> OnStartCalOperation;
        public event Func<UniTask> OnEndCalOperation;

        public void PushOperation(IBattleOperation operation);

        public void ClearOperation();
        public UniTask<IBattleOperation> CalOperation();

        public UniTask SwitchPokemon(HuluData data);
        public UniTask DrawSkills(int cnt);

        public UniTask UseCardFromHand(ActiveSkillData data);
        UniTask RandomDiscardCardFromHand(int i);
        UniTask DiscardAllHandCards();
        UniTask Discard2DrawZone();
        UniTask<int> DrawTarget(ActiveSkillTypeEnum type, int cnt);

        UniTask<int> DrawTarget(ActiveSkillEnum target, int cnt);

        int GetTargetCntInDeck(ActiveSkillTypeEnum targetType);
        UniTask AddCardToDeck(ActiveSkillData added);
        UniTask DrawHandFull();


        void ExitBattle();
        UniTask ConsumeCardFromHand(ActiveSkillData data);

        UniTask OnEnemyTrainerDiscardCard(List<ActiveSkillData> arg, IBattleTrainer trainer);
        UniTask RemoveBuff(BattleBuffEnum buff);
        UniTask BeforeRounding();
        UniTask AddBuff(BattleBuffEnum buff);
        UniTask MoveDiscardCardToConsumeZone(ActiveSkillData data);
        bool ContainsBuff(BattleBuffEnum buff);
        int GetConsumeCardInHandCount(ActiveSkillTypeEnum target);
    }

    public interface IBattleFlow
    {
        public UniTask Enter();
        public UniTask RoundStart();
        public UniTask BeforeRound();
        public UniTask Rounding();
        public UniTask AfterRound();
        public UniTask RoundEnd();
        public UniTask Exit();
        public void Cancel();
        public bool TryGetRoundWinner(out IBattleTrainer battleTrainer);

        public bool TryGetFinalWinner(out IBattleTrainer battleTrainer);

        public static async UniTask RoundFlow(IBattleFlow flow, CancellationToken token)
        {
            IBattleTrainer winner;
            while (!token.IsCancellationRequested)
            {
                await flow.RoundStart(); // 回合开始
                await flow.BeforeRound(); // 回合开始前
                await flow.Rounding(); // 回合进行
                if (flow.TryGetFinalWinner(out winner))
                {
                    break;
                }

                await flow.AfterRound();
                if (flow.TryGetFinalWinner(out winner))
                {
                    break;
                }

                await flow.RoundEnd();

                await UniTask.DelayFrame(1, cancellationToken: token);
            }

            // 执行退出战斗流程
            await flow.Exit();
        }
    }
}