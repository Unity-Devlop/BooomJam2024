using System;
using System.Collections.Generic;
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
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform counterContainer;
        // [SerializeField] private Image[] counterIconList;
        // [SerializeField] private GameObject prefab;
        private void Awake()
        {
            showButton.onClick.AddListener(ShowButtonClick);
            // counterIconList = counterContainer.GetComponentsInChildren<Image>();
            // int elementCount = Global.Table.ElementFitTable.DataList.Count;
            // Assert.IsTrue(counterIconList.Length == Global.Table.ElementFitTable.DataList.Count *
            //     Global.Table.ElementFitTable.DataList.Count);

            // for (int atk = 0; atk < elementCount; atk++)
            // {
            //     for (int def = 0; def < elementCount; def++)
            //     {
            //         
            //     }
            // }
            
            HideContent();
        }

        private void ShowButtonClick()
        {
            content.gameObject.SetActive(!content.gameObject.activeInHierarchy);
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
            content.gameObject.SetActive(false);
        }
    }
}