using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityToolkit;
using Random = UnityEngine.Random;

namespace Game.GamePlay
{
    public class DummyBattleFlow : MonoBehaviour, IBattleFlow
    {
        [ReadOnly, NonSerialized, ShowInInspector]
        private IBattleTrainer _self;

        [ReadOnly, NonSerialized, ShowInInspector]
        private IBattleTrainer _enemy;

        [HorizontalGroup("1")] public BattlePosition selfPos;

        [HorizontalGroup("1")] public BattlePosition enemyPos;

        public IBattleOperation selfOper = null;
        public IBattleOperation enemyOper = null;

        private CancellationTokenSource _cts;
        private BattleEnvironmentData _environmentData;

        public void Init(IBattleTrainer self, IBattleTrainer enemy, BattleEnvironmentData environmentData)
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

        public async UniTask ChangeBattleEnv(BattleEnvironmentEnum configChangeBattleEnvAfterUse)
        {
            _environmentData.id = configChangeBattleEnvAfterUse;
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
            
            _self.OnDiscardCard += _enemy.OnEnemyTrainerDiscardCard;
            _enemy.OnDiscardCard += _self.OnEnemyTrainerDiscardCard;
            
            _self.SetEnvironmentData(_environmentData);
            _enemy.SetEnvironmentData(_environmentData);

            Global.Get<AudioSystem>().Get(FMODName.Event.first_step).start();
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

                    // if (selfAtk.data.config.Type == ActiveSkillTypeEnum.指挥 &&
                    //     enemyAtk.data.config.Type == ActiveSkillTypeEnum.指挥)
                    // {
                    //     if (Random.value < 0.5)
                    //     {
                    //         await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk);
                    //         await ExecuteSkill(_enemy, _self, enemyPos, selfPos, enemyAtk);
                    //     }
                    //     else
                    //     {
                    //         await ExecuteSkill(_enemy, _self, enemyPos, selfPos, enemyAtk);
                    //         await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk);
                    //     }
                    //
                    //     continue;
                    // }

                    // if (selfAtk.data.config.Type == ActiveSkillTypeEnum.指挥)
                    // {
                    await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk);
                    await ExecuteSkill(_enemy, _self, enemyPos, selfPos, enemyAtk);
                    // }
                    // else
                    // {
                    //     await ExecuteSkill(_enemy, _self, enemyPos, selfPos, enemyAtk);
                    //     await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk);
                    // }

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


        public async UniTask AfterRound()
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
            await UniTask.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask RoundEnd()
        {
            await selfPos.currentData.RoundEnd();
            await enemyPos.currentData.RoundEnd();

            await selfPos.RoundEnd();
            await enemyPos.RoundEnd();

            await _environmentData.RoundEnd();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask Exit()
        {
            _self.OnDiscardCard -= _enemy.OnEnemyTrainerDiscardCard;
            _enemy.OnDiscardCard -= _self.OnEnemyTrainerDiscardCard;
            _environmentData.Clear();
            
            _self.ExitBattle();
            _enemy.ExitBattle();
            
            Global.Get<AudioSystem>().Get(FMODName.Event.first_step).stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
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
            if (oper is not ActiveSkillBattleOperation atk) return;

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
            var (faster, slower) = GameMath.WhoFirst(_self, _enemy, selfPos.currentData, enemyPos.currentData,
                selfAtk.data,
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
            BattlePosition userPosition, BattlePosition defPosition, IBattleOperation iOperation)
        {
            Assert.IsTrue(iOperation is ActiveSkillBattleOperation);
            var operation = (ActiveSkillBattleOperation)iOperation;
            Assert.IsTrue(userPosition.CanFight());
            Assert.IsTrue(userTrainer.currentBattleData == userPosition.currentData);
            Assert.IsTrue(defTrainer.currentBattleData == defPosition.currentData);
            var config = operation.data.config;

            if (_environmentData.GetBuff(userTrainer).buffList.Contains(BattleBuffEnum.结束回合))
            {
                if (userTrainer == _self)
                {
                    ModifyOperAfterUseSkill(ref selfOper);
                }
                else
                {
                    ModifyOperAfterUseSkill(ref enemyOper);
                }

                return;
            }

            await userTrainer.UseCardFromHandZone(operation.data);

            // 计算伤害
            Global.Event.Send<BattleTipEvent>(
                new BattleTipEvent($"{userPosition}使用[{config.Type}]{operation}"));

            UglyMath.PostprocessHuluDataWhenUseSkill(userPosition.currentData, config);

            #region 指挥牌

            if (config.Type == ActiveSkillTypeEnum.指挥)
            {
                await userPosition.ExecuteSkill(operation);
                if (operation.data.id == ActiveSkillEnum.重整思路)
                {
                    Debug.Log($"重整思路 弃所有手牌 抽等量牌");
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"重整思路 弃所有手牌 抽等量牌"));
                    int currentCount = userTrainer.handZone.Count;
                    await userTrainer.DiscardAllHandCards();
                    await userTrainer.DrawSkills(currentCount);
                }

                if (operation.data.id == ActiveSkillEnum.喝茶)
                {
                    Debug.Log($"喝茶 双方结束回合 自己回50血");
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"喝茶 双方结束回合 自己回50血"));
                    selfOper = new EndRoundOperation();
                    enemyOper = new EndRoundOperation();
                }

