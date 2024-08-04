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
        public enum State
        {
            Empty,
            ContainsItem,
        }

        public event Action<int> OnClickEvent = delegate { };
        private int _index;

        [SerializeField] private TextMeshProUGUI nameText;

        // [SerializeField] private Image icon;
        [SerializeField] private Image bg;
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
            // icon.sprite = arg.icon;
            bg.color = Color.white;
            SetState(State.ContainsItem);
            return UniTask.CompletedTask;
        }

        public void UnBind()
        {
            SetState(State.Empty);
            _unbind?.Execute();
            _unbind = null;
        }

        public void SetState(State state)
        {
            switch (state)
            {
                case State.Empty:
                    // TODO 置灰
                    bg.color = Color.gray;
                    break;
                case State.ContainsItem:
                    bg.color = Color.white;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickEvent.Invoke(_index);
        }
    }
}