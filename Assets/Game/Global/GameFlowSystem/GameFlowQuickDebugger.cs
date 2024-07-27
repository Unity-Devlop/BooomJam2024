using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(GameFlow))]
    public class GameFlowQuickDebugger : MonoBehaviour
    {
        public BattleData data;

        private async void Awake()
        {
            await UniTask.DelayFrame(2);
            GetComponent<GameFlow>().SetParam(nameof(BattleData), data);
        }
    }
}