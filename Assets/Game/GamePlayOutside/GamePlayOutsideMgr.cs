using System.Collections.Generic;
using UnityToolkit;

namespace Game
{
    public enum POState
    {
        FirstSettingState,
        FirstChooseState,
        DailyTrainState,
        SpecialTrainState,
    }

    public class GamePlayOutsideMgr : MonoSingleton<GamePlayOutsideMgr>
    {
        private Dictionary<POState, PlayOutsideState> states = new Dictionary<POState, PlayOutsideState>();
        private PlayOutsideState curState;

        public DateSystem dateSystem;

        protected override void OnInit()
        {
            base.OnInit();
            Global.Get<DataSystem>().Add(new PlayerData());
            dateSystem = new DateSystem(2024,8,1,1);//暂时写死，后续改为读表
            states.Add(POState.FirstSettingState,new FirstSettingState());
            states.Add(POState.FirstChooseState, new FirstChooseState());
            states.Add(POState.DailyTrainState, new DailyTrainState());
            states.Add(POState.SpecialTrainState, new SpecialTrainState());
            curState = states[POState.FirstSettingState];
            curState.OnEnter();
            Register();
        }

        protected override void OnDispose()
        {
            UnRegister();
            base.OnDispose();
        }

        private void Update()
        {
            curState.OnStay();
        }

        private void ChangeState(ChangeStateEvent e)
        {
            if (curState != null) curState.OnExit();
            curState = states[e.poState];
            curState.OnEnter();
        }

        private void Register()
        {
            TypeEventSystem.Global.Listen<ChangeStateEvent>(ChangeState);
        }

        private void UnRegister()
        {
            TypeEventSystem.Global.UnListen<ChangeStateEvent>(ChangeState);
        }
    }

    public class ChangeStateEvent
    {
        public POState poState;
    }
}