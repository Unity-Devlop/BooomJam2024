using System;
using System.Collections.Generic;
using System.Linq;
using cfg;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class ElementCounterView : MonoBehaviour, IUIView, IPointerClickHandler
    {
        [SerializeField] private Button showButton;
        [SerializeField] private RectTransform BG;
        [SerializeField] private RectTransform counterContainer;
        [ReadOnly, ShowInInspector] private List<Image> _counterIconList;

        public Sprite normal;
        public Sprite counter;
        public Sprite defense;
        public Sprite nonEffect;

        private void Awake()
        {
            showButton.onClick.AddListener(ShowButtonClick);
            _counterIconList = counterContainer.GetComponentsInChildren<Image>(true).ToList();
            _counterIconList.RemoveAt(0);
            int elementCount = Global.Table.ElementFitTable.DataList.Count;
            Assert.IsTrue(_counterIconList.Count == Global.Table.ElementFitTable.DataList.Count *
                Global.Table.ElementFitTable.DataList.Count);

            for (int atk = 0; atk < elementCount; atk++)
            {
                ElementEnum atkElement = Global.Table.ElementFitTable.DataList[atk].Id;
                for (int def = 0; def < elementCount; def++)
                {
                    ElementEnum defElement = Global.Table.ElementFitTable.DataList[def].Id;
                    float value = GameMath.CalElementFit(atkElement, defElement);
                    Image targetIcon = _counterIconList[atk * elementCount + def];
                    if (value == 0)
                    {
                        targetIcon.sprite = nonEffect;
                    }
                    else if (Mathf.Approximately(value, 1))
                    {
                        targetIcon.sprite = normal;
                    }
                    else if (Mathf.Approximately(value, 0.5f))
                    {
                        targetIcon.sprite = defense;
                    }
                    else if (value > 1)
                    {
                        targetIcon.sprite = counter;
                    }
                }
            }

            HideContent();
        }

        private void ShowButtonClick()
        {
            BG.gameObject.SetActive(!BG.gameObject.activeInHierarchy);
            counterContainer.gameObject.SetActive(!counterContainer.gameObject.activeInHierarchy);
        }

        public bool IsOpen()
        {
            return gameObject.activeInHierarchy;
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HideContent();
        }

        private void HideContent()
        {
            BG.gameObject.SetActive(false);
            counterContainer.gameObject.SetActive(false);
        }
    }
}