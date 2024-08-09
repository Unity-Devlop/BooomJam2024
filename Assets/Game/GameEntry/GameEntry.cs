using System;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
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

#if UNITY_EDITOR
            FindObjectOfType<DebugLogManager>().gameObject.SetActive(true);
            UIRoot.Singleton.OpenPanel<GameDebugPanel>();
#endif

            Global.Get<GameFlow>().Run<GameHomeState>();
        }
    }
}