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
        [SerializeField] private DebugLogManager manager;

        private async void Start()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 144;
            await UniTask.WaitUntil(() => Global.Singleton.initialized);

#if UNITY_EDITOR
            manager.Awake();
            manager.gameObject.SetActive(true);
            UIRoot.Singleton.OpenPanel<GameDebugPanel>();
#else
            manager.Awake();
            manager.gameObject.SetActive(false);
#endif
            Global.Get<GameFlow>().Run<GameHomeState>();
        }
    }
}