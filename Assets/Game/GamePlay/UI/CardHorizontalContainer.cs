using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}