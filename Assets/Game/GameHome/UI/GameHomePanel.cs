using System;
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

        private void Awake()
        {
            newGame.onClick.AddListener(NewGameClick);
            continueGame.onClick.AddListener(ContinueGameClick);
            developerButton.onClick.AddListener(DeveloperButtonClick);
            exitGame.onClick.AddListener(ExitGameClick);
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
                Global.Get<DataSystem>().Add(data.playerData);
                // TODO 这里根据数据进行的位置判断
            }
        }

        private async void NewGameClick()
        {
            Global.Get<DataSystem>().ClearGameData();
            GameData gameData = new GameData();
            gameData.playerData = new PlayerData(true);
            Global.Get<DataSystem>().Add(gameData);
            Global.Get<DataSystem>().Add(gameData.playerData);
            await Global.Get<GameFlow>().ToGameOutside<FirstSettingState>();
        }
    }
}