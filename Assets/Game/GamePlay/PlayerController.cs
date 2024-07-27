using System;

namespace Game.GamePlay
{
    [Serializable]
    public class PlayerController : ITrainer
    {
        public PlayerData playerData;
        public TrainerData trainerData => playerData.trainerData;
    }
}