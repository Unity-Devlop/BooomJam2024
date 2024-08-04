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
            UIRoot.Singleton.OpenPanel<GameDebugPanel>();
            // Init Battle Controller
            battleFlow = GetComponent<DummyBattleFlow>();
        }

        protected override void OnDispose()
        {
            battleFlow.Dispose();
        }

        [Button]
        public void DebugStart()
        {
            BattleData data = new BattleData();
            data.id = BattleEnvironmentEnum.草地;
            StartBattle(playerBattleTrainer.trainerData, robotBattleTrainer.trainerData, data).Forget();
        }

        public async void RollToStart()
        {
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

            BattleData battleData = GameMath.RandomBattleData();

            StartBattle(playerTrainerData, aiTrainerData, battleData).Forget();
        }

        public async UniTask StartBattle(TrainerData self, TrainerData enemy, BattleData battleData)
        {
            TrainerData playerTrainerData = self;
            TrainerData aiTrainerData = enemy;
            playerBattleTrainer.Init(playerTrainerData); // 暂时用Inspector配置的数据
            robotBattleTrainer.Init(aiTrainerData);

            battleFlow.Init(playerBattleTrainer, robotBattleTrainer, battleData);
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
    }
}