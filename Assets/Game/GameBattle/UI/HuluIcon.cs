using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class HuluIcon : Image, IPointerClickHandler
    {
        public event Action<int> OnClick = delegate { };
        private int _idx = -1;
        private HuluData _data;
        private ICommand _unbindCmd;
        private TextMeshProUGUI _nameText;

        protected override void Awake()
        {
            _nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        }

        public void Bind(HuluData data, int idx)
        {
            _idx = idx;
            _data = data;
            _unbindCmd = _data.bind.Listen(OnData);
            OnData(data);
        }

        private UniTask OnData(HuluData obj)
        {
            _nameText.text = obj.name;
            return UniTask.CompletedTask;
        }
        

        public void Unbind()
        {
            _unbindCmd?.Execute();
            _data = null;
            _idx = -1;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick(_idx);
        }
    }
}