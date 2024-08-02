using System;
using Game.GamePlay;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class GameDebugPanel : UIPanel
    {
        [SerializeField] private Button rollToStartButton;

        private void Awake()
        {
            rollToStartButton.onClick.AddListener(OnRollToStartButtonClick);
        }

        private void OnRollToStartButtonClick()
        {
            GameBattleMgr.Singleton.RollToStart();
            CloseSelf();
        }
    }
}