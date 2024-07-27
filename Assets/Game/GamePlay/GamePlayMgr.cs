#define QUICK_DEV
using Game.Game;
using Newtonsoft.Json;
using UnityToolkit;

namespace Game.GamePlay
{
    public class GamePlayMgr : MonoSingleton<GamePlayMgr>
    {
        public PlayerData Local { get; private set; }

        protected override void OnInit()
        {
#if QUICK_DEV
            // 访问一下 让Global初始化 正常从GameEntry进是不需要这一步的 因为初始化完毕才会加载到GamePlayMgr
            var _ = Global.Singleton;
#endif
            Local = JsonConvert.DeserializeObject<PlayerData>(Consts.LocalPlayerDataPath);
            // TODO 创建角色逻辑
            if (Local == null)
            {
                Local = new PlayerData();
            }
        }
    }
}