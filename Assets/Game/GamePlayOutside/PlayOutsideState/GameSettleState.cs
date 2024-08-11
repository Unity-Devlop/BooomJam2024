using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class GameSettleState : IState<GamePlayOutsideMgr>
    {
        public void OnEnter(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            UIRoot.Singleton.OpenPanel<GameSettlePanel>();
        }

        public void OnExit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            UIRoot.Singleton.ClosePanel<GameSettlePanel>();
        }

        public void OnInit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {

        }

        public void OnUpdate(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {

        }
    }
}
