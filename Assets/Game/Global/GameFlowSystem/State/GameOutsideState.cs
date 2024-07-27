using UnityToolkit;

namespace Game
{
    public class GameOutsideState :  IState<GameFlow>
    {
        public void OnInit(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
            owner.ToGameOutsideScene();
            // owner.ToOutsideScene(); // TODO: Implement this
        }

        public void OnEnter(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
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