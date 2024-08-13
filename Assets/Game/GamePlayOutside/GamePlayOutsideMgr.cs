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
        public OpponentConfig opponents;

        protected override void OnInit()
        {
            dateSystem = new DateSystem();

            machine = new StateMachine<GamePlayOutsideMgr>(this);
            machine.Add(new FirstSettingState());
            machine.Add(new FirstChooseState());
            machine.Add(new DailyTrainState());
            machine.Add(new SpecialTrainState());
            machine.Add(new BattleSettlementState());
            machine.Add(new SelectOpponentState());
            machine.Add(new ChangeHuluState());

            machine.OnStateChange += OnOutsideStateChange;


            // machine.Run<FirstSettingState>();

            Register();

            PlayBGM();
        }

        private void OnOutsideStateChange(Type from, Type to)
        {
            Global.Get<DataSystem>().Get<GameData>().gameOutsideStateType = to;
            Global.LogInfo($"GameOutsideState Switch {from}:{to}");
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