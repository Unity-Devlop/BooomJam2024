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
        private TextMeshProUGUI _nameText;
        private ICommand _unbindCommand;

        protected override void Awake()
        {
            _nameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        }

        public void Bind(HuluData data, int idx)
        {
            _idx = idx;
            _data = data;
            _unbindCommand = _data.bind.Listen(OnData);
            OnData(data).Forget();
        }

        private async UniTask OnData(HuluData obj)
        {
            _nameText.text = obj.name;
            sprite = await Global.Get<ResourceSystem>().LoadPortraitBox(obj.id);
        }


        public void Unbind()
        {
            _unbindCommand?.Execute();
            _data = null;
            _idx = -1;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick(_idx);
        }
    }
}