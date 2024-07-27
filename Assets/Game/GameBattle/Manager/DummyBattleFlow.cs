using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game.GamePlay
{
    public class DummyBattleFlow : MonoBehaviour, IBattleFlow
    {
        public PlayerTrainer self;
        public RebotTrainer enemy;
        
        public BattlePosition selfPos;
        public BattlePosition robotPos;

        private CancellationTokenSource _cts;

        public void Init(PlayerTrainer self, RebotTrainer enemy)
        {
            this.self = self;
            this.enemy = enemy;
            selfPos.trainer = self;
            robotPos.trainer = enemy;
        }

        public UniTask Enter()
        {
            // Roll Initial Cards

            // Open Battle Panel
            GameBattlePanel gameBattlePanel = UIRoot.Singleton.OpenPanel<GameBattlePanel>();
            gameBattlePanel.Bind(self);

            Assert.IsNull(_cts);
            _cts = new CancellationTokenSource();
            RoundFlow(this, _cts.Token).Forget();
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 检查是否有角色准备入场
        /// </summary>
        private async UniTask EnterBattleCheck()
        {
            if (selfPos.prepareData != null)
            {
                selfPos.currentData = selfPos.prepareData;
                selfPos.prepareData = null;
                await selfPos.ExecuteEnter();
                
                self.ChangeHulu(selfPos.currentData);
                self.DrawSkills();
            }

            if (robotPos.prepareData != null)
            {
                robotPos.currentData = robotPos.prepareData;
                robotPos.prepareData = null;
                await robotPos.ExecuteEnter();

                //删卡 抽卡
                enemy.ChangeHulu(robotPos.currentData);
                enemy.DrawSkills();
            }
        }

        public async UniTask RoundStart()
        {
            // throw new System.NotImplementedException();
            // 执行入场逻辑
            await EnterBattleCheck();
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

        public bool TryGetRoundWinner(out ITrainer trainer)
        {
            throw new NotImplementedException();
        }

        public bool TryGetFinalWinner(out ITrainer trainer)
        {
            throw new NotImplementedException();
        }

        public static async UniTask RoundFlow(IBattleFlow flow, CancellationToken token)
        {
            ITrainer winner;
            while (!token.IsCancellationRequested)
            {
                await flow.RoundStart(); // 回合开始
                await flow.BeforeRound(); // 回合开始前
                await flow.Rounding(); // 回合进行
                if (flow.TryGetFinalWinner(out winner))
                {
                    break;
                }

                await
                    flow.AfterRound();
                if (flow.TryGetFinalWinner(out winner))
                {
                    break;
                }

                await flow.RoundEnd();
            }

            // 执行退出战斗流程
            await flow.Exit();
        }

        public void Dispose()
        {
            UIRoot.Singleton.ClosePanel<GameBattlePanel>();
        }
    }
}