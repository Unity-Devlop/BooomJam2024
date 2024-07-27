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
            Global.Event.Listen<OnActiveCardConsume>(OnConsumeCard);
        }

        private void OnConsumeCard(OnActiveCardConsume obj)
        {
            self.OnConsume(obj);
            enemy.OnConsume(obj);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts = null;
            Global.Event.UnListen<OnActiveCardConsume>(OnConsumeCard);
            UIRoot.Singleton.ClosePanel<GameBattlePanel>();
        }
        public UniTask Enter()
        {
            // Roll Initial Cards

            // Open Battle Panel
            GameBattlePanel gameBattlePanel = UIRoot.Singleton.OpenPanel<GameBattlePanel>();
            gameBattlePanel.Bind(self);

            Assert.IsNull(_cts);
            _cts = new CancellationTokenSource();

            selfPos.prepareData = selfPos.trainer.Get(0);
            robotPos.prepareData = selfPos.trainer.Get(0);

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
                Debug.Log("EnterBattleCheck Self");
                selfPos.currentData = selfPos.prepareData;
                selfPos.prepareData = null;
                await selfPos.ExecuteEnter();
                await self.ChangeHulu(selfPos.currentData);
            }

            if (robotPos.prepareData != null)
            {
                robotPos.currentData = robotPos.prepareData;
                robotPos.prepareData = null;
                await robotPos.ExecuteEnter();
                await enemy.ChangeHulu(robotPos.currentData);
            }
        }

        public async UniTask RoundStart()
        {
            // throw new System.NotImplementedException();
            // 执行入场逻辑
            await EnterBattleCheck();

            // 各自抽卡
            await self.DrawSkills(1);
            await enemy.DrawSkills(1);
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

        public async UniTask RoundEnd()
        {
            // throw new System.NotImplementedException();
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        }


        public UniTask Exit()
        {
            if (UIRoot.Singleton.GetOpenedPanel(out GameBattlePanel battlePanel))
            {
                battlePanel.UnBind();
            }

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
            if (self.currentData.hp <= 0)
            {
                trainer = enemy;
                return true;
            }

            if (enemy.currentData.hp <= 0)
            {
                trainer = self;
                return true;
            }

            trainer = null;
            return false;
        }

        public bool TryGetFinalWinner(out ITrainer trainer)
        {
            if (self.canFight && !enemy.canFight)
            {
                trainer = self;
                return true;
            }

            if (!self.canFight && enemy.canFight)
            {
                trainer = enemy;
                return true;
            }

            trainer = null;
            return false;
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

                await UniTask.DelayFrame(1, cancellationToken: token);
            }

            // 执行退出战斗流程
            await flow.Exit();
        }
    }
}