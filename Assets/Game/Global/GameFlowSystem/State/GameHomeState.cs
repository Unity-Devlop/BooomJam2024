using UnityToolkit;

namespace Game
{
    public class GameHomeState : IState<GameFlow>

    {
        public void OnInit(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
        }

        public void OnEnter(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
            owner.ToGameHomeScene();
        }

        public void OnUpdate(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
        }

        public void OnExit(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
        }
    }
}