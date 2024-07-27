#define QUICK_DEV
using System.IO;
using Game.Game;
using Newtonsoft.Json;
using UnityToolkit;

namespace Game.GamePlay
{
    public class GamePlayMgr : MonoSingleton<GamePlayMgr>
    {
        public PlayerData local { get; private set; }

        public BattleController battleCtrl { get; private set; }

        private SystemLocator _systems;

        protected override void OnInit()
        {
#if QUICK_DEV
            // 访问一下 让Global初始化 正常从GameEntry进是不需要这一步的 因为初始化完毕才会加载到GamePlayMgr
            var _ = Global.Singleton;
#endif
            local = JsonConvert.DeserializeObject<PlayerData>(File.ReadAllText(Consts.LocalPlayerDataPath));
            // TODO 创建角色逻辑
            if (local == null)
            {
                local = new PlayerData();
            }

            _systems = new SystemLocator();

            battleCtrl = GetComponent<BattleController>();
            
            


            UIRoot.Singleton.OpenPanel<GameHUDPanel>();
        }

        protected override void OnDispose()
        {
            UIRoot.Singleton.ClosePanel<GameHUDPanel>();
        }
    }
}