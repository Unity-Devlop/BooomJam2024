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

        private async void Awake()
        {
            cardPool.Initialize();
            cardSlotPool.Initialize();
            cardVisualPool.Initialize();

            cardList = transform.GetComponentsInChildren<Card>().ToList();

            foreach (var card in cardList)
            {
                card.transform.localPosition = Vector3.zero;
                card.Init(this);
                await UniTask.Yield();
            }


            for (int i = 0; i < childCnt; i++)
            {
                // TODO 生成
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