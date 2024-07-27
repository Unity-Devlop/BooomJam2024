using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
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
        [field: SerializeField] public RectTransform visualRoot { get; private set; }
        [field: SerializeField] public EasyGameObjectPool cardSlotPool { get; private set; }
        [field: SerializeField] public EasyGameObjectPool cardVisualPool { get; private set; }
        [field: SerializeField] public EasyGameObjectPool cardPool { get; private set; }

        private void Awake()
        {
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
                await UniTask.Yield();
            }
        }

        private void Bind()
        {
        }

        private void UnBind()
        {
        }
    }
}