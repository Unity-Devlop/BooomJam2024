using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    [RequireComponent(typeof(Image))]
    public class Card : Selectable, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        // States
        [ReadOnly] public bool isDragging { get; private set; }

        [ReadOnly] public bool wasDragged { get; private set; }

        // Config
        public Vector3 offset;
        public float moveSpeedLimit = 20f;


        // components
        private CardVisual _visual;
        private CardHorizontalContainer _container;
        public Image img { get; private set; }
        private Canvas _canvas;

        public void Init(CardHorizontalContainer container)
        {
            img = GetComponent<Image>();
            _canvas = GetComponentInParent<Canvas>();
            _container = container;
            if (_visual != null)
            {
                _container.cardVisualPool.Release(_visual.gameObject);
                _visual = null;
            }

            _visual = _container.cardVisualPool.Get().GetComponent<CardVisual>();
            _visual.transform.SetParent(_container.visualRoot);
            _visual.transform.localScale = Vector3.one;
            _visual.transform.localPosition = Vector3.zero;
            _visual.Initialize(this);
        }

        private void Update()
        {
            ClampPosition(); // 限制位置 不能超出屏幕
            if (isDragging)
            {
                Vector2 targetPosition = UIRoot.Singleton.UICamera.ScreenToWorldPoint(Input.mousePosition) - offset;
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                Vector2 velocity = direction * Mathf.Min(moveSpeedLimit,
                    Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
                transform.Translate(velocity * Time.deltaTime);
            }
        }

        private void ClampPosition()
        {
            Vector2 screenBounds =
                UIRoot.Singleton.UICamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
            transform.position = new Vector3(clampedPosition.x, clampedPosition.y, transform.position.z);
            // Debug.Log("ClampPosition, transform.position: " + transform.position);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            // BeginDragEvent.Invoke(this);
            Vector2 mousePosition = UIRoot.Singleton.UICamera.ScreenToWorldPoint(eventData.position);
            offset = mousePosition - (Vector2)transform.position;
            isDragging = true;
            _canvas.GetComponent<GraphicRaycaster>().enabled = false;
            img.raycastTarget = false;

            wasDragged = true;
        }

        public async void OnEndDrag(PointerEventData eventData)
        {
            // EndDragEvent.Invoke(this);
            isDragging = false;
            _canvas.GetComponent<GraphicRaycaster>().enabled = true;
            img.raycastTarget = true;
            await UniTask.Yield();
            wasDragged = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}