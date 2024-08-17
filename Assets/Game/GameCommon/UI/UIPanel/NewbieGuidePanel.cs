using System;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class NewbieGuidePanel : UIPanel
    {
        [SerializeField] private Button left;
        [SerializeField] private Button right;

        private void Awake()
        {
            left.onClick.AddListener(OnLeftClick);
            right.onClick.AddListener(OnRightClick);
        }

        private void Next()
        {
            
        }

        private void Prev()
        {
            
        }
        
        private void OnRightClick()
        {
            
        }

        private void OnLeftClick()
        {
            
        }
    }
}