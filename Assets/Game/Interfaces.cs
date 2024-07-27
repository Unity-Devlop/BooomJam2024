using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.GamePlay
{
    public interface ITrainer
    {
        public bool canFight { get; }
        public TrainerData trainerData { get; }
        public HuluData currentData { get; }

        public HuluData Get(int idx)
        {
            return trainerData.datas[idx];
        }

        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard;

        public event Func<List<ActiveSkillData>, UniTask> OnDiscardCard;
        // public event Action OnDiscardCard;
        // public event Action OnUseCard;

        public void OnConsume(OnActiveCardConsume obj);
        public UniTask ChangeHulu(HuluData data);
        public UniTask DrawSkills(int cnt);
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
        public bool TryGetRoundWinner(out ITrainer trainer);

        public bool TryGetFinalWinner(out ITrainer trainer);
    }
}