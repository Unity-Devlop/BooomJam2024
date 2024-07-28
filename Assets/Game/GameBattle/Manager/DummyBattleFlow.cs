using System;
using System.Runtime.CompilerServices;
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
        private BattleEnvironmentData _environmentData;

        public void Init(PlayerBattleTrainer self, RebotBattleTrainer enemy, BattleEnvironmentData environmentData)
        {
            _self = self;
            _enemy = enemy;

            _environmentData = environmentData;
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

            selfPos.SetNext(selfPos.battleTrainer.Get(0));
            enemyPos.SetNext(enemyPos.battleTrainer.Get(0));

            RoundFlow(this, _cts.Token).Forget();
            return UniTask.CompletedTask;
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
                if (!selfPos.CanFight() || !enemyPos.CanFight())
                {
                    Debug.Log($"有人不能战斗了");
                    break;
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
                    // Debug.LogWarning("双方结束回合");
                    break;
                }


                Debug.Log($"Self Oper: {selfOper},Enemy Oper: {enemyOper}");

                if (selfOper is ActiveSkillBattleOperation selfAtk && enemyOper is ActiveSkillBattleOperation enemyAtk)
                {
                    Assert.IsNotNull(selfAtk.data);
                    Assert.IsNotNull(enemyAtk.data);
                    if (selfAtk.data.config.Type != ActiveSkillTypeEnum.指挥 &&
                        enemyAtk.data.config.Type != ActiveSkillTypeEnum.指挥)
                    {
                        await BothPokemonSkill(selfAtk, enemyAtk);
                        // 放了技能后 回合自动结束
                        selfOper = new EndRoundOperation();
                        enemyOper = new EndRoundOperation();
                        continue;
                    }

                    if (selfAtk.data.config.Type == ActiveSkillTypeEnum.指挥 &&
                        enemyAtk.data.config.Type != ActiveSkillTypeEnum.指挥)
                    {
                        await ExecuteSkill(_self, selfPos, selfAtk);
                        await ExecuteSkill(_enemy, enemyPos, enemyAtk);
                        enemyOper = default(EndRoundOperation);
                        continue;
                    }

                    if (enemyAtk.data.config.Type == ActiveSkillTypeEnum.指挥 &&
                        selfAtk.data.config.Type != ActiveSkillTypeEnum.指挥)
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
                    if (enemyAtk1.data.config.Type != ActiveSkillTypeEnum.指挥)
                    {
                        enemyOper = new EndRoundOperation();
                    }

                    continue;
                }

                if (selfOper is ActiveSkillBattleOperation selfAtk1 && enemyOper is EndRoundOperation)
                {
                    await ExecuteSkill(_self, selfPos, selfAtk1);
                    if (selfAtk1.data.config.Type != ActiveSkillTypeEnum.指挥)
                    {
                        selfOper = new EndRoundOperation();
                    }

                    continue;
                }


                // TODO 如果同时切换 或许有先后问题 但是和AI玩不用管 自己先切换
                if (selfOper is ChangeHuluOperation selfChange1 && enemyOper is ChangeHuluOperation enemyChange1)
                {
                    await ExecuteSwitch(_self, selfPos, selfChange1.next);
                    selfOper = new EndRoundOperation();
                    await ExecuteSwitch(_enemy, enemyPos, enemyChange1.next);
                    enemyOper = new EndRoundOperation();
                    continue;
                }

                if (selfOper is ChangeHuluOperation selfChange2 && enemyOper is ActiveSkillBattleOperation enemyAtk2)
                {
                    await ExecuteSwitch(_self, selfPos, selfChange2.next);
                    selfOper = new EndRoundOperation();
                    await ExecuteSkill(_enemy, enemyPos, enemyAtk2);
                    continue;
                }

                if (selfOper is ActiveSkillBattleOperation selfAtk3 && enemyOper is ChangeHuluOperation enemyChange2)
                {
                    await ExecuteSwitch(_enemy, enemyPos, enemyChange2.next);
                    enemyOper = new EndRoundOperation();
                    await ExecuteSkill(_self, selfPos, selfAtk3);
                    continue;
                }

                throw new NotImplementedException(
                    $"Self Oper: {selfOper.GetType()},Enemy Oper: {enemyOper.GetType()} NotImplemented");
            }
        }


        public UniTask AfterRound()
        {
            if (!selfPos.CanFight() && _self.trainerData.FindFirstCanFight(out HuluData selfNext))
            {
                selfPos.SetNext(selfNext);
            }

            if (!enemyPos.CanFight() && _enemy.trainerData.FindFirstCanFight(out HuluData enemyNext))
            {
                enemyPos.SetNext(enemyNext);
            }

            // 如果有一方G了 则进行自动替换逻辑
            return UniTask.CompletedTask;
        }

        public async UniTask RoundEnd()
        {
            await selfPos.ClearRoundData();
            await enemyPos.ClearRoundData();
        }


        public UniTask Exit()
        {
            Debug.Log("Exit");
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
            if (_self.currentData.HealthIsZero())
            {
                battleTrainer = _enemy;
                return true;
            }

            if (_enemy.currentData.HealthIsZero())
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

                await flow.AfterRound();
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

        /// <summary>
        /// 检查是否有角色准备入场
        /// </summary>
        private async UniTask EnterBattleCheck()
        {
            if (selfPos.next != null)
            {
                Debug.Log("EnterBattleCheck Self");
                await selfPos.Prepare2Current();
                await selfPos.ExecuteEnter();
                await _self.ChangeCurrentHulu(selfPos.currentData);
            }

            if (enemyPos.next != null)
            {
                await enemyPos.Prepare2Current();
                await enemyPos.ExecuteEnter();
                await _enemy.ChangeCurrentHulu(enemyPos.currentData);
            }
        }


        private async UniTask ExecuteSwitch(IBattleTrainer trainer, BattlePosition position, int idx)
        {
            HuluData next = trainer.trainerData.datas[idx];
            position.SetNext(next);
            await position.Prepare2Current();
            await position.ExecuteEnter();
            await trainer.ChangeCurrentHulu(next);
        }

        private async UniTask BothPokemonSkill(ActiveSkillBattleOperation selfAtk, ActiveSkillBattleOperation enemyAtk)
        {
            var (faster, _) = GameMath.WhoFirst(selfPos.currentData, enemyPos.currentData, selfAtk.data,
                enemyAtk.data, _environmentData);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async UniTask ExecuteSkill(IBattleTrainer trainer, BattlePosition position,
            ActiveSkillBattleOperation operation)
        {
            Assert.IsTrue(position.CanFight());
            await trainer.OnConsumeSkill(operation.data);
            await position.ExecuteSkill(operation);
            // 计算伤害

            HuluData atk = position.currentData;
            HuluData def;
            if (position == selfPos)
            {
                def = enemyPos.currentData;
            }
            else
            {
                def = selfPos.currentData;
            }
            if (operation.data.config.Type == ActiveSkillTypeEnum.伤害技能)
            {
                int damage = GameMath.CalDamage(atk, def, operation.data.id, _environmentData);
                // TODO 特殊技能有待实现
                await def.ChangeHealth(damage);
                Debug.Log($"计算技能伤害,pos:{position},{atk}对{def}使用{operation.data.id} 造成{damage}伤害");
                return;
            }

            if (operation.data.config.Type == ActiveSkillTypeEnum.变化技能)
            {
                if (operation.data.id == ActiveSkillEnum.守护)
                {
                    return;
                }
                if(operation.data.id.
            }
        }
    }
}