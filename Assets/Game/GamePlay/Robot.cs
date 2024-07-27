using UnityEngine;

namespace Game.GamePlay
{
    public class Robot : MonoBehaviour, ITrainer
    {
        public TrainerData data { get; private set; }
    }
}