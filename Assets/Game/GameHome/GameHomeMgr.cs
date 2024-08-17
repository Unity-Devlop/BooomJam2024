using FMOD.Studio;
using UnityToolkit;

namespace Game.GameHome
{
    public class GameHomeMgr : MonoSingleton<GameHomeMgr>
    {
        protected override async void OnInit()
        {
            await UIRoot.Singleton.OpenPanelAsync<GameHomePanel>();
            PlayBGM();
        }

        protected override void OnDispose()
        {
            UIRoot.Singleton.ClosePanel<GameHomePanel>();
            StopBGM();
        }

        public void PlayBGM()
        {
            Global.Get<AudioSystem>().PlaySingleton(FMODName.Event.MX_MX_MAINSCREEN_LOOP);
        }

        public void StopBGM()
        {
            Global.Get<AudioSystem>().StopSingleton(FMODName.Event.MX_MX_MAINSCREEN_LOOP, STOP_MODE.ALLOWFADEOUT);
        }
    }
}