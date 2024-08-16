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
            UIRoot.Singleton.OpenPanel<LoadingPanel>();
            owner.ToGameBattleScene();

            BattleEnvData battleEnvData = stateMachine.GetParam<BattleEnvData>(Consts.GameBattleData);
            stateMachine.RemoveParam(Consts.GameBattleData);
            TrainerData trainerData = stateMachine.GetParam<TrainerData>(Consts.LocalPlayerTrainerData);
            stateMachine.RemoveParam(Consts.LocalPlayerTrainerData);
            TrainerData robotData = stateMachine.GetParam<TrainerData>(Consts.EnemyTrainerData);
            stateMachine.RemoveParam(Consts.EnemyTrainerData);

            await UniTask.WaitUntil(() => GameBattleMgr.SingletonNullable != null);
            UIRoot.Singleton.ClosePanel<LoadingPanel>();
            GameBattleMgr.Singleton.StartBattle(trainerData, robotData, battleEnvData);
        }

        public void OnUpdate(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
        }

        public void OnExit(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
            // GameBattleMgr.Singleton.OnDispose();
        }
    }
}