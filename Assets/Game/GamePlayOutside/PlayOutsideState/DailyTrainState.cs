using UnityToolkit;

namespace Game
{
    public class DailyTrainState : IState<GamePlayOutsideMgr>
    {
        public void OnInit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public void OnEnter(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            DailyTrainPanel panel = UIRoot.Singleton.OpenPanel<DailyTrainPanel>();
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