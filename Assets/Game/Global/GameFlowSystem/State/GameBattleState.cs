using Cysharp.Threading.Tasks;
using Game.GamePlay;
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
            BattleData battleData = stateMachine.GetParam<BattleData>(Consts.GameBattleData);
            stateMachine.RemoveParam(Consts.GameBattleData);
            TrainerData trainerData = stateMachine.GetParam<TrainerData>(Consts.LocalPlayerTrainerData);
            stateMachine.RemoveParam(Consts.LocalPlayerTrainerData);
            TrainerData robotData = stateMachine.GetParam<TrainerData>(Consts.EnemyTrainerData);
            stateMachine.RemoveParam(Consts.EnemyTrainerData);
            GameBattleMgr.Singleton.StartBattle(trainerData, robotData, battleData).Forget();

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