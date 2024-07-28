using System;
using System.Collections.Generic;
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
        public HuluData currentData { get; }

        public HuluData Get(int idx)
        {
            return trainerData.datas[idx];
        }

        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard;

        public event Func<ActiveSkillData, UniTask> OnUseCard; 

        public event Func<List<ActiveSkillData>, UniTask> OnDiscardCard;

        public event Func<UniTask> OnStartCalOperation;
        public event Func<UniTask> OnEndCalOperation;
        public void PushOperation(IBattleOperation operation);


        public UniTask<IBattleOperation> CalOperation();

        public UniTask ChangeHulu(HuluData data);
        public UniTask DrawSkills(int cnt);


        public UniTask OnUseSkill(ActiveSkillData data);
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