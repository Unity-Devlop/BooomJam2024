using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class PokemonHUD : MonoBehaviour
    {
        [SerializeField] private Image BG;
        [SerializeField] private LoopHorizontalScrollRect ownedCardList;
        [SerializeField] private EasyGameObjectPool fixedCardPool;

        HuluData _data;

        private void Awake()
        {
            ownedCardList.itemRenderer = ItemRenderer;
            ownedCardList.ItemProvider = ItemProvider;
            ownedCardList.ItemReturn = ItemReturn;
        }

        private void ItemReturn(Transform transform1)
        {
            fixedCardPool.Release(transform1.gameObject);
        }

        private GameObject ItemProvider(int idx)
        {
            GameObject obj =  fixedCardPool.Get();
            obj.name = idx.ToString();
            return obj;
        }

        private void ItemRenderer(Transform transform1, int idx)
        {
            ActiveSkillData skillData = _data.ownedSkills[idx];
            FixedCard card = transform1.GetComponentInChildren<FixedCard>();
            card.Init(skillData);
        }


        public void UnBind()
        {
            ownedCardList.totalCount = 0;
            ownedCardList.RefillCells();
            ownedCardList.RefreshCells();
        }


        public void Bind(HuluData data)
        {
            _data = data;

            ownedCardList.totalCount = data.ownedSkills.Count;
            ownedCardList.RefillCells();
            ownedCardList.RefreshCells();
        }
    }
}