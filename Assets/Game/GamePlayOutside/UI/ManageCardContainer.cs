using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class ManageCardContainer : MonoBehaviour
    {
        public List<Card> CardList;
        public RectTransform rectTransform { get; private set; }

        [field: SerializeField] public EasyGameObjectPool cardSlotPool { get; private set; }
        [field: SerializeField] public CardVisualPool cardVisualPool { get; private set; }
        [field: SerializeField] public EasyGameObjectPool cardPool { get; private set; }

        [SerializeField] public float standardWidth = 100;
        [SerializeField] public float standardHeight = 150;
        [SerializeField] public int standardCount = 8;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            cardPool.Initialize();
            cardSlotPool.Initialize();
            cardVisualPool.Initialize();

            CardList = new List<Card>();
        }

        public void Refresh()
        {
            for(int i=0;i<CardList.Count;++i)
            {
                cardSlotPool.Release(CardList[i].transform.parent.gameObject);
                cardPool.Release(CardList[i].gameObject);
            }
            cardPool.Initialize();
            cardSlotPool.Initialize();
            cardVisualPool.Initialize();
            CardList.Clear();
        }

        public void DrawCardToHand(List<ActiveSkillData> dataList)
        {
            Refresh();
            for (int i = 0; i < dataList.Count; i++)
            {
                ActiveSkillData data = dataList[i];
                DrawOneCardToHand(data, i.ToString());
            }
            float maxWidth = standardWidth * standardCount;
            float width = standardWidth * CardList.Count;
            if (width < maxWidth)
            {
                rectTransform.sizeDelta = new Vector2(width, standardHeight);
            }
            else
            {
                rectTransform.sizeDelta = new Vector2(maxWidth, standardHeight);
            }
        }

        public CardItem DrawOneCardToHand(ActiveSkillData data, string name = "")
        {
            CardItem card = SpawnOneCardObj(name);
            CardList.Add(card);
            card.Init(cardVisualPool, data);
            return card;
        }

        private CardItem SpawnOneCardObj(string objName = "")
        {
            CardSlot slot = cardSlotPool.Get().GetComponent<CardSlot>();
            slot.transform.SetParent(transform);

            CardItem card = cardPool.Get().GetComponent<CardItem>();
            card.transform.SetParent(slot.transform);
            card.transform.localPosition = Vector3.zero;

            card.name = objName;

            return card;
        }
    }
}
