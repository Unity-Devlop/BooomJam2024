using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class Card : Selectable, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public GameObject cardVisualPrefab;
        private CardVisual _visual;
        private CardHorizontalContainer _container;
        public bool isDragging { get; private set; }
        public void Init(CardHorizontalContainer container)
        {
            _container = container;
            if (_visual != null)
            {
                _container.cardVisualPool.Release(_visual.gameObject);
            }
            _visual = _container.cardVisualPool.Get().GetComponent<CardVisual>();
            _visual.transform.SetParent(_container.visualRoot);
            _visual.Initialize(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }
    }
}