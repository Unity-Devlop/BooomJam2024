using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class NewbieGuidePanel : UIPanel
    {
        [SerializeField] private Button left;
        [SerializeField] private Button right;
        
        [SerializeField] private List<RectTransform> contents;

        private void Awake()
        {
            left.onClick.AddListener(OnLeftClick);
            right.onClick.AddListener(OnRightClick);
        }

        public void DirectTo(Type to)
        {
            // if (to == typeof(FirstSettingState))
        }
        
        private void OnRightClick()
        {
            
        }
        
        private void OnLeftClick()
        {
            
        }


    }
}