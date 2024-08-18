using System;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityToolkit;

namespace Game.GameEntry
{
    public class GameEntry : MonoBehaviour
    {
        [SerializeField] private DebugLogManager manager;

        // public Image background;
        public TextMeshProUGUI text;

        // private void Awake()
        // {
// #if UNITY_WEBGL
//             background.material = null;
// #endif
        // }

        private async void Start()
        {
            Application.runInBackground = true;
            Application.targetFrameRate = 144;
            UniTask task = UniTask.WaitUntil(() => Global.Singleton.initialized);
            
            // 任务没有完成时，显示加载界面
            while (!task.Status.IsCompleted())
            {
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
                text.text = $"Loading";
                await UniTask.Delay(500);
                if(task.Status.IsCompleted()) break;
            }
            
            await task;
            manager.Awake();
            manager.gameObject.SetActive(false);
            Global.Get<GameFlow>().Run<GameHomeState>();
        }
    }
}