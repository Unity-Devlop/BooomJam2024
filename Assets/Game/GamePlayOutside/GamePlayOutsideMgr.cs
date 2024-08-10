using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityToolkit;

namespace Game
{

    public class GamePlayOutsideMgr : MonoSingleton<GamePlayOutsideMgr>
    {
        public StateMachine<GamePlayOutsideMgr> machine { get; private set; }
        public DateSystem dateSystem;
        public List<Opponent> opponents = new List<Opponent>();

        protected override void OnInit()
        {
            Global.Get<DataSystem>().Get<GameData>();
            dateSystem = new DateSystem(2024, 8, 1, 1); //暂时写死，后续改为读表
            
            machine = new StateMachine<GamePlayOutsideMgr>(this);
            machine.Add(new FirstSettingState());
            machine.Add(new FirstChooseState());
            machine.Add(new DailyTrainState());
            machine.Add(new SpecialTrainState());
            machine.Add(new BattleSettlementState());
            machine.Add(new SelectOpponentState());
            
            machine.Run<FirstSettingState>();
            
            Register();
            
            PlayBGM();
        }

        protected override void OnDispose()
        {
            UnRegister();
            machine.Stop();
            StopBGM();
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
        
        public void PlayBGM()
        {
            Global.Get<AudioSystem>().PlaySingleton(FMODName.Event.MX_NORMAL_DEMO1);
        }

        public void StopBGM()
        {
            Global.Get<AudioSystem>().StopSingleton(FMODName.Event.MX_NORMAL_DEMO1, STOP_MODE.ALLOWFADEOUT);
        }
    }
}