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
        public BattleData envData;

        protected override void OnInit()
        {
            UIRoot.Singleton.OpenPanel<GameDebugPanel>();
            // Init Battle Controller
            battleFlow = GetComponent<DummyBattleFlow>();
        }

        [Button]
        public void DebugStart()
        {
            StartBattle(playerBattleTrainer.trainerData, robotBattleTrainer.trainerData, envData).Forget();
        }

        public async void RollToStart()
        {
            BattleEnvironmentEnum[] values = (BattleEnvironmentEnum[])Enum.GetValues(typeof(BattleEnvironmentEnum));
            envData.id = values.Shuffle()[0];

            HuluEnum[] huluValues = (HuluEnum[])Enum.GetValues(typeof(HuluEnum));
            huluValues.Shuffle();

            TrainerData playerTrainerData = new TrainerData();
            playerTrainerData.RollTrainerSkill9();
            for (int i = 0; i < 3; i++)
            {
                var item = new HuluData();
                item.id = huluValues[i];
                item.elementEnum = item.config.Elements;
                item.Roll9Skills();
                item.RollAbility();
                playerTrainerData.datas.Add(item);
            }

            playerBattleTrainer.Init(playerTrainerData);


            TrainerData aiTrainerData = new TrainerData();
            aiTrainerData.RollTrainerSkill9();
            for (int i = 3; i < 6; i++)
            {
                var item = new HuluData();
                item.id = huluValues[i];
                item.elementEnum = item.config.Elements;
                item.Roll9Skills();
                item.RollAbility();
                aiTrainerData.datas.Add(item);
            }

            robotBattleTrainer.Init(aiTrainerData);


            battleFlow.Init(playerBattleTrainer, robotBattleTrainer, envData);
            await battleFlow.Enter();
        }

        public async UniTask StartBattle(TrainerData self, TrainerData enemy, BattleData battleData)
        {
            TrainerData playerTrainerData = self;
            TrainerData aiTrainerData = enemy;
            playerBattleTrainer.Init(playerTrainerData); // 暂时用Inspector配置的数据
            robotBattleTrainer.Init(aiTrainerData);

            battleFlow.Init(playerBattleTrainer, robotBattleTrainer, envData);
            await battleFlow.Enter();
        }

        protected override void OnDispose()
        {
            battleFlow.Dispose();
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
    }
}