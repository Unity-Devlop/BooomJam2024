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

        private async void DevButtonClick()
        {
            if (!debugOn)
            {
                await UIRoot.Singleton.OpenPanelAsync<GameDebugPanel>();
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

        private async void DeveloperButtonClick()
        {
            await UIRoot.Singleton.OpenPanelAsync<DeveloperPanel>();
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
            GameData gameData = GameData.CreateEmpty();


            Global.Get<DataSystem>().Add(gameData);
            await Global.Get<GameFlow>().ToGameOutside<FirstSettingState>();
        }
    }
}