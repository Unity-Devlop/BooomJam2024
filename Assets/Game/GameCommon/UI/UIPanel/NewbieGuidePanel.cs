using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class NewbieGuidePanel : UIPanel
    {
        [SerializeField] private Button left;
        [SerializeField] private Button right;
        [SerializeField] private Button close;
        [SerializeField] private ScrollRect scrollRect;
        private List<RectTransform> _contents;
        private int _current = 0;

        private bool _dotweening = false;

        private void Awake()
        {
            _contents = new List<RectTransform>(scrollRect.content.childCount);
            for (var i = 0; i < scrollRect.content.childCount; i++)
            {
                _contents.Add(scrollRect.content.GetChild(i).GetComponent<RectTransform>());
            }

            left.onClick.AddListener(OnLeftClick);
            right.onClick.AddListener(OnRightClick);
            close.onClick.AddListener(OnCloseClick);
            // scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        private void OnCloseClick()
        {
            CloseSelf();
        }

        // private void OnScrollValueChanged(Vector2 arg0)
        // {
        //     Global.LogInfo($"NewbieGuidePanel OnScrollValueChanged {scrollRect.horizontalNormalizedPosition}");
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float Index2NormalizedPosition(int index)
        {
            return Mathf.Clamp(index / ((float)_contents.Count - 1), 0, 1);
        }

        public override void OnOpened()
        {
            base.OnOpened();
            ReDirect(0);
        }


        private void ReDirect(int idx)
        {
            if (_dotweening) return;
            int next = Mathf.Clamp(idx, 0, _contents.Count - 1);
            if (_current == next) return;
            _current = next;
            _dotweening = true;
            // scrollRect.GetComponent<CanvasGroup>().interactable = false;
            DOTween.To(() => scrollRect.horizontalNormalizedPosition, x => scrollRect.horizontalNormalizedPosition = x,
                Index2NormalizedPosition(_current), 0.5f).OnComplete(() =>
            {
                _dotweening = false;
                // scrollRect.GetComponent<CanvasGroup>().interactable = true;
            });
        }

        private void OnRightClick()
        {
            ReDirect(_current + 1);
        }

        private void OnLeftClick()
        {
            ReDirect(_current - 1);
        }
    }
}