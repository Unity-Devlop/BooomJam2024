using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.GamePlay
{
    public class BattleController : MonoBehaviour, IBattleController
    {
        public BattlePosition playerPos;
        public BattlePosition robotPos;

        private CancellationTokenSource _cts;
        
        public void Init(ITrainer self,ITrainer enemy)
        {
            playerPos.trainer = self;
            robotPos.trainer = enemy;
        }

        public UniTask Enter()
        {
            Assert.IsNull(_cts);
            _cts = new CancellationTokenSource();
            RoundFlow(this, _cts.Token).Forget();
            return UniTask.CompletedTask;
        }

        public UniTask RoundStart()
        {
            // throw new System.NotImplementedException();

            return UniTask.CompletedTask;
        }

        public UniTask BeforeRound()
        {
            // throw new System.NotImplementedException();

            return UniTask.CompletedTask;
        }

        public UniTask Rounding()
        {
            // throw new System.NotImplementedException();

            return UniTask.CompletedTask;
        }

        public UniTask AfterRound()
        {
            // throw new System.NotImplementedException();

            return UniTask.CompletedTask;
        }

        public UniTask RoundEnd()
        {
            // throw new System.NotImplementedException();

            return UniTask.CompletedTask;
        }


        public UniTask Exit()
        {
            return UniTask.CompletedTask;
        }

        public void Cancel()
        {
            Assert.IsNotNull(_cts);
            _cts.Cancel();
            _cts = null;
        }

        public bool TryGetWinner(out ITrainer trainer)
        {
            throw new NotImplementedException();
        }

        public static async UniTask RoundFlow(IBattleController controller, CancellationToken token)
        {
            ITrainer winner;
            while (!token.IsCancellationRequested)
            {
                await controller.RoundStart(); // 回合开始
                await controller.BeforeRound(); // 回合开始前
                await controller.Rounding(); // 回合进行
                if (controller.TryGetWinner(out winner))
                {
                    break;
                }

                await
                    controller.AfterRound();
                if (controller.TryGetWinner(out winner))
                {
                    break;
                }

                await controller.RoundEnd();
            }

            // 执行退出战斗流程
            await controller.Exit();
        }
    }
}