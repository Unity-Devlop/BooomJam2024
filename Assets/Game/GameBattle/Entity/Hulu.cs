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
        [SerializeField] private TextMeshPro hpText;
        [SerializeField] private TextMeshPro nameText;

        private ICommand _unbindCmd;
        
        public void Bind(HuluData data)
        {
            _data = data;
            _unbindCmd = _data.bind.Listen(OnData);
            OnData(data);
        }

        private void OnData(HuluData obj)
        {
            nameText.text = obj.name;
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