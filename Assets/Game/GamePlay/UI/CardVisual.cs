using System;
using UnityEngine;

namespace Game
{
    public class CardVisual : MonoBehaviour
    {
        private Card _target;
        
        
        private float _curveYOffset;
        private float _curveRotationOffset;
        
        [Header("Follow Parameters")]
        [SerializeField] private float followSpeed = 30;

        [Header("Rotation Parameters")]
        [SerializeField] private float rotationAmount = 20;
        [SerializeField] private float rotationSpeed = 20;
        // [SerializeField] private float autoTiltAmount = 30;
        // [SerializeField] private float manualTiltAmount = 20;
        // [SerializeField] private float tiltSpeed = 20;
        
        // [SerializeField] private CurveParameters curve;
        public void Initialize(Card card)
        {
            _target = card;
        }

        private void Update()
        {
            if (_target == null) return;
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
            transform.position = Vector3.Lerp(transform.position, _target.transform.position + verticalOffset, followSpeed * Time.deltaTime);
        }

        private void FollowRotation()
        {
            Vector3 verticalOffset = (Vector3.up * (_target.isDragging ? 0 : _curveYOffset));
            transform.position = Vector3.Lerp(transform.position, _target.transform.position + verticalOffset, followSpeed * Time.deltaTime);
        }

        private void CardTilt()
        {
        }
    }
}