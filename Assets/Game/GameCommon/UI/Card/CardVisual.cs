using System;
using cfg;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class CardVisual : MonoBehaviour
    {
        private Card _target;


        private float _curveYOffset;
        private float _curveRotationOffset;

        [Header("Follow Parameters")] [SerializeField]
        private float followSpeed = 30;

        [Header("Rotation Parameters")] [SerializeField]
        private float rotationAmount = 20;

        [SerializeField] private float rotationSpeed = 20;
        // [SerializeField] private float autoTiltAmount = 30;
        // [SerializeField] private float manualTiltAmount = 20;
        // [SerializeField] private float tiltSpeed = 20;

        // [SerializeField] private CurveParameters curve;


        private Canvas _canvas;
        // private Canvas _shadowCanvas;

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image icon;
        public ActiveSkillEnum id { get; private set; }

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        public void Initialize(Card card)
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
            id = card.data.id;
            card.PointerEnterEvent += PointerEnter;
            card.PointerExitEvent += PointerExit;
            card.BeginDragEvent += BeginDrag;
            card.EndDragEvent += EndDrag;
            card.PointerDownEvent += PointerDown;
            card.PointerUpEvent += PointerUp;
            card.SelectEvent += Select;
            card.HoverEvent += Hover;

            nameText.text = card.data.config.Id.ToString();
            if (card.data.config.Type == ActiveSkillTypeEnum.指挥)
            {
                icon.color = Color.yellow;
            }
            else
            {
                icon.color = Color.cyan;
            }
        }

        private void Hover(Card card, bool hovering)
        {
        }

        private void Select(Card card, bool state)
        {
            // DOTween.Kill(2, true);
            // float dir = state ? 1 : 0;
            // shakeParent.DOPunchPosition(shakeParent.up * selectPunchAmount * dir, scaleTransition, 10, 1);
            // shakeParent.DOPunchRotation(Vector3.forward * (hoverPunchAngle / 2), hoverTransition, 20, 1).SetId(2);
            //
            // if (scaleAnimations)
            //     transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
        }

        private void BeginDrag(Card card)
        {
            // if (scaleAnimations)
            // transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

            _canvas.overrideSorting = true;
        }

        private void EndDrag(Card card)
        {
            _canvas.overrideSorting = false;
            // transform.DOScale(1, scaleTransition).SetEase(scaleEase);
        }

        private void PointerEnter(Card card)
        {
            // if (scaleAnimations)
            //     transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
            //
            // DOTween.Kill(2, true);
            // shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
        }

        private void PointerExit(Card card)
        {
            // if (!card.wasDragged)
            // transform.DOScale(1, scaleTransition).SetEase(scaleEase);
        }

        private void PointerUp(Card card, bool longPress)
        {
            // if (scaleAnimations)
            // transform.DOScale(longPress ? scaleOnHover : scaleOnSelect, scaleTransition).SetEase(scaleEase);
            _canvas.overrideSorting = false;

            // visualShadow.localPosition = shadowDistance;
            // _shadowCanvas.overrideSorting = true;
        }

        private void PointerDown(Card card)
        {
            // if (scaleAnimations)
            // transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

            // visualShadow.localPosition += (-Vector3.up * shadowOffset);
            // _shadowCanvas.overrideSorting = false;
        }

        private void Update()
        {
            if (_target == null)
            {
                return;
            }

            gameObject.SetActive(_target.gameObject.activeInHierarchy);
            HandPositioning();
            SmoothFollow();
            FollowRotation();
            CardTilt();
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// 随时间自动旋转
        /// </summary>
        private void HandPositioning()
        {
        }

        private void SmoothFollow()
        {
            Vector3 verticalOffset = (Vector3.up * (_target.isDragging ? 0 : _curveYOffset));
            transform.position = Vector3.Lerp(transform.position, _target.transform.position + verticalOffset,
                followSpeed * Time.deltaTime);
        }

        private void FollowRotation()
        {
            Vector3 verticalOffset = (Vector3.up * (_target.isDragging ? 0 : _curveYOffset));
            transform.position = Vector3.Lerp(transform.position, _target.transform.position + verticalOffset,
                followSpeed * Time.deltaTime);
        }

        private void CardTilt()
        {
        }
    }
}