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

        private void OnEnterGameClick()
        {
            Global.Get<GameFlow>().ToGameScene();
        }
    }
}