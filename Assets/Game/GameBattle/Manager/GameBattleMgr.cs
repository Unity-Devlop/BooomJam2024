using System;
using System.Runtime.CompilerServices;
using cfg;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game.GamePlay
{
    public class GameBattleMgr : MonoSingleton<GameBattleMgr>
    {
        public DummyBattleFlow battleFlow { get; private set; }

        [field: SerializeField] public CameraEffect cameraEffect { get; private set; }

        // [HorizontalGroup("TrainerGroup")] 
        public PlayerBattleTrainer playerBattleTrainer;

        // [HorizontalGroup("TrainerGroup")] 
        public DummyRobot robotBattleTrainer;


        protected async override void OnInit()
        {
            // Init Battle Controller
            battleFlow = GetComponent<DummyBattleFlow>();
            Global.Event.Listen<BattleInfoRecordEvent>(OnTip);

#if UNITY_EDITOR

            await UniTask.WaitUntil(() => Global.Singleton.initialized);
            if (SceneManager.GetActiveScene().name == "QuickGameBattle")
            {
                DebugStartBattle();
            }
#endif
        }

        private void OnTip(BattleInfoRecordEvent obj)
        {
            Global.LogInfo(obj.tip);
        }

        protected override void OnDispose()
        {
            Global.Event.UnListen<BattleInfoRecordEvent>(OnTip);
            battleFlow.Dispose();
            if (UIRoot.Singleton.GetOpenedPanel(out GameBattlePanel battlePanel))
            {
                battlePanel.UnBind();
            }
        }

        public bool battling { get; private set; } = false;

        public async void StartBattle(TrainerData self, TrainerData enemy, BattleEnvData battleEnvData)
        {
            TrainerData playerTrainerData = self;
            TrainerData aiTrainerData = enemy;
            playerBattleTrainer.Init(playerTrainerData); // 暂时用Inspector配置的数据
            robotBattleTrainer.Init(aiTrainerData);
            battleFlow.Init(playerBattleTrainer, robotBattleTrainer, battleEnvData);


            GameBattlePanel gameBattlePanel = await UIRoot.Singleton.OpenPanelAsync<GameBattlePanel>();
            gameBattlePanel.Bind(battleFlow.self);
            battling = true;
            battleFlow.Enter().ContinueWith(OnBattleEnd).Forget();
        }


        private async void OnBattleEnd()
        {
            playerBattleTrainer.OnBattleEnd();
            robotBattleTrainer.OnBattleEnd();
            battling = false;
            if (UIRoot.Singleton.GetOpenedPanel(out GameBattlePanel battlePanel))
            {
                battlePanel.UnBind();
            }

            Global.Get<DataSystem>().Get<GameData>().battleSettlementData = battleFlow.settlementData;
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
            StartBattle(playerBattleTrainer.trainerData, robotBattleTrainer.trainerData,
                GameMath.RandomBattleEnvData());
        }
    }
}