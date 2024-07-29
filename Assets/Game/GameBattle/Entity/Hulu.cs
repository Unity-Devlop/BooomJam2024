using System;
using Cysharp.Threading.Tasks;
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
            OnDataDirect(data);
        }

        private void OnDataDirect(HuluData obj)
        {
            nameText.text = obj.name;
            hpText.text = $"{obj.currentHp}/{obj.hp}";
        }

        private async UniTask OnData(HuluData obj)
        {
            nameText.text = obj.name;
            int origin = hpText.text == "" ? 0 : int.Parse(hpText.text.Split('/')[0]);
            int delta = obj.currentHp - origin;
            for (int i = 0; i < 10; i++)
            {
                hpText.text = $"{origin + delta * i / 10}/{obj.hp}";
                await UniTask.Delay(100);
            }

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