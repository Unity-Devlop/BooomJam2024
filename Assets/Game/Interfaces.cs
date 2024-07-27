using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.GamePlay
{
    public interface ITrainer
    {
        public TrainerData trainerData { get; }

        public event Action<List<ActiveSkillData>> OnDrawCard;
        // public event Action OnDiscardCard;
        // public event Action OnUseCard;

        public void ChangeHulu(HuluData data);
        public void DrawSkills();
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