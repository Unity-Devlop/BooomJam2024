using System;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game.GameHome
{
    public class GameHomePanel : UIPanel
    {
        [SerializeField] private Button enterGame;

        private void Awake()
        {
            enterGame.onClick.AddListener(OnEnterGameClick);
        }

        private async void OnEnterGameClick()
        {
            // await Global.Get<GameFlow>().ToGameBattle();
            // TODO 根据存档情况进行判断
            await Global.Get<GameFlow>().ToGameOutside<FirstSettingState>();
        }
    }
}