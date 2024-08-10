using System;
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
                FindObjectOfType<DebugLogManager>(true).gameObject.SetActive(true);
                UIRoot.Singleton.OpenPanel<GameDebugPanel>();
            }
            else
            {
                FindObjectOfType<DebugLogManager>(true).gameObject.SetActive(false);
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
        }

        private async void ContinueGameClick()
        {
            if (Global.Get<DataSystem>().LoadPrevGameData(out GameData data))
            {
                Global.Get<DataSystem>().Add(data);
                // TODO 这里根据数据进行的位置判断
            }
        }

        private async void NewGameClick()
        {
            Global.Get<DataSystem>().ClearGameData();
            GameData gameData = new GameData();
            gameData.playerData = new PlayerData(true);
            Global.Get<DataSystem>().Add(gameData);
            await Global.Get<GameFlow>().ToGameOutside<FirstSettingState>();
        }
    }
}