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
        // [SerializeField] private Image bg;
        private ICommand _unbind;

        public void Bind(HuluData data, int index)
        {
            _index = index;
            _unbind = data.bind.Listen(OnData);
            OnData(data);
        }

        private UniTask OnData(HuluData arg)
        {
            nameText.text = arg.name;
            // icon.sprite = arg.icon;
            // bg.color = arg.bgColor;
            return UniTask.CompletedTask;
        }

        public void UnBind()
        {
            _unbind?.Execute();
            _unbind = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent.Invoke(_index);
        }
    }
}