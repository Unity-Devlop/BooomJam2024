using System;
using System.Collections.Generic;
using UnityToolkit;

namespace Game
{

    public class GamePlayOutsideMgr : MonoSingleton<GamePlayOutsideMgr>
    {
        public StateMachine<GamePlayOutsideMgr> machine { get; private set; }
        public DateSystem dateSystem;

        protected override void OnInit()
        {
            base.OnInit();
            Global.Get<DataSystem>().Add(new PlayerData());
            dateSystem = new DateSystem(2024, 8, 1, 1); //暂时写死，后续改为读表
            machine = new StateMachine<GamePlayOutsideMgr>(this);
            machine.Add(new FirstSettingState());
            machine.Add(new FirstChooseState());
            machine.Add(new DailyTrainState());
            machine.Add(new SpecialTrainState());
            machine.Run<FirstSettingState>();
            Register();
        }

        protected override void OnDispose()
        {
            UnRegister();
            base.OnDispose();
        }

        private void Update()
        {
            machine.OnUpdate();
        }
        

        private void Register()
        {
        }
        
        private void UnRegister()
        {
        }
    }
}