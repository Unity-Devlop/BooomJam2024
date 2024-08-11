using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
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
        private int count = 0;

        public override void OnLoaded()
        {
            base.OnLoaded();
            settlementData = Global.Get<DataSystem>().Get<GameData>().battleSettlementData;
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
            settle = settlementData.LocalPlayerWin()?SettleEnum.Win:SettleEnum.Lose;
            if(settle==SettleEnum.Win)
            {
                m_rectWin.gameObject.SetActive(true);
                uIShow.Bind(settlementData.MVP().Key);
            }
            else
            {
                m_rectLose.gameObject.SetActive(true);
                uIShow.Bind(settlementData.SVP().Key);
            }
            m_txtReward.text = $"+{settlementData.LocalAdmirePoint()}";
            Global.Get<DataSystem>().Get<GameData>().admireNum += settlementData.LocalAdmirePoint();
        }

        private void Confirm()
        {
            ManageCardsPanel mcp;
            switch(count)
            {
                case 0:
                    mcp = UIRoot.Singleton.OpenPanel<ManageCardsPanel>();
                    mcp.DeleteTrainerSkillCard();
                    ++count;
                    break;
                case 1:
                    mcp = UIRoot.Singleton.OpenPanel<ManageCardsPanel>();
                    mcp.SelectTrainerSkillCard();
                    ++count;
                    break;
                case 2:
                    mcp = UIRoot.Singleton.OpenPanel<ManageCardsPanel>();
                    mcp.SelectHuluSkillCard(settlementData.localPlayerTrainerData.datas[0]);
                    ++count;
                    break;
                case 3:
                    mcp = UIRoot.Singleton.OpenPanel<ManageCardsPanel>();
                    mcp.SelectHuluSkillCard(settlementData.localPlayerTrainerData.datas[1]);
                    ++count;
                    break;
                case 4:
                    mcp = UIRoot.Singleton.OpenPanel<ManageCardsPanel>();
                    mcp.SelectHuluSkillCard(settlementData.localPlayerTrainerData.datas[2]);
                    ++count;
                    break;
                case 5:
                    mcp = UIRoot.Singleton.OpenPanel<ManageCardsPanel>();
                    mcp.DeleteHuluSkillCard(settlementData.localPlayerTrainerData.datas[0]);
                    ++count;
                    break;
                case 6:
                    mcp = UIRoot.Singleton.OpenPanel<ManageCardsPanel>();
                    mcp.DeleteHuluSkillCard(settlementData.localPlayerTrainerData.datas[1]);
                    ++count;
                    break;
                case 7:
                    mcp = UIRoot.Singleton.OpenPanel<ManageCardsPanel>();
                    mcp.DeleteHuluSkillCard(settlementData.localPlayerTrainerData.datas[2]);
                    ++count;
                    break;
                default:
                    GamePlayOutsideMgr.Singleton.machine.Change<DailyTrainState>();
                    break;

            }
        }

        private void Refresh()
        {
            m_rectWin.gameObject.SetActive(false);
            m_rectLose.gameObject.SetActive(false);
            count = 0;
            
        }

    }
}
