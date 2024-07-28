using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game.GamePlay
{
    public class DummyBattleFlow : MonoBehaviour, IBattleFlow
    {
        public PlayerBattleTrainer self;
        public RebotBattleTrainer enemy;

        public BattlePosition selfPos;
        public BattlePosition enemyPos;

        private CancellationTokenSource _cts;

        public void Init(PlayerBattleTrainer self, RebotBattleTrainer enemy)
        {
            this.self = self;
            this.enemy = enemy;
            selfPos.battleTrainer = self;
            enemyPos.battleTrainer = enemy;
            // Global.Event.Listen<OnActiveCardConsume>(OnConsumeCard);
        }

        // private void OnConsumeCard(OnActiveCardConsume obj)
        // {
        //     self.OnConsume(obj);
        //     enemy.OnConsume(obj);
        // }

        public void Dispose()
        {
            _cts.Cancel();
            _cts = null;
            // Global.Event.UnListen<OnActiveCardConsume>(OnConsumeCard);
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

            selfPos.prepareData = selfPos.battleTrainer.Get(0);
            enemyPos.prepareData = selfPos.battleTrainer.Get(0);

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

            if (enemyPos.prepareData != null)
            {
                enemyPos.currentData = enemyPos.prepareData;
                enemyPos.prepareData = null;
                await enemyPos.ExecuteEnter();
                await enemy.ChangeHulu(enemyPos.currentData);
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

        public async UniTask Rounding()
        {
            // throw new System.NotImplementedException();

            IBattleOperation selfOper = default;
            IBattleOperation enemyOper = default;
            Debug.Log("Rounding");
            while (_cts is { IsCancellationRequested: false })
            {
                // 等待双方操作
                if (selfOper is not EndRoundOperation)
                {
                    selfOper = await self.CalOperation();
                }

                if (enemyOper is not EndRoundOperation)
                {
                    enemyOper = await enemy.CalOperation();
                }

                // 双方都结束回合 则进入下一阶段
                if (selfOper is EndRoundOperation && enemyOper is EndRoundOperation)
                {
                    break;
                }


                Debug.Log($"Self Oper: {selfOper},Enemy Oper: {enemyOper}");

                if (selfOper is ActiveSkillBattleOperation atk && enemyOper is ActiveSkillBattleOperation enemyAtk)
                {
                    // 计算流程
                    await self.OnConsumeSkill(atk.data); // 消耗牌
                    await enemy.OnConsumeSkill(enemyAtk.data); // 消耗牌

                    // 牌都消耗了再结算
                    Debug.LogWarning($"使用技能逻辑 {atk.data} {enemyAtk.data} 未实现");

                    var (faster, slower) = GameMath.WhoFirst(selfPos.currentData, enemyPos.currentData, atk.data,
                        enemyAtk.data);

                    await selfPos.ExecuteSkill();
                    await enemyPos.ExecuteSkill();

                    // TODO 如果打了 特殊的牌 则不能够再打牌
                    continue; // 这里双方技能都结算了 所以直接跳到下一回合
                }

                // TODO 如果同时切换 或许有先后问题 但是和AI玩不用管 自己先切换
                if (selfOper is ChangeHuluOperation selfChange)
                {
                    Debug.LogWarning($"Self,切换逻辑 {selfChange} 未实现 直接结束回合");
                    selfOper = new EndRoundOperation();
                }

                if (enemyOper is ChangeHuluOperation enemyChange)
                {
                    Debug.LogWarning($"Enemy,切换逻辑 {enemyChange} 未实现 直接结束回合");
                    enemyOper = new EndRoundOperation();
                }
            }
        }

        public UniTask AfterRound()
        {
            // throw new System.NotImplementedException();

            return UniTask.CompletedTask;
        }

        public async UniTask RoundEnd()
        {
            // throw new System.NotImplementedException();
            // await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
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

        public bool TryGetRoundWinner(out IBattleTrainer battleTrainer)
        {
            if (self.currentData.hp <= 0)
            {
                battleTrainer = enemy;
                return true;
            }

            if (enemy.currentData.hp <= 0)
            {
                battleTrainer = self;
                return true;
            }

            battleTrainer = null;
            return false;
        }

        public bool TryGetFinalWinner(out IBattleTrainer battleTrainer)
        {
            if (self.canFight && !enemy.canFight)
            {
                battleTrainer = self;
                return true;
            }

            if (!self.canFight && enemy.canFight)
            {
                battleTrainer = enemy;
                return true;
            }

            battleTrainer = null;
            return false;
        }


        public static async UniTask RoundFlow(IBattleFlow flow, CancellationToken token)
        {
            IBattleTrainer winner;
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