using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    struct Selection
    {
        public HuluEnum huluEnum;
        public ActiveSkillEnum originSkill;

        public Selection(HuluEnum hulu,ActiveSkillEnum ori)
        {
            huluEnum = hulu;
            originSkill = ori;
        }
    }

    public class ManageCardsPanel : UIPanel
    {
        public GameObject selectCard;
        public GameObject deleteCard;
        public Button selectConfirm;
        public CardHorizontalContainer container;

        private CardItem[] selectCardItems;
        private CardItem[] deleteCardItems;
        private PlayerData playerData;
        private List<ActiveSkillEnum> activeSkillEnums;
        private Action<bool, HuluEnum, ActiveSkillEnum, ActiveSkillEnum> action;
        private int curSelectCard=-1;
        private Selection selection;

        public override void OnLoaded()
        {
            selectCardItems = selectCard.GetComponentsInChildren<CardItem>();
            deleteCardItems=deleteCard.GetComponentsInChildren<CardItem>();
            playerData = Global.Get<DataSystem>().Get<PlayerData>();
            Register();
        }

        public override void OnDispose()
        {
            base.OnDispose();
            UnRegister();
        }

        public void SelectSkillCard(HuluEnum id,ActiveSkillEnum ori,Action<bool,HuluEnum,ActiveSkillEnum,ActiveSkillEnum> callback)
        {
            selectCard.SetActive(true);
            deleteCard.SetActive(false);
            activeSkillEnums = GetRandomSkill(Global.Table.HuluTable.Get(id).SkillPool);
            var table = Global.Table.ActiveSkillTable;
            for(int i=0;i< activeSkillEnums.Count; ++i)
            {
                selectCardItems[i].titleTxt.text = table.Get(activeSkillEnums[i]).Id.ToString();
                selectCardItems[i].descriptionTxt.text = table.Get(activeSkillEnums[i]).Desc;
                var data = new ActiveSkillData();
                data.id = activeSkillEnums[i];
                container.AddCardItem(selectCardItems[i], data);
            }
            action = callback;
            selection = new Selection(id, ori);
        }

        public void ClickSelectSkillCard(int index)
        {
            curSelectCard = index;
        }

        public void ConfirmSelectSkillCard()
        {
            if(action!=null)
            {
                if (curSelectCard != -1) action(true, selection.huluEnum, selection.originSkill, activeSkillEnums[curSelectCard]);
                else action(false,default(HuluEnum), default(ActiveSkillEnum), default(ActiveSkillEnum));
            }
            selectCard.SetActive(false);
            CloseSelf();
        }

        private List<ActiveSkillEnum> GetRandomSkill(ActiveSkillEnum[] array)
        {
            System.Random _random = new System.Random();
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + _random.Next(n - i);
                ActiveSkillEnum temp = array[r];
                array[r] = array[i];
                array[i] = temp;
            }
            return new List<ActiveSkillEnum>() { array[0], array[1], array[2] };
        }

        private void Register()
        {
            selectConfirm.onClick.AddListener(ConfirmSelectSkillCard);
            for(int i=0;i<selectCardItems.Length;++i)
            {
                int index = i;
                selectCardItems[i].cardBtn.onClick.AddListener(() => { ClickSelectSkillCard(index); });
            }
        }

        private void UnRegister()
        {
            selectConfirm.onClick.RemoveListener(ConfirmSelectSkillCard);
            for (int i = 0; i < selectCardItems.Length; ++i)
            {
                int index = i;
                selectCardItems[i].cardBtn.onClick.RemoveListener(() => { ClickSelectSkillCard(index); });
            }
        }
    }
}
