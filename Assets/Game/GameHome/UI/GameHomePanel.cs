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
            await Global.Get<GameFlow>().ToGameOutside();
        }
    }
}