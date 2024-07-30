using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public HuluData Get(int idx)
        {
            return trainerData.datas[idx];
        }

        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard;

        public event Func<ActiveSkillData, UniTask> OnUseHandCard;
        public event Func<List<ActiveSkillData>, UniTask> OnDestroyCard;
        public event Func<List<ActiveSkillData>, UniTask> OnDiscardCard;

        public event Func<List<ActiveSkillData>, List<ActiveSkillData>, UniTask> OnDiscardToDraw;

        public event Func<UniTask> OnStartCalOperation;
        public event Func<UniTask> OnEndCalOperation;
        public void PushOperation(IBattleOperation operation);

        public void ClearOperation();
        public UniTask<IBattleOperation> CalOperation();

        public UniTask ChangeCurrentHulu(HuluData data);
        public UniTask DrawSkills(int cnt);

        public UniTask UseCardFromHandZone(ActiveSkillData data);
        UniTask RandomDiscard(int i);
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
    }
}