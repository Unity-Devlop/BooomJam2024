using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class SelectOpponentState : IState<GamePlayOutsideMgr>
    {
        public void OnInit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public void OnEnter(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            UIRoot.Singleton.OpenPanel<SelectHuluPanel>();
        }

        public void OnUpdate(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public void OnExit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            UIRoot.Singleton.ClosePanel<SelectHuluPanel>();
        }
    }
}
