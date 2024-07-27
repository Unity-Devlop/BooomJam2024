using UnityEngine;

namespace Game.GamePlay
{
    public class Trainer : MonoBehaviour, ITrainer
    {
        public TrainerData data { get; private set; }
    }
}