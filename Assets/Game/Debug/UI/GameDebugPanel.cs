using System;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityToolkit;
using UnityToolkit.Debugger;

namespace Game
{
    public class GameDebugPanel : UIPanel
    {
        [SerializeField] private Button rollToStartButton;
        [SerializeField] private Button debugToStartButton;
        [SerializeField] private Button frameButton;
        [SerializeField] private Button debugger;
        [SerializeField] private RectTransform container;
        [SerializeField] private DebugDragButton dragButton;

        private void Awake()
        {
            dragButton.onClick.AddListener(OnDragButtonClick);
            rollToStartButton.onClick.AddListener(OnRollToStartButtonClick);
            debugToStartButton.onClick.AddListener(OnDebugToStartButtonClick);
            frameButton.onClick.AddListener(OnFrameButtonClick);
            debugger.onClick.AddListener(OnDebuggerButtonClick);
        }


        public override void OnLoaded()
        {
            base.OnLoaded();
            AddDebugCommand();
        }

        public static void AddDebugCommand()
        {
            DebugLogConsole.AddCommand("random-battle", "Start a random battle", RollToStart);
        }

        public static void RemoveDebugCommand()
        {
            DebugLogConsole.RemoveCommand("random-battle");
        }

        private static async UniTask RollToStart()
        {
            GameMath.RollBattleData(out var local, out var remote, out var battleData);
            await Global.Get<GameFlow>().ToGameBattle(local, remote, battleData);
        }


        public override void OnDispose()
        {
            base.OnDispose();
            RemoveDebugCommand();
        }

        private void OnDebuggerButtonClick()
        {
            DebuggerComponent component =
                Global.Singleton.GetComponentInChildren<DebuggerComponent>(true);
            if (component != null)
            {
                component.gameObject.SetActive(!component.gameObject.activeInHierarchy);
            }
        }

        private void OnDragButtonClick()
        {
            container.gameObject.SetActive(!container.gameObject.activeSelf);
        }

        private void OnFrameButtonClick()
        {
            Application.targetFrameRate = 600;
        }

        private void OnDebugToStartButtonClick()
        {
            GameBattleMgr.Singleton.DebugStartBattle();
            CloseSelf();
        }

        private async void OnRollToStartButtonClick()
        {
            await RollToStart();
            container.gameObject.SetActive(false);
        }
    }
}