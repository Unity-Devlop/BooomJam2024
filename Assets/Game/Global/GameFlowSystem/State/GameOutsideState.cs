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
            
            await UniTask.WaitUntil(() => GamePlayOutsideMgr.Singleton != null,
                cancellationToken: Global.Singleton.destroyCancellationToken);
            Assert.IsNotNull(GamePlayOutsideMgr.Singleton);
            Assert.IsNotNull(GamePlayOutsideMgr.Singleton.machine);
            if (GamePlayOutsideMgr.Singleton.machine.running)
            {
                GamePlayOutsideMgr.Singleton.machine.Change(type);
            }
            else
            {
                GamePlayOutsideMgr.Singleton.machine.Run(type);
            }
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