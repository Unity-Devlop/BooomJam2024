using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityToolkit;

namespace Game
{
    public enum SettleEnum
    {
        Win,
        Lose,
    }

    public class GameSettlePanel : UIPanel
    {
        public GameObject m_rectWin;
        public GameObject m_rectLose;
        public TextMeshProUGUI m_txtReward;
        public PokemonUIShow uIShow;
        public GameObject valueList;
        public Button confirmBtn;

        private SettleEnum settle;
        private BattleSettlementData settlementData;
        private List<HuluData> hulus = new List<HuluData>();
        private ValueUIItem[] valueUIItems;
        private int count = 0;
        private bool isNextSeason = false;

        public override void OnLoaded()
        {
            base.OnLoaded();
            valueUIItems = valueList.GetComponentsInChildren<ValueUIItem>();
            Register();
        }

        public override void OnDispose()
        {
            base.OnDispose();
            UnRegister();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            Refresh();
            settlementData = Global.Get<DataSystem>().Get<GameData>().battleSettlementData;
            GameSettle();
        }

        private void Register()
        {
            confirmBtn.onClick.AddListener(Confirm);
        }

        private void UnRegister()
        {
            confirmBtn.onClick.RemoveListener(Confirm);
        }

        public void GameSettle()
        {
            settle = settlementData.LocalPlayerWin() ? SettleEnum.Win : SettleEnum.Lose;
            for (int i = 0; i < settlementData.localPlayerTrainerData.datas.Count; ++i) hulus.Add(Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas
                    .Find(x => x.guid == settlementData.localPlayerTrainerData.datas[i].guid)) ;
            if (settle == SettleEnum.Win)
            {
                m_rectWin.gameObject.SetActive(true);
                HuluData data = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas
                    .Find(x => x.guid == settlementData.MVP().guid);
                Assert.IsNotNull(data);
                uIShow.Bind(data);
                AbilitiesUp(Random.Range(0,5),Random.Range(3, 6),data);
            }
            else
            {
                m_rectLose.gameObject.SetActive(true);
                HuluData data = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas
                    .Find(x => x.guid == settlementData.SVP().guid);
                Assert.IsNotNull(data);
                uIShow.Bind(data);
                Global.Get<DataSystem>().Get<GameData>().allowCompeting = false;
                AbilitiesUp(Random.Range(0, 5), Random.Range(2, 5), data);
            }

            m_txtReward.text = $"+{settlementData.LocalAdmirePoint()}";
            Global.Get<DataSystem>().Get<GameData>().admireNum += settlementData.LocalAdmirePoint();
            GamePlayOutsideMgr.Singleton.dateSystem.MonthElapse(1);
            if (++Global.Get<DataSystem>().Get<GameData>().date.count % 3 == 0)
            {
                GamePlayOutsideMgr.Singleton.dateSystem.SeasonElapse(1);
                Global.Get<DataSystem>().Get<GameData>().allowCompeting = true;
                isNextSeason = true;
            }
        }

        private void AbilitiesUp(int index,int num,HuluData data)
        {
            valueUIItems[0].slider.value = data.hp/(float)data.config.MaxHp;
            valueUIItems[1].slider.value = data.atk/(float)data.config.MaxAtk;
            valueUIItems[2].slider.value = data.def/(float)data.config.MaxDef;
            valueUIItems[3].slider.value = data.speed/(float)data.config.MaxSpeed;
            valueUIItems[4].slider.value = data.adap/(float)data.config.MaxAdap;
            valueUIItems[0].valueNum.text = data.hp.ToString();
            valueUIItems[1].valueNum.text = data.atk.ToString();
            valueUIItems[2].valueNum.text = data.def.ToString();
            valueUIItems[3].valueNum.text = data.speed.ToString();
            valueUIItems[4].valueNum.text = data.adap.ToString();
            if (index > 4) return;
            if (index == 0)
            {
                valueUIItems[0].valueNum.text = $"{data.hp}+{num}";
                data.hp = Mathf.Min(data.hp + num, data.config.MaxHp);
                valueUIItems[0].slider.value = data.hp / (float)data.config.MaxHp;
            }
            else if (index == 1)
            {
                valueUIItems[1].valueNum.text = $"{data.atk}+{num}";
                data.atk = Mathf.Min(data.atk + num, data.config.MaxAtk);
                valueUIItems[1].slider.value = data.atk / (float)data.config.MaxAtk;
            }
            else if (index == 2)
            {
                valueUIItems[2].valueNum.text = $"{data.def}+{num}";
                data.def = Mathf.Min(data.def + num, data.config.MaxDef);
                valueUIItems[2].slider.value = data.def / (float)data.config.MaxDef;
            }
            else if (index == 3)
            {
                valueUIItems[3].valueNum.text = $"{data.speed}+{num}";
                data.speed = Mathf.Min(data.speed + num, data.config.MaxSpeed);
                valueUIItems[3].slider.value = data.speed / (float)data.config.MaxSpeed;
            }
            else if (index == 4)
            {
                valueUIItems[4].valueNum.text = $"{data.adap}+{num}";
                data.adap = Mathf.Min(data.adap + num, data.config.MaxAdap);
                valueUIItems[4].slider.value = data.adap / (float)data.config.MaxAdap;
            }
        }

        private async void Confirm()
        {
            ManageCardsPanel mcp;
            if(count==0)
            {
                mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                mcp.DeleteTrainerSkillCard();
                ++count;
            }
            else if(count==1)
            {
                mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                mcp.SelectTrainerSkillCard();
                ++count;
            }
            else if(count<=1+hulus.Count)
            {
                mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                mcp.DeleteHuluSkillCard(hulus[count-2]);
                ++count;
            }
            else if(count<=1+hulus.Count*2)
            {
                mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                mcp.SelectHuluSkillCard(hulus[count - hulus.Count-2]);
                ++count;
            }
            else
            {
                if (isNextSeason) GamePlayOutsideMgr.Singleton.machine.Change<ChangeHuluState>();
                else GamePlayOutsideMgr.Singleton.machine.Change<DailyTrainState>();
            }
            /*switch (count)
            {
                case 0:
                    mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                    mcp.DeleteTrainerSkillCard();
                    ++count;
                    break;
                case 1:
                    mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                    mcp.SelectTrainerSkillCard();
                    ++count;
                    break;
                case 2:
                    mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                    mcp.SelectHuluSkillCard(hulus[0]);
                    ++count;
                    break;
                case 3:
                    mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                    mcp.SelectHuluSkillCard(hulus[1]);
                    ++count;
                    break;
                case 4:
                    mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                    mcp.SelectHuluSkillCard(hulus[2]);
                    ++count;
                    break;
                case 5:
                    mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                    mcp.DeleteHuluSkillCard(hulus[0]);
                    ++count;
                    break;
                case 6:
                    mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                    mcp.DeleteHuluSkillCard(hulus[1]);
                    ++count;
                    break;
                case 7:
                    mcp = await UIRoot.Singleton.OpenPanelAsync<ManageCardsPanel>();
                    mcp.DeleteHuluSkillCard(hulus[2]);
                    ++count;
                    break;
                default:
                    if (isNextSeason) GamePlayOutsideMgr.Singleton.machine.Change<ChangeHuluState>();
                    else GamePlayOutsideMgr.Singleton.machine.Change<DailyTrainState>();
                    break;
            }*/
        }

        private void Refresh()
        {
            hulus.Clear();
            m_rectWin.gameObject.SetActive(false);
            m_rectLose.gameObject.SetActive(false);
            count = 0;
            isNextSeason = false;
        }
    }
}