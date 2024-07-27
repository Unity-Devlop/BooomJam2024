using System;
using UnityEngine;

namespace Game.GamePlay
{
    [Serializable]
    public class RobotController : ITrainer
    {
        [field: SerializeField] public TrainerData trainerData { get; set; }
    }
}