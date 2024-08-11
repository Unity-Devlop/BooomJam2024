using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public enum ManageState
    {
        None,
        Select,
        Delete,
    }

    public class ManageCardsPanel : UIPanel
    {
        public GameObject selectCard;
        public GameObject deleteCard;
        public Button confirmBtn;
        public ManageCardContainer container;
        public TextMeshProUGUI title;
        public Button cancelBtn;
        private PlayerData playerData;
        private Action<bool, HuluEnum, ActiveSkillEnum, ActiveSkillEnum> action;
        private ActiveSkillEnum curSelectCardId;
        private HuluData curHulu = null;
        private ManageState curManageState;
        private Action callBack = null;

        public override void OnLoaded()
        {
            playerData = Global.Get<DataSystem>().Get<GameData>().playerData;
            Register();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            confirmBtn.gameObject.SetActive(false);
            callBack = null;
        }


        public override void OnClosed()
        {
            base.OnClosed();
            curHulu = null;
            curManageState = ManageState.None;
        }

        public override void OnDispose()
        {
            base.OnDispose();
            UnRegister();
        }

        public void SelectHuluSkillCard(HuluData hulu,Action callback=null)
        {
            title.text = $"ѡ��һ�ſ���\n{hulu.id}";
            curManageState = ManageState.Select;
            curHulu = hulu;
            var activeSkillEnums = GetRandomSkill(Global.Table.HuluTable.Get(hulu.id).SkillPool);
            List<ActiveSkillData> list = new List<ActiveSkillData>();
            for(int i=0;i< activeSkillEnums.Count; ++i)
            {
                var data = new ActiveSkillData();
                data.id = activeSkillEnums[i];
                list.Add(data);
            }
            container.DrawCardToHand(list);
            this.callBack = callback;
        }

        public void SelectTrainerSkillCard()
        {
            title.text = "ѡ��һ��ָ�ӿ���";
            curManageState = ManageState.Select;
            List<ActiveSkillData> list = new List<ActiveSkillData>();
            var targets = Global.Table.ActiveSkillTable.DataList.FindAll((c) => c.Type == ActiveSkillTypeEnum.ָ��);
            targets.Shuffle();
            for (int i = 0; i < 3; ++i)
            {
                var data = new ActiveSkillData();
                data.id = targets[i].Id;
                list.Add(data);
            }
            container.DrawCardToHand(list);
        }

        public void DeleteHuluSkillCard(HuluData hulu,Action callback=null)
        {
            title.text = $"ɾ��һ�ſ���\n{hulu.id}";
            curManageState = ManageState.Delete;
            curHulu = hulu;
            container.DrawCardToHand(hulu.ownedSkills);
            this.callBack = callback;
        }

        public void DeleteTrainerSkillCard()
        {
            title.text = "ɾ��һ��ָ�ӿ���";
            curManageState = ManageState.Delete;
            container.DrawCardToHand(playerData.trainerData.trainerSkills);
        }

        public void ClickSelectSkillCard(ClickCardEvent e)
        {
            curSelectCardId = e.data.id;
            ConfirmSkillCard();
        }

        public void ConfirmSkillCard()
        {
            if (curManageState == ManageState.Select)
            {
                if (curHulu != null)
                {
                    curHulu.AddOwnedSkill(curSelectCardId);
                    if (callBack != null) callBack.Invoke();
                }
                else
                {
                    ActiveSkillData data = new();
                    data.id = curSelectCardId;
                    playerData.trainerData.trainerSkills.Add(data);
                }
            }
            else
            {
                if (curHulu != null)
                {
                    curHulu.RemoveOwnedSkill(curSelectCardId);
                    if (callBack != null) callBack.Invoke();
                }
                else
                {
                    playerData.trainerData.RemoveTrainerSkill(curSelectCardId);
                }
            }
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

        private void Cancel()
        {
            CloseSelf();
        }

        private void Register()
        {
            confirmBtn.onClick.AddListener(ConfirmSkillCard);
            cancelBtn.onClick.AddListener(Cancel);
            Global.Event.Listen<ClickCardEvent>(ClickSelectSkillCard);
        }

        private void UnRegister()
        {
            confirmBtn.onClick.RemoveListener(ConfirmSkillCard);
            cancelBtn.onClick.RemoveListener(Cancel);
            Global.Event.UnListen<ClickCardEvent>(ClickSelectSkillCard);
        }
    }

    public class ClickCardEvent
    {
        public ActiveSkillData data;
    }
}
