using System;
using cfg;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class CardVisual : MonoBehaviour
    {
        public enum ShaderType
        {
            None,
            Regular,
            Polychrome,
            Negative
        }
        private Card _target;


        private float _curveYOffset;
        private float _curveRotationOffset;
        
        public ShaderType shaderType;

        [Header("Follow Parameters")] [SerializeField]
        private float followSpeed = 30;

        [Header("Scale Parameters")] [SerializeField]
        private float scaleSpeed = 20;

        [Header("Rotation Parameters")] [SerializeField]
        private float rotationAmount = 20;

        [SerializeField] private float rotationSpeed = 20;
        // [SerializeField] private float autoTiltAmount = 30;
        // [SerializeField] private float manualTiltAmount = 20;
        // [SerializeField] private float tiltSpeed = 20;

        // [SerializeField] private CurveParameters curve;


        private Canvas _canvas;
        // private Canvas _shadowCanvas;

        [SerializeField] protected Image background;
        [SerializeField] protected TextMeshProUGUI nameText;

        [SerializeField] private RectTransform shakeContainer;
        [SerializeField] private Transform tiltContainer;
        public ActiveSkillTypeEnum id { get; private set; }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        public virtual async void Initialize(Card card)
        {
            if (_target != null)
            {
                _target.PointerEnterEvent -= PointerEnter;
                _target.PointerExitEvent -= PointerExit;
                _target.BeginDragEvent -= BeginDrag;
                _target.EndDragEvent -= EndDrag;
                _target.PointerDownEvent -= PointerDown;
                _target.PointerUpEvent -= PointerUp;
                _target.SelectEvent -= Select;
                _target.HoverEvent -= Hover;
            }

            _target = card;
            id = card.data.config.Type;
            gameObject.name = card.data.id.ToString();
            card.PointerEnterEvent += PointerEnter;
            card.PointerExitEvent += PointerExit;
            card.BeginDragEvent += BeginDrag;
            card.EndDragEvent += EndDrag;
            card.PointerDownEvent += PointerDown;
            card.PointerUpEvent += PointerUp;
            card.SelectEvent += Select;
            card.HoverEvent += Hover;

            nameText.text = card.data.config.Id.ToString();
        }

        protected virtual void Hover(Card card, bool hovering)
        {
            _canvas.overrideSorting = hovering;
        }

        protected virtual void Select(Card card, bool state)
        {
            // _canvas.overrideSorting = state;
            // DOTween.Kill(2, true);
            // float dir = state ? 1 : 0;
            // shakeParent.DOPunchPosition(shakeParent.up * selectPunchAmount * dir, scaleTransition, 10, 1);
            // shakeParent.DOPunchRotation(Vector3.forward * (hoverPunchAngle / 2), hoverTransition, 20, 1).SetId(2);
            //
            // if (scaleAnimations)
            //     transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
        }

        protected virtual void BeginDrag(Card card)
        {
            // if (scaleAnimations)
            // transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

            _canvas.overrideSorting = true;
        }

        protected virtual void EndDrag(Card card)
        {
            _canvas.overrideSorting = false;
            // transform.DOScale(1, scaleTransition).SetEase(scaleEase);
        }
        [Header("Hober Parameters")]
        [SerializeField] private float hoverPunchAngle = 5;
        [SerializeField] private float hoverTransition = .15f;
        protected virtual void PointerEnter(Card card)
        {
            DOTween.Kill(2, true);
            shakeContainer.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
            // if (scaleAnimations)
            //     transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
            //
            // DOTween.Kill(2, true);
            // shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
        }

        protected virtual void PointerExit(Card card)
        {
            // if (!card.wasDragged)
            // transform.DOScale(1, scaleTransition).SetEase(scaleEase);
        }

        protected virtual void PointerUp(Card card, bool longPress)
        {
            // if (scaleAnimations)
            // transform.DOScale(longPress ? scaleOnHover : scaleOnSelect, scaleTransition).SetEase(scaleEase);
            _canvas.overrideSorting = false;

            // visualShadow.localPosition = shadowDistance;
            // _shadowCanvas.overrideSorting = true;
        }

        protected virtual void PointerDown(Card card)
        {
            // if (scaleAnimations)
            // transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

            // visualShadow.localPosition += (-Vector3.up * shadowOffset);
            // _shadowCanvas.overrideSorting = false;
        }

        protected virtual void Update()
        {
            if (_target == null)
            {
                return;
            }

            if (_target.isDragging)
            {
                _canvas.overrideSorting = true;
            }

            if (_target.isHovering)
            {
                _canvas.overrideSorting = true;
            }

            gameObject.SetActive(_target.gameObject.activeInHierarchy);
            SmoothScale();
            HandPositioning();
            SmoothFollow();
            FollowRotation();
            CardTilt();
        }

        private void SmoothScale()
        {
            Vector3 targetScale = _target.transform.localScale;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);
        }

        public virtual void Dispose()
        {
        }

        [SerializeField] private CardVisualParameters curve;


        /// <summary>
        /// 随时间自动旋转
        /// </summary>
        protected virtual void HandPositioning()
        {
            _curveYOffset = (curve.positioning.Evaluate(_target.NormalizedPosition()) * curve.positioningInfluence) *
                            _target.SlotAmount();
            _curveYOffset = _target.SlotAmount() < 5 ? 0 : _curveYOffset;
            _curveRotationOffset = curve.rotation.Evaluate(_target.NormalizedPosition());
        }

        protected virtual void SmoothFollow()
        {
            Vector3 verticalOffset = (Vector3.up * (_target.isDragging ? 0 : _curveYOffset));
            Vector3 target = _target.transform.position + verticalOffset;

            // // 加上扇形的偏移
            int amount = _target.SlotAmount();
            if (amount != 0)
            {
                float percent = _target.SlotIndex() / (float)amount;
                float angle = percent * 90 - 45;
                float x = Mathf.Sin(angle * Mathf.Deg2Rad);
                x = curve.positioning.Evaluate(percent) * curve.positioningInfluence * x;
                float y = Mathf.Cos(angle * Mathf.Deg2Rad);
                y = curve.positioning.Evaluate(percent) * curve.positioningInfluence * y;
                target += new Vector3(x, y, 0) / amount * curve.positioningRadius;
            }
            else
            {
                // Debug.LogWarning($"{_target}.SlotAmount is 0");
            }

            transform.position = Vector3.Lerp(transform.position, target,
                followSpeed * Time.deltaTime);
        }

        private Vector3 _followRotationMovementDelta;
        private Vector3 _rotationDelta;
        [SerializeField] private float followRotationSpeed = 25;
        [SerializeField] private Vector2 rotationLimits = new Vector2(-60, 60);

        protected virtual void FollowRotation()
        {
            Vector3 movement = (transform.position - _target.transform.position);
            _followRotationMovementDelta = Vector3.Lerp(_followRotationMovementDelta, movement,
                followRotationSpeed * Time.deltaTime);
            Vector3 movementRotation = (_target.isDragging ? _followRotationMovementDelta : movement) * rotationAmount;
            _rotationDelta = Vector3.Lerp(_rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y,
                Mathf.Clamp(_rotationDelta.x, rotationLimits.x, rotationLimits.y));
        }

        [SerializeField] private float autoTiltAmount = 30;
        [SerializeField] private float manualTiltAmount = 20;
        [SerializeField] private float tiltSpeed = 20;

        private int _savedIndex;

        protected virtual void CardTilt()
        {
            _savedIndex = _target.isDragging ? _savedIndex : _target.SlotIndex();
            float sine = Mathf.Sin(Time.time + _savedIndex) * (_target.isHovering ? .2f : 1);
            float cosine = Mathf.Cos(Time.time + _savedIndex) * (_target.isHovering ? .2f : 1);

            Vector3 offset = transform.position - Global.Singleton.mainCamera.ScreenToWorldPoint(Input.mousePosition);
            float tiltX = _target.isHovering ? offset.y * -1 * manualTiltAmount : 0;
            float tiltY = _target.isHovering ? offset.x * manualTiltAmount : 0;

            if (offset.y < 0)
            {
                tiltX = -tiltX;
                // tiltY = -tiltY;
            }


            float tiltZ = _target.isDragging
                ? tiltContainer.eulerAngles.z
                : (_curveRotationOffset * (curve.rotationInfluence * _target.SlotAmount()));

            float lerpX = Mathf.LerpAngle(tiltContainer.eulerAngles.x, tiltX + (sine * autoTiltAmount),
                tiltSpeed * Time.deltaTime);
            float lerpY = Mathf.LerpAngle(tiltContainer.eulerAngles.y, tiltY + (cosine * autoTiltAmount),
                tiltSpeed * Time.deltaTime);
            float lerpZ = Mathf.LerpAngle(tiltContainer.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.deltaTime);

            tiltContainer.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
        }
    }
}