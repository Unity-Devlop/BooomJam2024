using System;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityToolkit;

namespace Game.GameEntry
{
    public class GameEntry : MonoBehaviour
    {
        [SerializeField] private DebugLogManager manager;

        public TextMeshProUGUI text;
        private async void Start()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 144;
            UniTask task = UniTask.WaitUntil(() => Global.Singleton.initialized);
            
            // 任务没有完成时，显示加载界面
            while (!task.Status.IsCompleted())
            {
                await UniTask.Delay(500);
                if(task.Status.IsCompleted()) break;
                text.text = $"Loading";
                await UniTask.Delay(500);
                if(task.Status.IsCompleted()) break;
                text.text = $"Loading.";
                await UniTask.Delay(500);
                if(task.Status.IsCompleted()) break;
                text.text = $"Loading..";
                await UniTask.Delay(500);
                if(task.Status.IsCompleted()) break;
                text.text = $"Loading...";
                await UniTask.Delay(500);
                if(task.Status.IsCompleted()) break;
            }
            
            await task;
#if UNITY_EDITOR
            manager.Awake();
            manager.gameObject.SetActive(true);
            await UIRoot.Singleton.OpenPanelAsync<GameDebugPanel>();
#else
            manager.Awake();
            manager.gameObject.SetActive(false);
#endif
            Global.Get<GameFlow>().Run<GameHomeState>();
        }
    }
}