using System;
using System.Threading;
using cfg;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game.GamePlay
{
    public class DummyBattleFlow : MonoBehaviour, IBattleFlow
    {
        [ReadOnly, NonSerialized, ShowInInspector]
        private PlayerBattleTrainer _self;

        [ReadOnly, NonSerialized, ShowInInspector]
        private RebotBattleTrainer _enemy;

        [HorizontalGroup("1")] public BattlePosition selfPos;

        [HorizontalGroup("1")] public BattlePosition enemyPos;

        private CancellationTokenSource _cts;

        public void Init(PlayerBattleTrainer self, RebotBattleTrainer enemy)
        {
            this._self = self;
            this._enemy = enemy;
            selfPos.battleTrainer = self;
            enemyPos.battleTrainer = enemy;
        }


        public void Dispose()
        {
            _cts.Cancel();
            _cts = null;
            UIRoot.Singleton.ClosePanel<GameBattlePanel>();
        }

        public UniTask Enter()
        {
            // Roll Initial Cards

            // Open Battle Panel
            GameBattlePanel gameBattlePanel = UIRoot.Singleton.OpenPanel<GameBattlePanel>();
            gameBattlePanel.Bind(_self);

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
                await _self.ChangeHulu(selfPos.currentData);
            }

            if (enemyPos.prepareData != null)
            {
                enemyPos.currentData = enemyPos.prepareData;
                enemyPos.prepareData = null;
                await enemyPos.ExecuteEnter();
                await _enemy.ChangeHulu(enemyPos.currentData);
            }
        }

        public async UniTask RoundStart()
        {
            // throw new System.NotImplementedException();
            // 执行入场逻辑
            await EnterBattleCheck();

            // 各自抽卡
            await _self.DrawSkills(1);
            await _enemy.DrawSkills(1);
        }

        public UniTask BeforeRound()
        {
            // throw new System.NotImplementedException();
            return UniTask.CompletedTask;
        }

        private async UniTask BothPokemonSkill(ActiveSkillBattleOperation selfAtk, ActiveSkillBattleOperation enemyAtk)
        {
            var (faster, _) = GameMath.WhoFirst(selfPos.currentData, enemyPos.currentData, selfAtk.data,
                enemyAtk.data);

            // 根据顺序结算
            if (faster == selfPos.currentData)
            {
                await ExecuteSkill(_self, selfPos, selfAtk);
                if (!enemyPos.CanFight())
                {
                    return;
                }

                // 如果打死了对方 则不用再打了
                await ExecuteSkill(_enemy, enemyPos, enemyAtk);
            }
            else
            {
                await ExecuteSkill(_enemy, enemyPos, enemyAtk);
                if (!selfPos.CanFight())
                {
                    return;
                }

                // 如果打死了对方 则不用再打了
                await ExecuteSkill(_self, selfPos, selfAtk);
            }
        }

        private async UniTask ExecuteSkill(IBattleTrainer trainer, BattlePosition position,
            ActiveSkillBattleOperation operation)
        {
            await trainer.OnConsumeSkill(operation.data);
            await position.ExecuteSkill(operation);
        }

        public async UniTask Rounding()
        {
            // throw new System.NotImplementedException();

            IBattleOperation selfOper = null;
            IBattleOperation enemyOper = null;
            Debug.Log("Rounding");
            while (_cts is { IsCancellationRequested: false })
            {
                await UniTask.DelayFrame(1);

                // 有人不能战斗了
                if (!selfPos.CanFight())
                {
                    Debug.Log("Self 不能战斗了");
                    selfOper = new EndRoundOperation();
                }

                if (!enemyPos.CanFight())
                {
                    Debug.Log("Enemy 不能战斗了");
                    enemyOper = new EndRoundOperation();
                }

                // 等待双方操作
                if (selfOper is not EndRoundOperation)
                {
                    _self.ClearOperation();
                    selfOper = await _self.CalOperation();
                }

                if (enemyOper is not EndRoundOperation)
                {
                    _enemy.ClearOperation();
                    enemyOper = await _enemy.CalOperation();
                }

                // 双方都结束回合 则进入下一阶段
                if (selfOper is EndRoundOperation && enemyOper is EndRoundOperation)
                {
                    Debug.Log("双方结束回合");
                    break;
                }


                Debug.Log($"Self Oper: {selfOper},Enemy Oper: {enemyOper}");

                if (selfOper is ActiveSkillBattleOperation selfAtk && enemyOper is ActiveSkillBattleOperation enemyAtk)
                {
                    Assert.IsNotNull(selfAtk.data);
                    Assert.IsNotNull(enemyAtk.data);
                    if (selfAtk.data.config.Type == ActiveSkillTypeEnum.技能 &&
                        enemyAtk.data.config.Type == ActiveSkillTypeEnum.技能)
                    {
                        await BothPokemonSkill(selfAtk, enemyAtk);
                        // 放了技能后 回合自动结束
                        selfOper = new EndRoundOperation();
                        enemyOper = new EndRoundOperation();
                        continue;
                    }

                    if (selfAtk.data.config.Type == ActiveSkillTypeEnum.指挥 &&
                        enemyAtk.data.config.Type == ActiveSkillTypeEnum.技能)
                    {
                        await ExecuteSkill(_self, selfPos, selfAtk);
                        await ExecuteSkill(_enemy, enemyPos, enemyAtk);
                        enemyOper = default(EndRoundOperation);
                        continue;
                    }

                    if (enemyAtk.data.config.Type == ActiveSkillTypeEnum.指挥 &&
                        selfAtk.data.config.Type == ActiveSkillTypeEnum.技能)
                    {
                        await ExecuteSkill(_enemy, enemyPos, enemyAtk);
                        await ExecuteSkill(_self, selfPos, selfAtk);
                        selfOper = default(EndRoundOperation);
                        continue;
                    }

                    // 都是指挥技能
                    await ExecuteSkill(_self, selfPos, selfAtk);
                    await ExecuteSkill(_enemy, enemyPos, enemyAtk);

                    // TODO 如果打了 特殊的牌 则不能够再打牌
                    continue; // 这里双方技能都结算了 所以直接跳到下一回合
                }

                if (selfOper is EndRoundOperation && enemyOper is ActiveSkillBattleOperation enemyAtk1)
                {
                    await ExecuteSkill(_enemy, enemyPos, enemyAtk1);
                    if (enemyAtk1.data.config.Type == ActiveSkillTypeEnum.技能)
                    {
                        enemyOper = new EndRoundOperation();
                    }

                    continue;
                }

                if (selfOper is ActiveSkillBattleOperation selfAtk1 && enemyOper is EndRoundOperation)
                {
                    await ExecuteSkill(_self, selfPos, selfAtk1);
                    if (selfAtk1.data.config.Type == ActiveSkillTypeEnum.技能)
                    {
                        selfOper = new EndRoundOperation();
                    }

                    continue;
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
            if (_self.currentData.hp <= 0)
            {
                battleTrainer = _enemy;
                return true;
            }

            if (_enemy.currentData.hp <= 0)
            {
                battleTrainer = _self;
                return true;
            }

            battleTrainer = null;
            return false;
        }

        public bool TryGetFinalWinner(out IBattleTrainer battleTrainer)
        {
            if (_self.canFight && !_enemy.canFight)
            {
                battleTrainer = _self;
                return true;
            }

            if (!_self.canFight && _enemy.canFight)
            {
                battleTrainer = _enemy;
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