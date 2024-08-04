using System;
using System.Runtime.CompilerServices;
using cfg;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game.GamePlay
{
    public class GameBattleMgr : MonoSingleton<GameBattleMgr>
    {
        public DummyBattleFlow battleFlow { get; private set; }

        // [HorizontalGroup("TrainerGroup")] 
        public PlayerBattleTrainer playerBattleTrainer;

        // [HorizontalGroup("TrainerGroup")] 
        public DummyRobot robotBattleTrainer;

        protected override void OnInit()
        {
            // Init Battle Controller
            battleFlow = GetComponent<DummyBattleFlow>();
        }

        protected override void OnDispose()
        {
            battleFlow.Dispose();
        }

        public async UniTask StartBattle(TrainerData self, TrainerData enemy, BattleEnvData battleEnvData)
        {
            TrainerData playerTrainerData = self;
            TrainerData aiTrainerData = enemy;
            playerBattleTrainer.Init(playerTrainerData); // 暂时用Inspector配置的数据
            robotBattleTrainer.Init(aiTrainerData);

            battleFlow.Init(playerBattleTrainer, robotBattleTrainer, battleEnvData);
            await battleFlow.Enter();
            OnBattleEnd();
        }

        private async void OnBattleEnd()
        {
            Global.Get<GameFlow>().SetParam(Consts.BattleSettlementData, battleFlow.settlementData);
            await Global.Get<GameFlow>().ToGameOutside<BattleSettlementState>();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlayBGM()
        {
            Global.Get<AudioSystem>().PlaySingleton(FMODName.Event.MX_COMBAT_DEMO1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StopBGM()
        {
            Global.Get<AudioSystem>().StopSingleton(FMODName.Event.MX_COMBAT_DEMO1, STOP_MODE.ALLOWFADEOUT);
        }

        public void DebugStartBattle()
        {
            StartBattle(playerBattleTrainer.trainerData, robotBattleTrainer.trainerData, GameMath.RandomBattleEnvData());
        }
    }
}