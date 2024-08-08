using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityToolkit;

namespace Game.GameEntry
{
    public class GameEntry : MonoBehaviour
    {
        private async void Start()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 144;
            await UniTask.WaitUntil(() => Global.Singleton.initialized);
            UIRoot.Singleton.OpenPanel<GameDebugPanel>();
            Global.Get<GameFlow>().Run<GameHomeState>();
        }
    }
}