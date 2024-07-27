#define QUICK_DEV
using System.IO;
using Game.Game;
using Newtonsoft.Json;
using UnityEngine;
using UnityToolkit;

namespace Game.GamePlay
{
    public class GamePlayMgr : MonoSingleton<GamePlayMgr>
    {
        public BattleController battleCtrl { get; private set; }

        public PlayerController playerController;
        public RobotController robotController;

        private SystemLocator _systems;
        
        protected override void OnInit()
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

            // 随机生成机器人 TODO 后续配置一下 随机几个拿出来
            Debug.LogWarning($"随机生成机器人未实现");
            // TrainerData data = new TrainerData();
            // robotController.trainerData = data;


            UIRoot.Singleton.OpenPanel<GameHUDPanel>();
        }

        protected override void OnDispose()
        {
            UIRoot.Singleton.ClosePanel<GameHUDPanel>();
        }
    }
}