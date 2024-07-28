#define QUICK_DEV
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
        public RebotBattleTrainer rebotBattleTrainer;


        protected override async void OnInit()
        {
#if QUICK_DEV
            // 访问一下 让Global初始化 正常从GameEntry进是不需要这一步的 因为初始化完毕才会加载到GamePlayMgr
            var _ = Global.Singleton;
#endif

            // Init Battle Controller
            battleFlow = GetComponent<DummyBattleFlow>();
            await UniTask.DelayFrame(5); // TODO 后续删除这个等待逻辑 因为在进入游戏时 一定初始完毕了
            // TrainerData trainerData = Global.Get<GameFlow>().GetParam<TrainerData>(nameof(TrainerData)); // 从游戏流程中获取数据
            // playerTrainer.Init(trainerData); // 暂时用Inspector配置的数据

            Debug.LogWarning($"随机生成机器人未实现"); // TODO 后续配置一下 随机几个拿出来
            // TrainerData data = new TrainerData();
            // robotController.trainerData = data;

            battleFlow.Init(playerBattleTrainer, rebotBattleTrainer);
            await battleFlow.Enter();
        }

        protected override void OnDispose()
        {
            battleFlow.Dispose();
        }
    }
}