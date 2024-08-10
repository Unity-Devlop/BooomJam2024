using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameBattleTip : MonoBehaviour
    {
        [SerializeField] private Image Bg;
        [SerializeField] private TextMeshProUGUI tipText;
        private void Awake()
        {
            Global.Event.Listen<BattleTipEvent>(OnBattleTipEvent);
        }

        private void OnDestroy()
        {
            Global.Event.UnListen<BattleTipEvent>(OnBattleTipEvent);
        }

        private void OnBattleTipEvent(BattleTipEvent obj)
        {
            tipText.text = obj.tip;
            Bg.enabled = true;
        }
    }
}