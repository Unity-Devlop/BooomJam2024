#define QUICK_DEV
using System.IO;
using Cysharp.Threading.Tasks;
using Game.Game;
using Newtonsoft.Json;
using UnityEngine;
using UnityToolkit;

namespace Game.GamePlay
{
    public class GameBattleMgr : MonoSingleton<GameBattleMgr>
    {
        public BattleController battleCtrl { get; private set; }

        public PlayerController playerController;
        public RobotController robotController;

        private SystemLocator _systems;

        private StateMachine<GameBattleMgr> _stateMachine;


        protected override async void OnInit()
        {
#if QUICK_DEV
            // 访问一下 让Global初始化 正常从GameEntry进是不需要这一步的 因为初始化完毕才会加载到GamePlayMgr
            var _ = Global.Singleton;
#endif

            // Register GamePlay System
            _systems = new SystemLocator();


            // Init Battle Controller
            battleCtrl = GetComponent<BattleController>();

            var local = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(Consts.LocalPlayerDataPath));
            playerController.playerData = local;


            // TODO后续删除这个等待逻辑 因为在进入游戏时 一定初始完毕了
            await UniTask.DelayFrame(5);
            // 从游戏流程中获取数据
            BattleData battleData = Global.Get<GameFlow>().GetParam<BattleData>(nameof(BattleData));


            // 随机生成机器人 TODO 后续配置一下 随机几个拿出来
            Debug.LogWarning($"随机生成机器人未实现");
            // TrainerData data = new TrainerData();
            // robotController.trainerData = data;
            GameBattlePanel gameBattlePanel = UIRoot.Singleton.OpenPanel<GameBattlePanel>();
            gameBattlePanel.Bind(battleData);
            
        }

        protected override void OnDispose()
        {
            UIRoot.Singleton.ClosePanel<GameBattlePanel>();
        }
    }
}