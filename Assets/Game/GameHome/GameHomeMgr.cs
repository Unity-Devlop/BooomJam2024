using FMOD.Studio;
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
            PlayBGM();
        }

        protected override void OnDispose()
        {
            UIRoot.Singleton.ClosePanel<GameHomePanel>();
            StopBGM();
        }

        public void PlayBGM()
        {
            Global.Get<AudioSystem>().PlaySingleton(FMODName.Event.MX_NORMAL_DEMO1);
        }

        public void StopBGM()
        {
            Global.Get<AudioSystem>().StopSingleton(FMODName.Event.MX_NORMAL_DEMO1, STOP_MODE.ALLOWFADEOUT);
        }
    }
}