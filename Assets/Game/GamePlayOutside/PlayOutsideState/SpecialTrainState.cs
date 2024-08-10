using UnityToolkit;

namespace Game
{
    public class SpecialTrainState : IState<GamePlayOutsideMgr>
    {
        public void OnInit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public void OnEnter(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            UIRoot.Singleton.OpenPanel<SpecialTrainPanel>();
        }

        public void OnUpdate(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
        }

        public async void OnExit(GamePlayOutsideMgr owner, IStateMachine<GamePlayOutsideMgr> stateMachine)
        {
            UIRoot.Singleton.ClosePanel<SpecialTrainPanel>();
        }
    }
}