using Cysharp.Threading.Tasks;

namespace Game.GamePlay
{
    public interface ITrainer
    {
        public TrainerData trainerData { get; }
    }

    public interface IBattleController
    {
        public UniTask Enter();
        public UniTask RoundStart();
        public UniTask BeforeRound();
        public UniTask Rounding();
        public UniTask AfterRound();
        public UniTask RoundEnd();
        public UniTask Exit();
        public void Cancel();
        public bool TryGetWinner(out ITrainer trainer);
    }
}