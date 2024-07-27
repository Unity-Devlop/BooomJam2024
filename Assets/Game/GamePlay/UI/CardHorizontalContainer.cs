using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class CardHorizontalContainer : MonoBehaviour
    {
        public int childCnt = 8;
        [SerializeField] private List<ActiveSkillData> cardDataList;

        [SerializeField] private List<Card> cardList;

        //把卡牌拖到这个区域内 -> 出牌
        [SerializeField] private RectTransform outsideArea;
        [SerializeField] private bool tweenCardReturn = true;
        public RectTransform rectTransform { get; private set; }

        // STATES
        [ReadOnly] public Card hoveringCard { get; private set; }
        [ReadOnly] public Card selectedCard { get; private set; }


        [field: SerializeField] public RectTransform visualRoot { get; private set; }
        [field: SerializeField] public EasyGameObjectPool cardSlotPool { get; private set; }
        [field: SerializeField] public EasyGameObjectPool cardVisualPool { get; private set; }
        [field: SerializeField] public EasyGameObjectPool cardPool { get; private set; }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            cardPool.Initialize();
            cardSlotPool.Initialize();
            cardVisualPool.Initialize();

            //TODO DEBUG 用
            Spawn(childCnt);
        }

        private async void Spawn(int cnt)
        {
            cardList = new List<Card>();
            cardDataList = new List<ActiveSkillData>();
            cardList = transform.GetComponentsInChildren<Card>().ToList();
            for (int i = 0; i < cnt; i++)
            {
                CardSlot slot = cardSlotPool.Get().GetComponent<CardSlot>();
                slot.transform.SetParent(transform);
                // slot.transform.localPosition = Vector3.zero;

                Card card = cardPool.Get().GetComponent<Card>();
                card.transform.SetParent(slot.transform);
                card.transform.localPosition = Vector3.zero;

                cardList.Add(card);
                cardDataList.Add(new ActiveSkillData()); // TODO DBEUG 用 数据应该传进来用
                card.Init(this);

                card.PointerEnterEvent += CardPointerEnter;
                card.PointerExitEvent += CardPointerExit;
                card.BeginDragEvent += BeginDrag;
                card.EndDragEvent += EndDrag;

                card.name = i.ToString();

                await UniTask.Yield();
            }
        }


        private void BeginDrag(Card card)
        {
            selectedCard = card;
        }

        private Tweener _endDragTween;

        void EndDrag(Card card)
        {
            if (selectedCard == null)
                return;
            if (_endDragTween != null)
            {
                _endDragTween.Complete();
                _endDragTween.Kill();
                _endDragTween = null;
            }

            Vector3 selectPos = new Vector3(0, card.selectionOffset, 0);
            Vector3 zeroPos = Vector3.zero;
            _endDragTween = card.transform.DOLocalMove(card.selected ? selectPos : zeroPos, tweenCardReturn ? .15f : 0)
                .SetEase(Ease.OutBack)
                .OnComplete(() => { card.transform.localPosition = card.selected ? selectPos : zeroPos; });

            selectedCard = null;
        }

        private void CardPointerExit(Card obj)
        {
            hoveringCard = null;
        }

        private void CardPointerEnter(Card obj)
        {
            hoveringCard = obj;
        }

        private void Bind()
        {
        }

        private void UnBind()
        {
        }
    }
}