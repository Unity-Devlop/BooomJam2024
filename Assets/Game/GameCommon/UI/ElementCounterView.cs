using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class ElementCounterView : MonoBehaviour, IUIView, IPointerClickHandler
    {
        [SerializeField] private Button showButton;
        [SerializeField] private RectTransform content;

        private void Awake()
        {
            showButton.onClick.AddListener(ShowButtonClick);
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