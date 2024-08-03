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
        [SerializeField] private Button debugToStartButton;

        private void Awake()
        {
            rollToStartButton.onClick.AddListener(OnRollToStartButtonClick);
            debugToStartButton.onClick.AddListener(OnDebugToStartButtonClick);
        }

        private void OnDebugToStartButtonClick()
        {
            GameBattleMgr.Singleton.DefaultStart();
            CloseSelf();
        }

        private void OnRollToStartButtonClick()
        {
            GameBattleMgr.Singleton.RollToStart();
            CloseSelf();
        }
    }
}