using System;
using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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

        public event Action<Card, bool> HoverEvent = delegate { };

        // States
        public bool isDragging { get; private set; }


        public bool isHovering { get; private set; }

        public bool selected { get; private set; }

        /// <summary>
        /// 是否可以自己回到原来的位置
        /// </summary>
        [NonSerialized] public bool canReset = true;

        // Config
        public Vector3 offset;
        public float moveSpeedLimit = 20f;

        public float selectionOffset = 50;
        public float biggerScale = 2f;

        // components
        public CardVisual visual { get; private set; }
        private CardVisualPool _cardVisualPool;
        public Image img { get; private set; }
        private Canvas _canvas;


        public ActiveSkillData data;

        public async void Init(CardVisualPool cardVisualPool, ActiveSkillData data, bool directSetPos = false)
        {
            _cardVisualPool = cardVisualPool;
            this._cardVisualPool = cardVisualPool;
            // Debug.Log($"Init Card: HashCode: {this.data.GetHashCode()}, data: {data}");
            img = GetComponent<Image>();
            _canvas = GetComponentInParent<Canvas>();
            if (visual != null)
            {
                cardVisualPool.Release(this.data.id, visual);
                visual = null;
            }

            this.data = data;


            visual = cardVisualPool.Get(data.id);


            visual.transform.localScale = Vector3.one;
            visual.transform.localPosition = Vector3.zero;
            if (directSetPos)
            {
                visual.transform.position = transform.position;
            }

            visual.Initialize(this);
            img.sprite = await Global.Get<ResourceSystem>().LoadCardBg(data.id);
        }

        public void OnGet()
        {
            gameObject.SetActive(true);
        }

        public void OnRelease()
        {
            transform.localScale = Vector3.one;
            gameObject.SetActive(false);
            // Reste Events
            PointerEnterEvent = delegate { };
            PointerExitEvent = delegate { };
            PointerUpEvent = delegate { };
            PointerDownEvent = delegate { };
            BeginDragEvent = delegate { };
            EndDragEvent = delegate { };
            SelectEvent = delegate { };
            // Reset States
            isDragging = false;
            isHovering = false;
            selected = false;

            if (visual != null)
            {
                _cardVisualPool.Release(data.id, visual);
            }

            visual = null;

            data = null;
        }

        private void Update()
        {
            if (!Application.isPlaying) return;
            if (isDragging)
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector2 targetPosition = UIRoot.Singleton.UICamera.ScreenToWorldPoint(mousePosition) - offset;
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                Vector2 velocity = direction * Mathf.Min(moveSpeedLimit,
                    Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);
                transform.Translate(velocity * Time.deltaTime);
                ClampPosition(); // 限制位置 不能超出屏幕
            }
            else if (canReset)
            {
                RectTransform rectTransform = transform as RectTransform;
                rectTransform.anchoredPosition = Vector2.zero;
            }

            if (isDragging || isHovering)
            {
                transform.localScale = Vector3.one * biggerScale;
            }
            else if (canReset)
            {
                transform.localScale = Vector3.one;
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


        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            // Debug.Log("OnBeginDrag");
            transform.DOScale(Vector3.one * biggerScale, 0.1f);
            BeginDragEvent(this);
            Vector2 mousePosition = UIRoot.Singleton.UICamera.ScreenToWorldPoint(eventData.position);
            offset = mousePosition - (Vector2)transform.position;
            isDragging = true;
            // Debug.Log("OnBeginDrag, isDragging: " + isDragging);
            _canvas.GetComponent<GraphicRaycaster>().enabled = false;
            img.raycastTarget = false;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
        }

        public virtual async void OnEndDrag(PointerEventData eventData)
        {
            EndDragEvent.Invoke(this);
            isDragging = false;
            // Debug.Log("OnEndDrag, isDragging: " + isDragging);
            _canvas.GetComponent<GraphicRaycaster>().enabled = true;
            img.raycastTarget = true;
            await UniTask.Yield();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            PointerEnterEvent.Invoke(this);
            isHovering = true;

            Global.Event.Send<OnBattleCardHover>(new OnBattleCardHover(this, isHovering));
            HoverEvent.Invoke(this, isHovering);
            Global.Get<AudioSystem>().PlayOneShot(FMODName.Event.SFX_SFX_UI_进入卡牌);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            PointerExitEvent.Invoke(this);
            isHovering = false;
            Global.Event.Send<OnBattleCardHover>(new OnBattleCardHover(this, isHovering));
            HoverEvent.Invoke(this, isHovering);
        }


        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            PointerDownEvent.Invoke(this);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            PointerUpEvent.Invoke(this, selected);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            // TODO 感觉不好听 不如不要
            // Global.Get<AudioSystem>().PlayOneShot(FMODName.Event.SFX_SFX_UI_选择卡牌);
            selected = true;
            SelectEvent.Invoke(this, selected);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            selected = false;

            SelectEvent.Invoke(this, selected);
            transform.DOKill();
        }


        public int SlotAmount()
        {
            if (transform.parent.TryGetComponent(out CardSlot slot))
            {
                return slot.transform.parent.childCount - 1;
            }

            return 0;
        }

        public int SlotIndex()
        {
            if (transform.parent.TryGetComponent(out CardSlot slot))
            {
                return slot.transform.GetSiblingIndex();
            }

            return 0;
        }

        public float NormalizedPosition()
        {
            if (transform.TryGetComponent(out CardSlot slot))
            {
                return CardMathExtensions.Remap(SlotIndex(), 0, SlotAmount(), 0, 1);
            }

            return 0;
        }
    }
}