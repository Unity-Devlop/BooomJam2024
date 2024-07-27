using System;
using UnityEngine;

namespace Game.GamePlay
{
    [Serializable]
    public class PlayerController : ITrainer, ITrainerController
    {
        [field: SerializeField] public TrainerData data { get; private set; }
    }
}