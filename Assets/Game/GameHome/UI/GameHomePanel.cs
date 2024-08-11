using System;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game.GameHome
{
    public class GameHomePanel : UIPanel
    {
        [SerializeField] private Button newGame;
        [SerializeField] private Button continueGame;
        [SerializeField] private Button developerButton;
        [SerializeField] private Button exitGame;
        [SerializeField] private Button devButton;

        private void Awake()
        {
            newGame.onClick.AddListener(NewGameClick);
            continueGame.onClick.AddListener(ContinueGameClick);
            developerButton.onClick.AddListener(DeveloperButtonClick);
            exitGame.onClick.AddListener(ExitGameClick);
            devButton.onClick.AddListener(DevButtonClick);
        }

        public bool debugOn = false;

        private void DevButtonClick()
        {
            if (!debugOn)
            {
                UIRoot.Singleton.OpenPanel<GameDebugPanel>();
            }
            else
            {
                UIRoot.Singleton.ClosePanel<GameDebugPanel>();
            }

            debugOn = !debugOn;
        }

        private void ExitGameClick()
        {
            Application.Quit();
        }

        private void DeveloperButtonClick()
        {
            UIRoot.Singleton.OpenPanel<DeveloperPanel>();
        }

        private async void ContinueGameClick()
        {
            if (Global.Get<DataSystem>().LoadPrevGameData(out GameData data))
            {
                Global.Get<DataSystem>().Add(data);
                Global.LogInfo($"Load Game Data:{data},outsideState:{data.gameOutsideStateType}");
                await Global.Get<GameFlow>().ToGameOutside(data.gameOutsideStateType);
            }
        }

        private async void NewGameClick()
        {
            Global.Get<DataSystem>().ClearGameData();
            
            // 构造一个新的GameData
            GameData gameData = new GameData();
            gameData.playerData = new PlayerData(true);
            GameRuleConfig gameRuleConfig = new GameRuleConfig();
            gameRuleConfig.ruleList = new HashSet<GameRuleEnum>();
            gameRuleConfig.prevCnt = 0;
            gameData.ruleConfig = gameRuleConfig;
            
            
            Global.Get<DataSystem>().Add(gameData);
            await Global.Get<GameFlow>().ToGameOutside<FirstSettingState>();
        }
    }
}