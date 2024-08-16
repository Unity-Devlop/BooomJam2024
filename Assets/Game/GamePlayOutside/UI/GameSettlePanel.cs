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
        public Button confirmBtn;

        private SettleEnum settle;
        private BattleSettlementData settlementData;
        private List<HuluData> hulus = new List<HuluData>();
        private int count = 0;
        private bool isNextSeason = false;

        public override void OnLoaded()
        {
            base.OnLoaded();
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
            }
            else
            {
                m_rectLose.gameObject.SetActive(true);
                HuluData data = Global.Get<DataSystem>().Get<GameData>().playerData.trainerData.datas
                    .Find(x => x.guid == settlementData.SVP().guid);
                Assert.IsNotNull(data);
                uIShow.Bind(data);
                Global.Get<DataSystem>().Get<GameData>().allowCompeting = false;
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

        private async void Confirm()
        {
            ManageCardsPanel mcp;
            switch (count)
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
            }
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