                if (operation.data.id == ActiveSkillEnum.轮转)
                {
                    HuluData next = userTrainer.trainerData.RandomSelectExpect(userTrainer.currentBattleData);
                    if (next == null)
                    {
                        Debug.Log("轮转没有可用的精灵");
                        Global.Event.Send<BattleTipEvent>(new BattleTipEvent("轮转没有可用的精灵"));
                    }
                    else
                    {
                        int idx = userTrainer.trainerData.datas.IndexOf(next);
                        await ExecuteSwitch(userTrainer, userPosition, idx);
                    }
                }
            }

            #endregion

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (config.Type == ActiveSkillTypeEnum.伤害技能)
            {
                await userTrainer.currentBattleData.UseSkill(operation.data, defTrainer);
                int times;
                if (config.MulAttackTimes == null || config.MulAttackTimes.Length == 0)
                {
                    times = 1;
                }
                else
                {
                    Assert.IsTrue(config.MulAttackTimes.Length == 2);
                    Assert.IsTrue(config.MulAttackTimes[0] <= config.MulAttackTimes[1]);
                    times = UnityEngine.Random.Range(config.MulAttackTimes[0],
                        config.MulAttackTimes[1]);
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

                    await UglyMath.PostprocessHuluDataBeforeUseSkill(userPosition.currentData, config);
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
                            config,
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
            else if (config.Type == ActiveSkillTypeEnum.变化技能)
            {
                await userPosition.ExecuteSkill(operation);
            }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            #region 通用效果

            if (config.IncreaseHealthPercentAfterUse != 0)
            {
                Debug.Log(
                    $"{userPosition}使用{operation.data.id}回血,百分比:{config.IncreaseHealthPercentAfterUse}");
                await userPosition.currentData.DecreaseHealth(
                    -(int)(config.IncreaseHealthPercentAfterUse *
                           userPosition.currentData.hp));
            }

            if (config.IncreaseHealthPointAfterUse != 0)
            {
                Debug.Log(
                    $"{userPosition}使用{operation.data.id}回血,固定值:{config.IncreaseHealthPointAfterUse}");
                await userPosition.currentData.DecreaseHealth(-config.IncreaseHealthPointAfterUse);
            }

            if (config.DarwCardCountAfterUse > 0)
            {
                Debug.Log($"{userPosition}使用{operation.data.id}抽牌,数量:{config.DarwCardCountAfterUse}");
                await userTrainer.DrawSkills(config.DarwCardCountAfterUse);
            }

            if (config.DarwCardCountAfterUse == -1)
            {
                Debug.Log($"{userPosition}使用{operation.data.id}抽牌,手牌抽满");
                await userTrainer.DrawHandFull();
            }

            if (config.DarwLeaderCardCountAfterUse != 0)
            {
                int cnt = 0;
                foreach (var handCard in userTrainer.handZone)
                {
                    if (handCard.config.Type == ActiveSkillTypeEnum.指挥)
                    {
                        cnt++;
                    }
                }

                // TODO 有必要触发洗牌么
                if (cnt < config.DarwLeaderCardCountAfterUse)
                {
                    await userTrainer.Discard2DrawZone();
                }

                userTrainer.DrawTarget(ActiveSkillTypeEnum.指挥, config.DarwLeaderCardCountAfterUse);
            }

            if (config.SelfBattleBuffAfterUse != BattleBuffEnum.None)
            {
                var buffConfig = Global.Table.BattleBuffTable.Get(config.SelfBattleBuffAfterUse);
                for (int i = 0; i != config.SelfBattleBuffCountAfterUse; ++i)
                {
                    Global.Event.Send<BattleTipEvent>(
                        new BattleTipEvent(
                            $"{userPosition}使用{operation.data.id}获得{config.SelfBattleBuffAfterUse}"));
                    Debug.Log($"{userPosition}使用{operation.data.id}获得{config.SelfBattleBuffAfterUse}");
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                    if (buffConfig.IsEnvBuff)
                    {
                        await _environmentData.AddBuff(userTrainer, config.SelfBattleBuffAfterUse);
                    }
                    else
                    {
                        await userTrainer.currentBattleData.AddBuff(config.SelfBattleBuffAfterUse);
                    }
                }
            }

            if (config.SelfTrainerBuffAfterUse != BattleBuffEnum.None)
            {
                var buffConfig = Global.Table.BattleBuffTable.Get(config.SelfTrainerBuffAfterUse);
                for (int i = 0; i < config.SelfTrainerBuffAfterUseCount; i++)
                {
                    Global.Event.Send<BattleTipEvent>(
                        new BattleTipEvent(
                            $"{userPosition}使用{operation.data.id}获得{config.SelfTrainerBuffAfterUse}"));
                    Debug.Log($"{userPosition}使用{operation.data.id}获得{config.SelfTrainerBuffAfterUse}");
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                    Assert.IsTrue(buffConfig.IsEnvBuff);

                    await _environmentData.AddBuff(userTrainer, config.SelfTrainerBuffAfterUse);
                }
            }

            if (config.DefTrainerBuffAfterUse != BattleBuffEnum.None)
            {
                var buffConfig = Global.Table.BattleBuffTable.Get(config.DefTrainerBuffAfterUse);
                for (int i = 0; i < config.DefTrainerBuffAfterUseCount; i++)
                {
                    Global.Event.Send<BattleTipEvent>(
                        new BattleTipEvent(
                            $"{userPosition}使用{operation.data.id}给对方加buff:{config.DefTrainerBuffAfterUse}"));
                    Debug.Log(
                        $"{userPosition}使用{operation.data.id}给对方加buff:{config.DefTrainerBuffAfterUse}");
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                    Assert.IsTrue(buffConfig.IsEnvBuff);
                    await _environmentData.AddBuff(defTrainer, config.DefTrainerBuffAfterUse);
                }
            }

            if (config.ChangeBattleEnvAfterUse != BattleEnvironmentEnum.None)
            {
                await ChangeBattleEnv(config.ChangeBattleEnvAfterUse);
            }


            var drawTargetCardConfigAfterUse = config.DarwTargetCardConfigAfterUse;
            if (drawTargetCardConfigAfterUse != null && drawTargetCardConfigAfterUse.Target != ActiveSkillEnum.None)
            {
                int drawed = await userTrainer.DrawTarget(drawTargetCardConfigAfterUse.Target,
                    drawTargetCardConfigAfterUse.Cnt);
                if (drawTargetCardConfigAfterUse.DrawAnyIfCanNotDrawTarget && drawed < drawTargetCardConfigAfterUse.Cnt)
                {
                    Debug.Log($"抽指定牌失败，抽了{drawed}张，继续抽{drawTargetCardConfigAfterUse.Cnt - drawed}张任意牌");
                    await userTrainer.DrawSkills(drawTargetCardConfigAfterUse.Cnt - drawed);
                }

                int targetCnt = userTrainer.GetTargetCntInDeck(ActiveSkillTypeEnum.伤害技能 | ActiveSkillTypeEnum.变化技能);
                if (targetCnt < drawTargetCardConfigAfterUse.AddWhenSkillCardLessThanCnt)
                {
                    for (int i = 0; i < drawTargetCardConfigAfterUse.AddCnt; i++)
                    {
                        await userTrainer.AddCardToDeck(new ActiveSkillData()
                        {
                            id = drawTargetCardConfigAfterUse.AddTarget
                        });
                    }
                }
            }

            var fullHpAddBuffConfig = config.FullHpBuffForUserPokemon;
            if (userTrainer.currentBattleData.currentHp >= userTrainer.currentBattleData.hp &&
                fullHpAddBuffConfig.Buff != BattleBuffEnum.None)
            {
                for (int i = 0; i < fullHpAddBuffConfig.Cnt; i++)
                {
                    await _environmentData.AddBuff(userTrainer, fullHpAddBuffConfig.Buff);
                }
            }

            var notFullHpAddBuffConfig = config.NotFullHpBuffForUserPokemon;
            if (userTrainer.currentBattleData.currentHp < userTrainer.currentBattleData.hp &&
                notFullHpAddBuffConfig.Buff != BattleBuffEnum.None)
            {
                for (int i = 0; i < notFullHpAddBuffConfig.Cnt; i++)
                {
                    await _environmentData.AddBuff(userTrainer, notFullHpAddBuffConfig.Buff);
                }
            }

            #endregion

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