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
        [SerializeField] private TextMeshPro elementText;
        [SerializeField] private TextMeshPro statusText;

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
            elementText.text = obj.elementEnum.ToString();
            statusText.text = $"Atk:{obj.atk}\nDef:{obj.def}\nSpeed:{obj.speed}\nAdap:{obj.adap}";
        }

        private async UniTask OnData(HuluData obj)
        {
            nameText.text = obj.name;

            elementText.text = obj.elementEnum.ToString();

            statusText.text = $"Atk:{obj.atk}\nDef:{obj.def}\nSpeed:{obj.speed}\nAdap:{obj.adap}";

            int origin = hpText.text == "" ? 0 : int.Parse(hpText.text.Split('/')[0]);
            int delta = obj.currentHp - origin;
            // 10滴血一帧
            int cnt = Mathf.Abs(delta) / 10;
            for (int i = 0; i < cnt; i++)
            {
                hpText.text = $"{origin + delta * i / 10}/{obj.hp}";
                await UniTask.Delay(TimeSpan.FromMilliseconds(1 / 60f * 1000));
            }

            OnDataDirect(obj);
        }

        public void UnBind()
        {
            if (_unbindCmd == null) return;
            _unbindCmd.Execute();
            _data = null;
        }
    }
}