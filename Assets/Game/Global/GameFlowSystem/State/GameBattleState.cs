using Cysharp.Threading.Tasks;
using UnityToolkit;

namespace Game
{
    public class GameBattleState : IState<GameFlow>
    {
        public void OnInit(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();

        }

        public async void OnEnter(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
            // BattleData battleData = stateMachine.GetParam<BattleData>(nameof(BattleData));
            await owner.ToGameBattleScene();
            
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