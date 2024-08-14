using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class PokemonHUD : MonoBehaviour
    {
        [SerializeField] private Image BG;
        [SerializeField] private LoopHorizontalScrollRect ownedCardList;
        [SerializeField] private EasyGameObjectPool fixedCardAndSlotPool;
        [SerializeField] private RectTransform valueUIRoot;
        [SerializeField] private CardVisualParameters visualParameters;
        private ValueUIItem[] _valueUIItems;
        HuluData _data;
        [SerializeField] private CardVisualPool cardVisualPool;

        private void Awake()
        {
            ownedCardList.itemRenderer = ItemRenderer;
            ownedCardList.ItemProvider = ItemProvider;
            ownedCardList.ItemReturn = ItemReturn;

            _valueUIItems = valueUIRoot.GetComponentsInChildren<ValueUIItem>();
        }

        private void ItemReturn(Transform transform1)
        {
            fixedCardAndSlotPool.Release(transform1.gameObject);
        }

        private GameObject ItemProvider(int idx)
        {
            GameObject obj = fixedCardAndSlotPool.Get();
            obj.transform.GetChild(0).name = idx.ToString();
            return obj;
        }

        private async void ItemRenderer(Transform transform1, int idx)
        {
            ActiveSkillData skillData = _data.ownedSkills[idx];
            Assert.IsTrue(transform1.GetComponent<CardSlot>() != null);
            OutSideCard card = transform1.GetComponentInChildren<OutSideCard>();
            await UniTask.DelayFrame(1);
            card.Init(cardVisualPool, skillData,true);
            card.visual.SetParameters(visualParameters);
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

            _valueUIItems[0].valueNum.text = data.currentHp.ToString();
            _valueUIItems[0].slider.value = (float)data.currentHp / data.config.MaxHp;
            
            _valueUIItems[1].valueNum.text = data.currentAtk.ToString();
            _valueUIItems[1].slider.value = (float)data.currentAtk / data.config.MaxAtk;
            
            _valueUIItems[2].valueNum.text = data.currentDef.ToString();
            _valueUIItems[2].slider.value = (float)data.currentDef / data.config.MaxDef;
            
            _valueUIItems[3].valueNum.text = data.currentSpeed.ToString();
            _valueUIItems[3].slider.value = (float)data.currentSpeed / data.config.MaxSpeed;
            
            _valueUIItems[4].valueNum.text = data.currentAdap.ToString();
            _valueUIItems[4].slider.value = (float)data.currentAdap / data.config.MaxAdap;

            ownedCardList.totalCount = data.ownedSkills.Count;
            ownedCardList.RefillCells();
            ownedCardList.RefreshCells();
        }
    }
}