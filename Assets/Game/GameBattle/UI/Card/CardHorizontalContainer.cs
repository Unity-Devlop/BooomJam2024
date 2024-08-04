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
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game
{
    public class CardHorizontalContainer : MonoBehaviour
    {
        public int childCnt = 8;

        [SerializeField] private List<Card> handZoneCardList; // 手牌区域
        // private List<ActiveSkillData> _cemeteryZoneCardList; // 墓地区域
        // private List<ActiveSkillData> _discardZoneCardList; // 弃牌区域
        // private List<ActiveSkillData> _drawZoneCardList; // 抽牌区域

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

            handZoneCardList = new List<Card>();

            // _discardZoneCardList = new List<ActiveSkillData>();
            // _drawZoneCardList = new List<ActiveSkillData>();
            // _cemeteryZoneCardList = new List<ActiveSkillData>();
        }


        public async UniTask DrawCardToHand(List<ActiveSkillData> dataList, float interval = 0.1f)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                ActiveSkillData data = dataList[i];
                SpawnOneToHand(data, i.ToString());
                await UniTask.Delay(TimeSpan.FromSeconds(interval));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Card SpawnOneToHand(ActiveSkillData data, string name = "")
        {
            Card card = SpawnOneCardObj(name);
            handZoneCardList.Add(card);
            card.Init(cardVisualPool, visualRoot, data);
            // Debug.Log($"Push Card: HashCode: {data.GetHashCode()}, data: {data}");
            return card;
        }

        public async UniTask UseFromHand(ActiveSkillData data)
        {
            // var card = handZoneCardList.Find(card => card.data == data);
            //
            // Assert.IsNotNull(card.data);
            //
            // cardSlotPool.Release(card.transform.parent.gameObject);
            // cardPool.Release(card.gameObject);
            // // Debug.Log($"消耗牌{card.data}");
            // // 移除数据
            // handZoneCardList.Remove(card);
            Debug.Log($"从手牌区使用{data}");
            await UniTask.CompletedTask;
        }


        public async UniTask DestroyCard(List<ActiveSkillData> activeSkillDatas)
        {
            foreach (var skillData in activeSkillDatas)
            {
                // TODO 性能问题
                var card = handZoneCardList.Find(card => card.data == skillData);
                if (card == null)
                {
                    // _drawZoneCardList.Remove(skillData);
                    // _discardZoneCardList.Remove(skillData);
                    // _cemeteryZoneCardList.Remove(skillData);
                    Debug.LogWarning($"移除的牌不在手牌里: {skillData}");
                    continue;
                }


                cardSlotPool.Release(card.transform.parent.gameObject);
                cardPool.Release(card.gameObject);
                handZoneCardList.Remove(card);
                await UniTask.DelayFrame(1);
            }
        }

        public async UniTask DiscardToDraw(List<ActiveSkillData> discard, List<ActiveSkillData> draw)
        {
            // _discardZoneCardList.Clear();
            // _discardZoneCardList.AddRange(discard);
            // _drawZoneCardList.Clear();
            // _drawZoneCardList.AddRange(draw);
            await UniTask.CompletedTask;
        }

        public async UniTask ConsumedCard(List<ActiveSkillData> activeSkillDatas)
        {
            foreach (var skillData in activeSkillDatas)
            {
                // TODO 暂时不做消耗牌的表现 直接移除
                var card = handZoneCardList.Find(card => card.data == skillData);
                if (card == null)
                {
                    Debug.LogWarning($"消耗的牌不在手牌里: {skillData} 可能是打出了消耗牌");
                    continue;
                }

                cardSlotPool.Release(card.transform.parent.gameObject);
                cardPool.Release(card.gameObject);
                handZoneCardList.Remove(card);
                // await UniTask.DelayFrame(1);
            }

            await UniTask.CompletedTask;
        }

        public async UniTask Discard(List<ActiveSkillData> activeSkillDatas)
        {
            await DiscardFromHandZone(activeSkillDatas);
            // _drawZoneCardList.AddRange(activeSkillDatas);
        }

        public async UniTask DiscardFromHandZone(List<ActiveSkillData> activeSkillDatas)
        {
            foreach (var skillData in activeSkillDatas)
            {
                // TODO 暂时不做弃牌的表现 直接移除
                var card = handZoneCardList.Find(card => card.data == skillData); // TODO 性能问题 
                Assert.IsNotNull(card);
                cardSlotPool.Release(card.transform.parent.gameObject);
                cardPool.Release(card.gameObject);
                handZoneCardList.Remove(card);
                await UniTask.DelayFrame(1);
            }
        }

        private Card SpawnOneCardObj(string objName = "")
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
                Global.Get<AudioSystem>().PlayOneShot(FMODName.Event.SFX_ui_出牌);
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