using Cysharp.Threading.Tasks;
using Game.GamePlay;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            // Register Command
        }

        public override void OnDispose()
        {
            base.OnDispose();
            // UnRegister Command
        }

        public override void OnOpened()
        {
            base.OnOpened();
            DebugLogManager.Instance.gameObject.SetActive(true);
        }

        public override void OnClosed()
        {
            base.OnClosed();
            DebugLogManager.Instance.gameObject.SetActive(false);
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
            // if (Global.Get<DataSystem>().LoadPrevGameData(out GameData gameData))
            // {
            //     Global.Get<DataSystem>().Add(gameData);
            //     Global.LogInfo($"Load Game Data:{gameData},outsideState:{gameData.gameOutsideStateType}");
            //     await Global.Get<GameFlow>().ToGameOutside(gameData.gameOutsideStateType);
            //     container.gameObject.SetActive(false);
            //     return;
            // }

            if (Global.Get<GameFlow>().currentState is GameBattleState)
            {
                await Global.Get<GameFlow>().ToGameHome();
                await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == "GameHome");
                GameMath.RollBattleData(out var local, out var remote, out var battleData);
                await Global.Get<GameFlow>().ToGameBattle(local, remote, battleData);
                container.gameObject.SetActive(false);
            }
            else
            {
                GameMath.RollBattleData(out var local, out var remote, out var battleData);
                await Global.Get<GameFlow>().ToGameBattle(local, remote, battleData);
                container.gameObject.SetActive(false);
            }
        }
    }
}