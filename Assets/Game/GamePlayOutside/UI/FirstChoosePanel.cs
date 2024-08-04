using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public class FirstChoosePanel : UIPanel
    {
        public GameObject roleList;
        public GameObject skillList;
        public GameObject ValueList;
        public Button chooseBtn;
        public Text chooseBtnText;
        public Button nextBtn;
        public GameObject rolePortraitUIItem;
        public List<HuluEnum> huluIds = new List<HuluEnum>();

        private RolePortraitUIItem[] rolePortraitUIItems;
        private ValueUIItem[] valueUIItems;
        private List<List<ActiveSkillEnum>> activeSkills = new List<List<ActiveSkillEnum>>();
        private List<HuluEnum> chosenHulu = new List<HuluEnum>();
        private int curHulu = 0;


        [SerializeField] private PokemonUIShow show;
        [SerializeField] private PokemonHUD hud;
        
        public override void OnLoaded()
        {
            base.OnLoaded();
            Register();
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
            chooseBtn.onClick.AddListener(Choose);
            nextBtn.onClick.AddListener(Continue);
        }

        private void UnRegister()
        {
            Global.Event.UnListen<ClickHuluEvent>(ClickHulu);
            chooseBtn.onClick.RemoveListener(Choose);
            nextBtn.onClick.RemoveListener(Continue);
        }

        private void ShowUI()
        {
            var config = Global.Table.HuluTable.Get(huluIds[curHulu]);
            HuluData data = new HuluData(huluIds[curHulu]);
            data.Roll9Skills();
            data.RollAbility();
            show.UnBind();
            show.Bind(data);
            
            hud.UnBind();
            hud.Bind(data);
            
            valueUIItems[0].valueNum.text = config.BaseHp.ToString();
            valueUIItems[0].slider.value = (float)config.BaseHp / config.MaxHp;
            valueUIItems[1].valueNum.text = config.BaseAtk.ToString();
            valueUIItems[1].slider.value = (float)config.BaseAtk / config.MaxAtk;
            valueUIItems[2].valueNum.text = config.BaseDef.ToString();
            valueUIItems[2].slider.value = (float)config.BaseDef / config.MaxDef;
            valueUIItems[3].valueNum.text = config.BaseSpeed.ToString();
            valueUIItems[3].slider.value = (float)config.BaseSpeed / config.MaxSpeed;
            valueUIItems[4].valueNum.text = config.BaseAdap.ToString();
            valueUIItems[4].slider.value = (float)config.BaseAdap / config.MaxAdap;
            if(chosenHulu.Contains(config.Id))
            {
                chooseBtnText.text = "取消选择";
            }
            else
            {
                chooseBtnText.text = "选择";
            }
        }

        public void ClickHulu(ClickHuluEvent e)
        {
            curHulu = e.index;
            ShowUI();
        }

        public void Choose()
        {
            if (chosenHulu.Contains(huluIds[curHulu]))
            {
                chosenHulu.Remove(huluIds[curHulu]);
                chooseBtnText.text = "选择";
            }
            else
            {
                if (chosenHulu.Count < 4)
                {
                    chosenHulu.Add(huluIds[curHulu]);
                    chooseBtnText.text = "取消选择";
                }
            }
            if (chosenHulu.Count >= 4) nextBtn.gameObject.SetActive(true);
            else nextBtn.gameObject.SetActive(false);
        }

        public void Continue()
        {
            var playerData = Global.Get<DataSystem>().Get<PlayerData>();
            for(int i=0;i<huluIds.Count; ++i)
            {
                if (chosenHulu.Contains(huluIds[i]))
                {
                    //添加hulu
                    HuluData h = new HuluData(huluIds[i]);
                    h.id = huluIds[i];
                    for (int j = 0; j < activeSkills[i].Count; ++j)
                    {
                        ActiveSkillData asd = new ActiveSkillData();
                        asd.id = activeSkills[i][j];
                        h.ownedSkills.Add(asd);
                    }
                    playerData.trainerData.datas.Add(h);
                 }
            }
            var e = new ChangeStateEvent();
            e.poState = POState.DailyTrainState;
            TypeEventSystem.Global.Send<ChangeStateEvent>(e);
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
