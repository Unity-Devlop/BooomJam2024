using System;
using Cysharp.Threading.Tasks;
using Game.Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    [RequireComponent(typeof(Image))]
    public class Card : Selectable, IDragHandler, IBeginDragHandler, IEndDragHandler, IPoolObject
    {
        // Events
        public event Action<Card> PointerEnterEvent = delegate { };
        public event Action<Card> PointerExitEvent = delegate { };
        public event Action<Card, bool> PointerUpEvent = delegate { };
        public event Action<Card> PointerDownEvent = delegate { };
        public event Action<Card> BeginDragEvent = delegate { };
        public event Action<Card> EndDragEvent = delegate { };

        public event Action<Card, bool> SelectEvent = delegate { };

        // States
        [ReadOnly] public bool isDragging { get; private set; }

        [ReadOnly] public bool wasDragged { get; private set; }

        [ReadOnly] public bool isHovering { get; private set; }

        [ReadOnly] public bool selected { get; private set; }

        // Config
        public Vector3 offset;
        public float moveSpeedLimit = 20f;
        public float selectionOffset = 50;

        // components
        private CardVisual _visual;
        private CardHorizontalContainer _container;
        public Image img { get; private set; }
        private Canvas _canvas;


        public ActiveSkillData data;

        public void Init(CardHorizontalContainer container, ActiveSkillData data)
        {
            this.data = data;
            // Debug.Log($"Init Card: HashCode: {this.data.GetHashCode()}, data: {data}");
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

        public void OnGet()
        {
            gameObject.SetActive(true);
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
            // Reste Events
            PointerEnterEvent = delegate { };
            PointerExitEvent = delegate { };
            PointerUpEvent = delegate { };
            PointerDownEvent = delegate { };
            BeginDragEvent = delegate { };
            EndDragEvent = delegate { };
            SelectEvent = delegate { };

            if (_visual != null)
            {
                _container.cardVisualPool.Release(_visual.gameObject);
            }
            _visual = null;

            data = null;
        }

        private void Update()
        {
            if (!Application.isPlaying) return;
            ClampPosition(); // 限制位置 不能超出屏幕
            if (isDragging)
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector2 targetPosition = UIRoot.Singleton.UICamera.ScreenToWorldPoint(mousePosition) - offset;
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
            float z = transform.position.z;
            transform.position = new Vector3(clampedPosition.x, clampedPosition.y, z);
            // Debug.Log("ClampPosition, transform.position: " + transform.position);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            BeginDragEvent(this);
            Vector2 mousePosition = UIRoot.Singleton.UICamera.ScreenToWorldPoint(eventData.position);
            offset = mousePosition - (Vector2)transform.position;
            isDragging = true;
            // Debug.Log("OnBeginDrag, isDragging: " + isDragging);
            _canvas.GetComponent<GraphicRaycaster>().enabled = false;
            img.raycastTarget = false;

            wasDragged = true;
        }

        public async void OnEndDrag(PointerEventData eventData)
        {
            EndDragEvent.Invoke(this);
            isDragging = false;
            // Debug.Log("OnEndDrag, isDragging: " + isDragging);
            _canvas.GetComponent<GraphicRaycaster>().enabled = true;
            img.raycastTarget = true;
            await UniTask.Yield();
            wasDragged = false;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            PointerEnterEvent.Invoke(this);
            isHovering = true;
            Global.Get<AudioSystem>().Play(FMODName.Event.ui_进入卡牌);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            PointerExitEvent.Invoke(this);
            isHovering = false;
        }


        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            PointerDownEvent.Invoke(this);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            Global.Get<AudioSystem>().Play(FMODName.Event.ui_选择牌);
            selected = true;
            transform.localPosition += (_visual.transform.up * selectionOffset);
            SelectEvent.Invoke(this, selected);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            selected = false;

            transform.localPosition = Vector3.zero;
            SelectEvent.Invoke(this, selected);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            PointerUpEvent.Invoke(this, selected);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}