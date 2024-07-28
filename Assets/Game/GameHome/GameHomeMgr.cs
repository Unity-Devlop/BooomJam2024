using UnityToolkit;

namespace Game.GameHome
{
    public class GameHomeMgr : MonoSingleton<GameHomeMgr>
    {
        protected override void OnInit()
        {
            // PlayerData data = new PlayerData();
            // Global.Get<DataSystem>().Add(data);
            UIRoot.Singleton.OpenPanel<GameHomePanel>();
        }

        protected override void OnDispose()
        {
            UIRoot.Singleton.ClosePanel<GameHomePanel>();
        }
    }
}