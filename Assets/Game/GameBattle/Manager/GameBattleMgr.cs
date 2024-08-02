#define QUICK_DEV
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Game.Game;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game.GamePlay
{
    public class GameBattleMgr : MonoSingleton<GameBattleMgr>
    {
        public DummyBattleFlow battleFlow { get; private set; }

        public PlayerBattleTrainer playerBattleTrainer;
        [FormerlySerializedAs("rebotBattleTrainer")] public RobotBattleTrainer robotBattleTrainer;
        public BattleEnvironmentData environmentData;

        protected override async void OnInit()
        {
#if QUICK_DEV
            // 访问一下 让Global初始化 正常从GameEntry进是不需要这一步的 因为初始化完毕才会加载到GamePlayMgr
            var _ = Global.Singleton;
#endif

            // Init Battle Controller
            battleFlow = GetComponent<DummyBattleFlow>();
            await UniTask.Delay(TimeSpan.FromSeconds(1)); // TODO 后续删除这个等待逻辑 因为在进入游戏时 一定初始完毕了
            StartBattle();
        }

        public async void StartBattle()
        {
            //TODO Global.Get<GameFlow>().GetParam<TrainerData>(nameof(TrainerData)); // 从游戏流程中获取数据
            TrainerData playerTrainerData = playerBattleTrainer.trainerData;
            // TODO Global.Get<GameFlow>().GetParam<BattleEnvironmentData>(nameof(BattleEnvironmentData));
            environmentData = environmentData;
            playerBattleTrainer.Init(playerTrainerData); // 暂时用Inspector配置的数据

            // TODO 后续配置一下 随机几个拿出来
            Debug.LogWarning($"随机生成机器人未实现");
            TrainerData aiTrainerData = robotBattleTrainer.trainerData;
            robotBattleTrainer.Init(aiTrainerData);


            battleFlow.Init(playerBattleTrainer, robotBattleTrainer, environmentData);
            await battleFlow.Enter();
        }

        protected override void OnDispose()
        {
            battleFlow.Dispose();
        }
    }
}