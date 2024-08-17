using System.Text;
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
        // [SerializeField] private Button rollToStartButton;
        // [SerializeField] private Button debugToStartButton;
        // [SerializeField] private Button frameButton;
        // [SerializeField] private Button debugger;
        // [SerializeField] private RectTransform container;
        // [SerializeField] private DebugDragButton dragButton;

        // private void Awake()
        // {
        //     dragButton.onClick.AddListener(OnDragButtonClick);
        //     rollToStartButton.onClick.AddListener(OnRollToStartButtonClick);
        //     debugToStartButton.onClick.AddListener(OnDebugToStartButtonClick);
        //     frameButton.onClick.AddListener(OnFrameButtonClick);
        //     debugger.onClick.AddListener(OnDebuggerButtonClick);
        // }


        // public override void OnLoaded()
        // {
        //     base.OnLoaded();
        //     // Register Command
        // }
        //
        // public override void OnDispose()
        // {
        //     base.OnDispose();
        //     // UnRegister Command
        // }

        public override void OnOpened()
        {
            Global.LogInfo("GameDebugPanel Opened");
            base.OnOpened();
            DebugLogManager.Instance.gameObject.SetActive(true);
            DebugLogConsole.AddCommand("Increase1000Atk", "", () =>
            {
                if (Global.Get<GameFlow>().currentState is GameBattleState)
                {
                    GameBattleMgr.Singleton.playerBattleTrainer.currentBattleData.IncreaseAtk(1000, true).ContinueWith(
                        () =>
                        {
                            Global.LogInfo(
                                $"{GameBattleMgr.Singleton.playerBattleTrainer.currentBattleData}当前攻击力：{GameBattleMgr.Singleton.playerBattleTrainer.currentBattleData.currentAtk}");
                        });
                }
            });
            DebugLogConsole.AddCommand("Increase1000AtkEnemy", "给对面当前宝可梦增加1000点攻击力", () =>
            {
                if (Global.Get<GameFlow>().currentState is GameBattleState)
                {
                    GameBattleMgr.Singleton.robotBattleTrainer.currentBattleData.IncreaseAtk(1000, true).ContinueWith(
                        () =>
                        {
                            Global.LogInfo(
                                $"{GameBattleMgr.Singleton.robotBattleTrainer.currentBattleData}当前攻击力：{GameBattleMgr.Singleton.robotBattleTrainer.currentBattleData.currentAtk}");
                        });
                }
            });
            DebugLogConsole.AddCommand($"ShowSelfDraw", "显示自己的抽牌区域", () =>
            {
                if (Global.Get<GameFlow>().currentState is GameBattleState)
                {
                    foreach (var skillData in GameBattleMgr.Singleton.playerBattleTrainer.drawZone)
                    {
                        Global.LogInfo(skillData.ToString());
                    }
                }
            });
            DebugLogConsole.AddCommand($"ShowSelfConsume", "显示自己的消耗区域", () =>
            {
                if (Global.Get<GameFlow>().currentState is GameBattleState)
                {
                    foreach (var skillData in GameBattleMgr.Singleton.playerBattleTrainer.consumedZone)
                    {
                        Global.LogInfo(skillData.ToString());
                    }
                }
            });
            DebugLogConsole.AddCommand($"ShowEnemyDraw", "显示对面的抽牌区域", () =>
            {
                if (Global.Get<GameFlow>().currentState is GameBattleState)
                {
                    foreach (var skillData in GameBattleMgr.Singleton.robotBattleTrainer.drawZone)
                    {
                        Global.LogInfo(skillData.ToString());
                    }
                }
            });

            DebugLogConsole.AddCommand($"ShowEnemyConsume", "显示对面的消耗区域", () =>
            {
                if (Global.Get<GameFlow>().currentState is GameBattleState)
                {
                    foreach (var skillData in GameBattleMgr.Singleton.robotBattleTrainer.consumedZone)
                    {
                        Global.LogInfo(skillData.ToString());
                    }
                }
            });


            DebugLogConsole.AddCommand($"ShowEnemyHand", "显示对面的手牌", () =>
            {
                if (Global.Get<GameFlow>().currentState is GameBattleState)
                {
                    foreach (var skillData in GameBattleMgr.Singleton.robotBattleTrainer.handZone)
                    {
                        Global.LogInfo(skillData.ToString());
                    }
                }
            });

            DebugLogConsole.AddCommand($"LogSystemInfo", "", DebugLogConsole.LogSystemInfo);

            DebugLogConsole.AddCommand($"600FrameRate", "设置帧率为600", () => { Application.targetFrameRate = 600; });
        }

        public override void OnClosed()
        {
            base.OnClosed();
            DebugLogManager.Instance.gameObject.SetActive(false);
            DebugLogConsole.RemoveCommand("Increase1000Atk");
            DebugLogConsole.RemoveCommand("Increase1000AtkEnemy");
            DebugLogConsole.RemoveCommand("ShowSelfDraw");
            DebugLogConsole.RemoveCommand("ShowSelfConsume");
            DebugLogConsole.RemoveCommand("ShowEnemyDraw");
            DebugLogConsole.RemoveCommand("ShowEnemyConsume");
            DebugLogConsole.RemoveCommand("ShowEnemyHand");
            DebugLogConsole.RemoveCommand("LogSystemInfo");
            DebugLogConsole.RemoveCommand("600FrameRate");
        }

        // private void OnDebuggerButtonClick()
        // {
        //     DebuggerComponent component =
        //         Global.Singleton.GetComponentInChildren<DebuggerComponent>(true);
        //     if (component != null)
        //     {
        //         component.gameObject.SetActive(!component.gameObject.activeInHierarchy);
        //     }
        // }
        //
        // private void OnDragButtonClick()
        // {
        //     container.gameObject.SetActive(!container.gameObject.activeSelf);
        // }
        //
        // private void OnFrameButtonClick()
        // {
        //     Application.targetFrameRate = 600;
        // }
        //
        // private void OnDebugToStartButtonClick()
        // {
        //     GameBattleMgr.Singleton.DebugStartBattle();
        //     CloseSelf();
        // }
        //
        // private async void OnRollToStartButtonClick()
        // {
        //     // if (Global.Get<DataSystem>().LoadPrevGameData(out GameData gameData))
        //     // {
        //     //     Global.Get<DataSystem>().Add(gameData);
        //     //     Global.LogInfo($"Load Game Data:{gameData},outsideState:{gameData.gameOutsideStateType}");
        //     //     await Global.Get<GameFlow>().ToGameOutside(gameData.gameOutsideStateType);
        //     //     container.gameObject.SetActive(false);
        //     //     return;
        //     // }
        //
        //     if (Global.Get<GameFlow>().currentState is GameBattleState)
        //     {
        //         await Global.Get<GameFlow>().ToGameHome();
        //         await UniTask.WaitUntil(() => SceneManager.GetActiveScene().name == "GameHome");
        //         GameMath.RollBattleData(out var local, out var remote, out var battleData);
        //         await Global.Get<GameFlow>().ToGameBattle(local, remote, battleData);
        //         container.gameObject.SetActive(false);
        //     }
        //     else
        //     {
        //         GameMath.RollBattleData(out var local, out var remote, out var battleData);
        //         await Global.Get<GameFlow>().ToGameBattle(local, remote, battleData);
        //         container.gameObject.SetActive(false);
        //     }
        // }
    }
}