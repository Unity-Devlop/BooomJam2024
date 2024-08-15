using UnityToolkit;

namespace Game
{
    public class DailyTrainState : IState<GamePlayOutsideMgr>
    {
        public void OnInit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public async void OnEnter(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            DailyTrainPanel panel = await UIRoot.Singleton.OpenPanelAsync<DailyTrainPanel>();
            panel.Bind(Global.Get<DataSystem>().Get<GameData>().playerData);
        }

        public void OnUpdate(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public void OnExit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            if (UIRoot.Singleton.GetOpenedPanel(out DailyTrainPanel panel))
            {
                panel.UnBind();
                UIRoot.Singleton.ClosePanel<DailyTrainPanel>();
            }
        }
    }
}