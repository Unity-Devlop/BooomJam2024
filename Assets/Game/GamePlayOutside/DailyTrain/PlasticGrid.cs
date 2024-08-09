using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class PlasticGrid : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private RectTransform rectTransform;
        private Image image;
        private Vector2 offsetPosition;
        private Vector3 originPos;
        private Canvas canvas;

        public RectTransform _RectTransform => rectTransform;
        [HideInInspector] public int width;
        [HideInInspector] public int heigth;
        public List<int> realSize = new List<int>();
        public TrainContent trainContent;
        public GameObject courseTable;
        public DailyTrainTable trainTable;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            image.alphaHitTestMinimumThreshold = 0.5f;
            heigth = realSize.Count;
            width = realSize.Max();
            originPos = rectTransform.anchoredPosition;
            canvas = GetComponentInParent<Canvas>();
        }

        private void OnEnable()
        {
            if (width < 1) width = 1;
            if (heigth < 1) heigth = 1;
            rectTransform.sizeDelta = new Vector2(width * 100, heigth * 100);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            gameObject.transform.SetParent(trainTable.transform);
            image.raycastTarget = false;
            image.transform.SetAsLastSibling();
            trainTable.RemoveGrid(rectTransform.anchoredPosition, false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (target != null)
            {
                if (target.CompareTag("PlasticGrid"))
                {
                    trainTable.RemoveGrid(target.GetComponent<RectTransform>().anchoredPosition, true);
                    if (!trainTable.AddTrainGrid(this)) ResetPos();
                }
                else if (!target.CompareTag("DailyTrainTable") || !trainTable.AddTrainGrid(this))
                {
                    ResetPos();
                }
            }
            else ResetPos();

            image.raycastTarget = true;
        }

        public void SetSize(Vector2 size)
        {
            width = (int)size.x;
            heigth = (int)size.y;
            if (width < 1) width = 1;
            if (heigth < 1) heigth = 1;
            rectTransform.sizeDelta = new Vector2(width * 100, heigth * 100);
        }

        public void ResetPos()
        {
            gameObject.transform.SetParent(courseTable.transform);
            rectTransform.anchoredPosition = originPos;
        }
    }
}