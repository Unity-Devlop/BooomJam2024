using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(GameFlow))]
    public class GameFlowQuickDebugger : MonoBehaviour
    {
        public TrainerData data;

        private async void Awake()
        {
            await UniTask.DelayFrame(2);
            GetComponent<GameFlow>().SetParam(nameof(TrainerData), data);
        }
    }
}