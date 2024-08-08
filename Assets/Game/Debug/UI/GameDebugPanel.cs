using System;
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
            DebugLogConsole.AddCommand("random game", "Random game data", OnRollToStartButtonClick);
        }

        public override void OnDispose()
        {
            base.OnDispose();
            DebugLogConsole.RemoveCommand("random game");
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
            GameMath.RollBattleData(out var local, out var remote, out var battleData);
            await Global.Get<GameFlow>().ToGameBattle(local, remote, battleData);

            container.gameObject.SetActive(false);
        }
    }
}