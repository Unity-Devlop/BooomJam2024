﻿using System;
using System.Collections.Generic;
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

        public IBattleOperation selfOper = null;
        public IBattleOperation enemyOper = null;

        private CancellationTokenSource _cts;
        private BattleEnvironmentData _environmentData;

        public void Init(PlayerBattleTrainer self, RebotBattleTrainer enemy, BattleEnvironmentData environmentData)
        {
            Assert.IsNull(_cts);
            _self = self;
            _enemy = enemy;

            _environmentData = environmentData;
            _environmentData.AddTrainer(_self);
            _environmentData.AddTrainer(_enemy);

            selfPos.battleTrainer = self;
            enemyPos.battleTrainer = enemy;
        }


        public void Dispose()
        {
            _cts.Cancel();
            _cts = null;
            UIRoot.Singleton.ClosePanel<GameBattlePanel>();
        }

        public async UniTask Enter()
        {
            Assert.IsNull(_cts);
            // Roll Initial Cards

            // Open Battle Panel
            GameBattlePanel gameBattlePanel = UIRoot.Singleton.OpenPanel<GameBattlePanel>();
            gameBattlePanel.Bind(_self);
            _cts = new CancellationTokenSource();

            selfPos.SetNext(selfPos.battleTrainer.Get(0));
            enemyPos.SetNext(enemyPos.battleTrainer.Get(0));

            await IBattleFlow.RoundFlow(this, _cts.Token);
        }

        public async UniTask RoundStart()
        {
            // throw new System.NotImplementedException();
            // 执行入场逻辑
            await EnterBattleCheck();
            var selfBuffs = _environmentData.GetBuff(_self);
            await ExecuteBuffBeforeRound(_self, _enemy, selfBuffs);
            var enemyBuffs = _environmentData.GetBuff(_enemy);
            await ExecuteBuffBeforeRound(_enemy, _self, enemyBuffs);
        }

        public async UniTask BeforeRound()
        {
            // throw new System.NotImplementedException();
            // 各自抽卡
            await _self.DrawSkills(1);
            await _enemy.DrawSkills(1);
        }


        public async UniTask Rounding()
        {
            // throw new System.NotImplementedException();
            selfOper = null;
            enemyOper = null;

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


                // TODO 重写 


                if (selfOper is ActiveSkillBattleOperation selfAtk && enemyOper is ActiveSkillBattleOperation enemyAtk)
                {
                    Assert.IsNotNull(selfAtk.data);
                    Assert.IsNotNull(enemyAtk.data);
                    if (selfAtk.data.config.Type != ActiveSkillTypeEnum.指挥 &&
                        enemyAtk.data.config.Type != ActiveSkillTypeEnum.指挥)
                    {
                        await BothPokemonSkill(selfAtk, enemyAtk);
                        // 放了技能后 回合自动结束
                        continue;
                    }

                    await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk);
                    await ExecuteSkill(_enemy, _self, enemyPos, selfPos, enemyAtk);
                    continue; // 这里双方技能都结算了 所以直接跳到下一回合
                }

                if (selfOper is EndRoundOperation && enemyOper is ActiveSkillBattleOperation enemyAtk1)
                {
                    await ExecuteSkill(_enemy, _self, enemyPos, selfPos, enemyAtk1);
                    continue;
                }

                if (selfOper is ActiveSkillBattleOperation selfAtk1 && enemyOper is EndRoundOperation)
                {
                    await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk1);
                    continue;
                }


                // TODO 如果同时切换 或许有先后问题 但是和AI玩不用管 自己先切换
                if (selfOper is ChangeHuluOperation selfChange1 && enemyOper is ChangeHuluOperation enemyChange1)
                {
                    await ExecuteSwitch(_self, selfPos, selfChange1.next);
                    ModifyOperAfterChangeHulu(ref selfOper);
                    await ExecuteSwitch(_enemy, enemyPos, enemyChange1.next);
                    ModifyOperAfterChangeHulu(ref enemyOper);
                    continue;
                }

                if (selfOper is ChangeHuluOperation selfChange2 && enemyOper is ActiveSkillBattleOperation enemyAtk2)
                {
                    await ExecuteSwitch(_self, selfPos, selfChange2.next);
                    ModifyOperAfterChangeHulu(ref selfOper);
                    await ExecuteSkill(_enemy, _self, enemyPos, selfPos, enemyAtk2);
                    continue;
                }

                if (selfOper is ActiveSkillBattleOperation selfAtk3 && enemyOper is ChangeHuluOperation enemyChange2)
                {
                    await ExecuteSwitch(_enemy, enemyPos, enemyChange2.next);
                    ModifyOperAfterChangeHulu(ref enemyOper);
                    await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk3);
                    continue;
                }

                if (selfOper is ChangeHuluOperation selfChange && enemyOper is EndRoundOperation)
                {
                    await ExecuteSwitch(_self, selfPos, selfChange.next);
                    ModifyOperAfterChangeHulu(ref selfOper);
                    continue;
                }

                if (selfOper is EndRoundOperation && enemyOper is ChangeHuluOperation enemyChange)
                {
                    await ExecuteSwitch(_enemy, enemyPos, enemyChange.next);
                    ModifyOperAfterChangeHulu(ref enemyOper);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask RoundEnd()
        {
            await selfPos.ClearRoundData();
            await enemyPos.ClearRoundData();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask Exit()
        {
            Debug.Log("Exit");
            if (UIRoot.Singleton.GetOpenedPanel(out GameBattlePanel battlePanel))
            {
                battlePanel.UnBind();
            }

            return UniTask.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cancel()
        {
            Assert.IsNotNull(_cts);
            _cts.Cancel();
            _cts = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetRoundWinner(out IBattleTrainer battleTrainer)
        {
            if (_self.currentBattleData.HealthIsZero())
            {
                battleTrainer = _enemy;
                return true;
            }

            if (_enemy.currentBattleData.HealthIsZero())
            {
                battleTrainer = _self;
                return true;
            }

            battleTrainer = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ModifyOperAfterUseSkill(ref IBattleOperation oper)
        {
            Assert.IsTrue(oper is ActiveSkillBattleOperation);
            var atk = (ActiveSkillBattleOperation)oper;

            if (atk.data.config.Type != ActiveSkillTypeEnum.指挥)
            {
                oper = new EndRoundOperation();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ModifyOperAfterChangeHulu(ref IBattleOperation oper)
        {
            Assert.IsTrue(oper is ChangeHuluOperation);
            oper = new EndRoundOperation();
        }

        /// <summary>
        /// 检查是否有角色准备入场
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async UniTask EnterBattleCheck()
        {
            if (selfPos.next != null)
            {
                await ExecuteEnter(_self, selfPos, selfPos.next);
            }

            if (enemyPos.next != null)
            {
                await ExecuteEnter(_enemy, enemyPos, enemyPos.next);
            }
        }


        private async UniTask BothPokemonSkill(ActiveSkillBattleOperation selfAtk, ActiveSkillBattleOperation enemyAtk)
        {
            var (faster, slower) = GameMath.WhoFirst(selfPos.currentData, enemyPos.currentData, selfAtk.data,
                enemyAtk.data, _environmentData);

            // 根据顺序结算
            if (faster == selfPos.currentData)
            {
                await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk);
                if (!enemyPos.CanFight())
                {
                    return;
                }

                if (slower != _enemy.currentBattleData)
                {
                    Debug.Log("走了退场逻辑 不再放技能");
                    return;
                }

                // 如果打死了对方 则不用再打了
                await ExecuteSkill(_enemy, _self, enemyPos, selfPos, enemyAtk);
            }
            else
            {
                await ExecuteSkill(_enemy, _self, enemyPos, selfPos, enemyAtk);
                if (!selfPos.CanFight())
                {
                    return;
                }

                if (slower != _self.currentBattleData)
                {
                    Debug.Log("走了退场逻辑 不再放技能");
                    return;
                }

                // 如果打死了对方 则不用再打了
                await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async UniTask ExecuteBuffBeforeRound(IBattleTrainer positiveTrainer, IBattleTrainer negativeTrainer,
            BuffContainer container)
        {
            Assert.IsTrue(positiveTrainer != negativeTrainer);
            if (container.lastRoundBuffEnums.Contains(BuffEnum.起风))
            {
                Debug.Log("起风消失");
                Global.Event.Send<BattleTipEvent>(new BattleTipEvent("起风消失"));
                foreach (var data in positiveTrainer.trainerData.datas)
                {
                    await data.DecreaseCurrentSpeed(10);
                }

                foreach (var data in positiveTrainer.trainerData.datas)
                {
                    await data.IncreaseCurrentSpeed(10);
                }

                container.lastRoundBuffEnums.Remove(BuffEnum.起风);
            }

            if (container.buffEnums.Contains(BuffEnum.起风))
            {
                Debug.Log("起风");
                Global.Event.Send<BattleTipEvent>(new BattleTipEvent("起风"));
                foreach (var data in positiveTrainer.trainerData.datas)
                {
                    await data.DecreaseCurrentSpeed(10);
                }

                foreach (var data in positiveTrainer.trainerData.datas)
                {
                    await data.IncreaseCurrentSpeed(10);
                }

                container.buffEnums.Remove(BuffEnum.起风);
                container.lastRoundBuffEnums.Add(BuffEnum.起风);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async UniTask ExecuteSwitch(IBattleTrainer trainer, BattlePosition position, int idx)
        {
            HuluData next = trainer.trainerData.datas[idx];
            position.SetNext(next);
            await ExecuteEnter(trainer, position, next);
        }

        private async UniTask ExecuteEnter(IBattleTrainer trainer, BattlePosition position, HuluData next)
        {
            Assert.IsNotNull(next);
            await position.Prepare2Current();
            await position.ExecuteEnter();
            Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{position}切换到{next}"));
            await trainer.ChangeCurrentHulu(next);
            next.enterTimes += 1;
            Debug.Log($"{position}登场 times:{next.enterTimes}");
            Debug.Log($"{trainer.currentBattleData}->{next}->{position.currentData}");
            Assert.IsTrue(trainer == position.battleTrainer);
            await UglyMath.PostprocessHuluEnterBattle(next);
        }

        private async UniTask ExecuteSkill(IBattleTrainer userTrainer, IBattleTrainer defTrainer,
            BattlePosition userPosition, BattlePosition defPosition, ActiveSkillBattleOperation operation)
        {
            Assert.IsTrue(userPosition.CanFight());

            Assert.IsTrue(userTrainer.currentBattleData == userPosition.currentData);
            Assert.IsTrue(defTrainer.currentBattleData == defPosition.currentData);

            await userTrainer.UseCardFromHandZone(operation.data);

            // 计算伤害
            Global.Event.Send<BattleTipEvent>(
                new BattleTipEvent($"{userPosition}使用[{operation.data.config.Type}]{operation}"));

            UglyMath.PostprocessHuluDataWhenUseSkill(userPosition.currentData, operation.data.config);

            if (operation.data.config.Type == ActiveSkillTypeEnum.指挥)
            {
                if (operation.data.id == ActiveSkillEnum.疗伤药膏)
                {
                    Debug.Log($"疗伤药膏+100");
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"疗伤药膏+100"));
                    await userPosition.currentData.DecreaseHealth(-100);
                }

                if (operation.data.id == ActiveSkillEnum.战斗灵感)
                {
                    Debug.Log($"战斗灵感 抽三张牌");
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"战斗灵感 抽三张牌"));
                    await userTrainer.DrawSkills(3);
                }

                return;
            }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (operation.data.config.Type == ActiveSkillTypeEnum.伤害技能)
            {
                int times;
                if (operation.data.config.MulAttackTimes == null || operation.data.config.MulAttackTimes.Length == 0)
                {
                    times = 1;
                }
                else
                {
                    Assert.IsTrue(operation.data.config.MulAttackTimes.Length == 2);
                    Assert.IsTrue(operation.data.config.MulAttackTimes[0] <= operation.data.config.MulAttackTimes[1]);
                    times = UnityEngine.Random.Range(operation.data.config.MulAttackTimes[0],
                        operation.data.config.MulAttackTimes[1]);
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{userPosition}攻击次数:{times}"));
                }

                Debug.Log($"{userPosition}:Attack Times:{times}");
                for (int i = 0; i < times; i++)
                {
                    await userPosition.ExecuteSkill(operation);
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{userPosition}攻击次数:{i + 1}"));
                    // await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                    if (defPosition.currentData.HealthIsZero())
                    {
                        Debug.Log($"{defPosition.currentData}已经死亡 不再计算伤害");
                        break;
                    }

                    await UglyMath.PostprocessHuluDataBeforeUseSkill(userPosition.currentData, operation.data.config);
                    bool hitted = GameMath.CalHit(userPosition.currentData, defPosition.currentData, operation.data.id,
                        _environmentData);
                    if (hitted && UglyMath.PostprocessHitRate(userPosition.currentData, defPosition.currentData,
                            operation.data.id, _environmentData))
                    {
                        int damage = GameMath.CalDamage(userPosition.currentData, defPosition.currentData,
                            operation.data.id, _environmentData);
                        Global.Event.Send<BattleTipEvent>(
                            new BattleTipEvent($"{userPosition}对{defPosition.currentData}造成{damage}伤害"));
                        Debug.Log(
                            $"计算技能伤害,pos:{userPosition},{userPosition.currentData}对{defPosition.currentData}使用{operation.data.id} 造成{damage}伤害");
                        await defPosition.currentData.DecreaseHealth(damage);

                        IBattleOperation newOper =
                            await UglyMath.PostprocessHuluDataWhenHealthChange(defPosition.currentData, defTrainer);
                        if (newOper != null)
                        {
                            if (newOper is ChangeHuluOperation changeHuluOperation)
                            {
                                await ExecuteSwitch(defTrainer, defPosition, changeHuluOperation.next);
                            }
                        }

                        if (defPosition.currentData.HealthIsZero())
                        {
                            await UglyMath.PostprocessHuluDataWhenDead(defPosition.currentData);
                        }

                        await UglyMath.PostprocessHuluDataWhenAfterUseSkill(userTrainer, userPosition.currentData,
                            defTrainer,
                            operation.data.config,
                            damage, _environmentData);
                    }
                    else
                    {
                        Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{userPosition}未命中"));
                        Debug.Log(
                            $"计算技能伤害,pos:{userPosition},{userPosition.currentData}对{defPosition.currentData}使用{operation.data.id} 未命中");
                        // break;
                    }
                }
            }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            else if (operation.data.config.Type == ActiveSkillTypeEnum.变化技能)
            {
                await userPosition.ExecuteSkill(operation);
                if (operation.data.id == ActiveSkillEnum.守护)
                {
                    userPosition.currentData.buffList.Add(BuffEnum.守护);
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{userPosition}使用{operation.data.id}"));
                }
                else if (operation.data.id == ActiveSkillEnum.光合作用)
                {
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{userPosition}使用{operation.data.id}"));
                    int delta = userPosition.currentData.hp / 5;
                    await userPosition.currentData.DecreaseHealth(-delta);
                }
            }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (userTrainer == _self)
            {
                ModifyOperAfterUseSkill(ref selfOper);
            }
            else
            {
                ModifyOperAfterUseSkill(ref enemyOper);
            }
        }
    }
}