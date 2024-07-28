using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GamePlay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game
{
    public class CardHorizontalContainer : MonoBehaviour
    {
        public int childCnt = 8;

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

        private Tweener _endDragTween;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            cardPool.Initialize();
            cardSlotPool.Initialize();
            cardVisualPool.Initialize();

            cardList = new List<Card>();
        }


        public async UniTask Spawn(List<ActiveSkillData> dataList, float interval = 0.1f)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                ActiveSkillData data = dataList[i];
                PushCard(data, i.ToString());
                await UniTask.Delay(TimeSpan.FromSeconds(interval));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Card PushCard(ActiveSkillData data, string name = "")
        {
            Card card = SpawnOne(name);
            cardList.Add(card);
            card.Init(this, data);
            Debug.Log($"Push Card: HashCode: {data.GetHashCode()}, data: {data}");
            return card;
        }

        public async UniTask Use(ActiveSkillData data)
        {
            var card = cardList.Find(card => card.data == data);

            Assert.IsNotNull(card.data);

            cardSlotPool.Release(card.transform.parent.gameObject);
            cardPool.Release(card.gameObject);
            // Debug.Log($"消耗牌{card.data}");
            // 移除数据
            cardList.Remove(card);
            await UniTask.CompletedTask;
        }

        private Card SpawnOne(string objName = "")
        {
            CardSlot slot = cardSlotPool.Get().GetComponent<CardSlot>();
            slot.transform.SetParent(transform);

            Card card = cardPool.Get().GetComponent<Card>();
            card.transform.SetParent(slot.transform);
            card.transform.localPosition = Vector3.zero;

            card.name = objName;
            card.PointerEnterEvent += CardPointerEnter;
            card.PointerExitEvent += CardPointerExit;
            card.BeginDragEvent += BeginDrag;
            card.EndDragEvent += EndDrag;


            return card;
        }


        private void BeginDrag(Card card)
        {
            selectedCard = card;
        }


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

            // 判断结束拖拽的位置是不是出牌区

            Vector3 screenPoint = UIRoot.Singleton.UICamera.WorldToScreenPoint(selectedCard.transform.position);
            if (RectTransformUtility.RectangleContainsScreenPoint(outsideArea,
                    new Vector2(screenPoint.x, screenPoint.y), UIRoot.Singleton.UICamera)
                && _calCts is { IsCancellationRequested: false })
            {
                // 释放对象
                _trainer.PushOperation(new ActiveSkillBattleOperation()
                {
                    data = selectedCard.data
                });
            }
            else
            {
                Vector3 selectPos = new Vector3(0, card.selectionOffset, 0);
                Vector3 zeroPos = Vector3.zero;
                _endDragTween = card.transform
                    .DOLocalMove(card.selected ? selectPos : zeroPos, tweenCardReturn ? .15f : 0)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() => { card.transform.localPosition = card.selected ? selectPos : zeroPos; });
            }


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

        private IBattleTrainer _trainer;

        public void Bind(IBattleTrainer trainer)
        {
            _trainer = trainer;
        }

        public void UnBind()
        {
        }

        private CancellationTokenSource _calCts;

        public UniTask StartCalOperation()
        {
            Assert.IsNull(_calCts);
            _calCts = new CancellationTokenSource();
            return UniTask.CompletedTask;
        }

        public UniTask EndCalOperation()
        {
            Assert.IsNotNull(_calCts);
            _calCts.Cancel();
            _calCts = null;
            return UniTask.CompletedTask;
        }
    }
}