using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class FirstChoosePanel : UIPanel
    {
        public GameObject roleList;
        public Image roleShowImg;
        public Text roleShowName;
        public Text roleShowPassiveSkill;
        public GameObject skillList;
        public GameObject ValueList;
        public Button chooseBtn;
        public GameObject rolePortraitUIItem;
        public List<HuluEnum> huluIds = new List<HuluEnum>();

        private RolePortraitUIItem[] rolePortraitUIItems;
        private SkillUIItem[] skillUIItems;
        private ValueUIItem[] valueUIItems;
        private List<List<ActiveSkillEnum>> activeSkills = new List<List<ActiveSkillEnum>>();
        private int curHulu = 0;

        public override void OnLoaded()
        {
            base.OnLoaded();
            Register();
            skillUIItems = skillList.GetComponentsInChildren<SkillUIItem>();
            valueUIItems = ValueList.GetComponentsInChildren<ValueUIItem>();
            rolePortraitUIItems = new RolePortraitUIItem[huluIds.Count];
            for(int i=0;i<huluIds.Count;++i)
            {
                var huluData = Global.Table.HuluTable.Get(huluIds[i]);
                var go = Instantiate(rolePortraitUIItem,roleList.transform);
                rolePortraitUIItems[i] = go.GetComponent<RolePortraitUIItem>();
                rolePortraitUIItems[i].roleName.text = huluData.Id.ToString();
                rolePortraitUIItems[i].index = i;
                activeSkills.Add(GetRandomSkill(huluData.SkillPool));
            }
        }

        public override void OnDispose()
        {
            UnRegister();
            base.OnDispose();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            ShowUI();
        }

        private void Register()
        {
            Global.Event.Listen<ClickHuluEvent>(ClickHulu);
        }

        private void UnRegister()
        {
            Global.Event.UnListen<ClickHuluEvent>(ClickHulu);
        }

        private void ShowUI()
        {
            var huluData = Global.Table.HuluTable.Get(huluIds[curHulu]);
            roleShowName.text = huluData.Id.ToString();
            roleShowPassiveSkill.text = Global.Table.PassiveSkillTable.Get(huluData.PassiveSkill).Desc;
            for (int i=0;i<skillUIItems.Length;++i)
            {
                var skill = activeSkills[curHulu][i];
                skillUIItems[i].skillName.text = skill.ToString();
                skillUIItems[i].SkillDescription.text = Global.Table.ActiveSkillTable.Get(skill).Desc;
            }
            valueUIItems[0].valueNum.text = huluData.BaseHp.ToString();
            valueUIItems[0].slider.value = (float)huluData.BaseHp / huluData.MaxHp;
            valueUIItems[1].valueNum.text = huluData.BaseAtk.ToString();
            valueUIItems[1].slider.value = (float)huluData.BaseAtk / huluData.MaxAtk;
            valueUIItems[2].valueNum.text = huluData.BaseDef.ToString();
            valueUIItems[2].slider.value = (float)huluData.BaseDef / huluData.MaxDef;
            valueUIItems[3].valueNum.text = huluData.BaseSpeed.ToString();
            valueUIItems[3].slider.value = (float)huluData.BaseSpeed / huluData.MaxSpeed;
            valueUIItems[4].valueNum.text = huluData.BaseAdap.ToString();
            valueUIItems[4].slider.value = (float)huluData.BaseAdap / huluData.MaxAdap;
        }

        public void ClickHulu(ClickHuluEvent e)
        {
            curHulu = e.index;
            ShowUI();
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
    }

    public class ClickHuluEvent
    {
        public int index;
    }
}
