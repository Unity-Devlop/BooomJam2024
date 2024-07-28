using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game.GamePlay
{
    public class Hulu : MonoBehaviour
    {
        [ReadOnly, ShowInInspector] private HuluData _data;
        public TextMeshPro hpText;
        private ICommand _unbindCmd;

        private void Awake()
        {
            hpText = GetComponentInChildren<TextMeshPro>();
        }

        public void Bind(HuluData data)
        {
            _data = data;
            _unbindCmd = _data.bind.Listen(OnData);
            OnData(data);
        }

        private void OnData(HuluData obj)
        {
            hpText.text = $"{obj.currentHp}/{obj.hp}";
        }

        public void UnBind()
        {
            if (_unbindCmd == null) return;
            _unbindCmd.Execute();
            _data = null;
        }
    }
}