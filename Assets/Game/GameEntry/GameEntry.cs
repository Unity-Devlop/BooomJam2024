using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.GameEntry
{
    public class GameEntry : MonoBehaviour
    {
        private async void Start()
        {
            await UniTask.WaitUntil(() => Global.Singleton.initialized);
            Global.Get<GameFlow>().Run<GameHomeState>();
        }
    }
}