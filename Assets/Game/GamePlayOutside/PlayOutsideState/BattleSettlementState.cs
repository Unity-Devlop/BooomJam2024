﻿using UnityToolkit;

namespace Game
{
    public class BattleSettlementState : IState<GamePlayOutsideMgr>
    {
        public void OnInit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public async void OnEnter(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            BattleSettlementData data = Global.Get<DataSystem>().Get<GameData>().battleSettlementData;
            // TODO Do something with data
            await UIRoot.Singleton.OpenPanelAsync<GameSettlePanel>();
        }

        public void OnUpdate(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public void OnExit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            UIRoot.Singleton.ClosePanel<GameSettlePanel>();
        }
    }
}