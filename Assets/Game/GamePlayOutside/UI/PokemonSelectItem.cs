using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class PokemonSelectItem : Selectable, IPointerClickHandler
    {
        public event Action<int> OnClickEvent = delegate { };
        private int _index;

        [SerializeField] private TextMeshProUGUI nameText;

        // [SerializeField] private Image icon;
        [SerializeField] public Image bg;
        private ICommand _unbind;

        public void Bind(HuluData data, int index)
        {
            _index = index;
            _unbind = data.bind.Listen(OnData);
            OnData(data);
        }

        private UniTask OnData(HuluData data)
        {
            nameText.text = data.name;
            LoadSprite(data);
            // icon.sprite = arg.icon;
            return UniTask.CompletedTask;
        }

        public void UnBind()
        {
            _unbind?.Execute();
            _unbind = null;
        }

        private async void LoadSprite(HuluData data)
        {
            bg.sprite = await Global.Get<ResourceSystem>().LoadElementTag(data.elementEnum);
        }

        public void SetState(Color color)
        {
            bg.color = color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent.Invoke(_index);
        }
    }
}