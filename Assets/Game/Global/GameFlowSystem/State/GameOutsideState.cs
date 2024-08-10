using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game
{
    public class GameOutsideState : IState<GameFlow>
    {
        public void OnInit(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();

            // owner.ToOutsideScene(); // TODO: Implement this
        }

        public async void OnEnter(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
            // Global.Get<DataSystem>().Add(new PlayerData());
            await owner.ToGameOutsideScene();
            // 拿到要进入的小状态
            Type type = stateMachine.GetParam<Type>(Consts.GamePlayOutsideStateType);
            stateMachine.RemoveParam(Consts.GamePlayOutsideStateType);


            // 将数据写入自己的状态机 并且移除全局状态机的数据
            if (stateMachine.ContainsParam(Consts.BattleSettlementData))
            {
                BattleSettlementData settlementData =
                    stateMachine.GetParam<BattleSettlementData>(Consts.BattleSettlementData);
                stateMachine.RemoveParam(Consts.BattleSettlementData);
                GamePlayOutsideMgr.Singleton.machine.SetParam(Consts.BattleSettlementData, settlementData);
            }

            await UniTask.WaitUntil(() => GamePlayOutsideMgr.Singleton != null,
                cancellationToken: Global.Singleton.destroyCancellationToken);
            Assert.IsNotNull(GamePlayOutsideMgr.Singleton);
            Assert.IsNotNull(GamePlayOutsideMgr.Singleton.machine);
            GamePlayOutsideMgr.Singleton.machine.Change(type);
        }

        public void OnUpdate(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
        }

        public void OnExit(GameFlow owner, IStateMachine<GameFlow> stateMachine)
        {
            // throw new System.NotImplementedException();
            // 关闭外部场景还有UI
        }
    }
}