using UnityToolkit;

namespace Game.GameHome
{
    public class GameHomeMgr : MonoSingleton<GameHomeMgr>
    {
        protected override void OnInit()
        {
            UIRoot.Singleton.OpenPanel<GameHomePanel>();
        }

        protected override void OnDispose()
        {
            UIRoot.Singleton.ClosePanel<GameHomePanel>();
        }
    }
}