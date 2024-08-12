﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using cfg;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.GamePlay
{
    public class DummyBattleFlow : MonoBehaviour, IBattleFlow
    {
        [ReadOnly, ShowInInspector] public IBattleTrainer self { get; private set; }

        [ReadOnly, ShowInInspector] public IBattleTrainer enemy { get; private set; }

        [HorizontalGroup("1")] public BattlePosition selfPos;
        [HorizontalGroup("1")] public BattlePosition enemyPos;
        [HorizontalGroup("1")] public BattleEnv battleEnv;

        private IBattleOperation _selfOper;
        private IBattleOperation _enemyOper;

        private CancellationTokenSource _cts;
        [ReadOnly, ShowInInspector] public BattleEnvData envEnvData { get; private set; }

        [field: SerializeField] public BattleSettlementData settlementData { get; private set; }

        public void Init(IBattleTrainer self, IBattleTrainer enemy, BattleEnvData envData)
        {
            this.self = self;
            this.enemy = enemy;

            envEnvData = envData;

            selfPos.battleTrainer = self;
            enemyPos.battleTrainer = enemy;
            settlementData = new BattleSettlementData(self.trainerData, enemy.trainerData);

            Global.Event.Listen<OnBattleApplyDamage>(OnBattleApplyDamage);
            Global.Event.Listen<OnDefeatPokemon>(OnDefeatPokemon);
        }

        private void OnDefeatPokemon(OnDefeatPokemon obj)
        {
            settlementData.AddDefeatCount(obj.attacker.trainerData, obj.attacker.currentBattleData, 1);
        }

        private void OnBattleApplyDamage(OnBattleApplyDamage obj)
        {
            settlementData.AddDamageCount(obj.attacker.trainerData, obj.attacker.currentBattleData, obj.damage);
        }


        public void Dispose()
        {
            Global.Event.UnListen<OnBattleApplyDamage>(OnBattleApplyDamage);
            Global.Event.UnListen<OnDefeatPokemon>(OnDefeatPokemon);
            _cts?.Cancel();
            _cts = null;
            UIRoot.Singleton.ClosePanel<GameBattlePanel>();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts = null;
        }

        public async UniTask ChangeBattleEnv(BattleEnvironmentEnum configChangeBattleEnvAfterUse)
        {
            await envEnvData.Change(configChangeBattleEnvAfterUse);
        }

        public async UniTask Enter()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }

            Assert.IsNull(_cts);
            // Roll Initial Cards

            // Open Battle Panel
            _cts = new CancellationTokenSource();

            selfPos.PrepareNext(selfPos.battleTrainer.Get(0));
            enemyPos.PrepareNext(enemyPos.battleTrainer.Get(0));

            await battleEnv.Init(envEnvData);


            self.OnDiscardCardFromHand += enemy.OnEnemyTrainerDiscardCard;
            enemy.OnDiscardCardFromHand += self.OnEnemyTrainerDiscardCard;


            GameBattleMgr.Singleton.PlayBGM();
            await IBattleFlow.RoundFlow(this, _cts.Token);
        }

        public async UniTask RoundStart()
        {
            // throw new System.NotImplementedException();
            // 执行入场逻辑
            await EnterBattleCheck();
            await self.BeforeRounding();
            await enemy.BeforeRounding();
        }

        public async UniTask BeforeRound()
        {
            // throw new System.NotImplementedException();
            // 各自抽卡
            await self.DrawSkills(CalEveryRoundDrawCnt());
            await enemy.DrawSkills(CalEveryRoundDrawCnt());
        }

        public int CalEveryRoundDrawCnt()
        {
            var gameData = Global.Get<DataSystem>().Get<GameData>();
            if (gameData is { ruleConfig: { ruleList: not null } } &&
                gameData.ruleConfig.ruleList.Contains(GameRuleEnum.每回合抽牌数量变为2张))
            {
                return 2;
            }

            return Consts.EvertRoundDrawCardCnt;
        }


        public async UniTask Rounding()
        {
            // throw new System.NotImplementedException();
            _selfOper = null;
            _enemyOper = null;

            Debug.Log("Rounding");
            while (_cts is { IsCancellationRequested: false })
            {
                await UniTask.DelayFrame(1);

                // 有人不能战斗了
                if (!selfPos.current.CanFight() || !enemyPos.current.CanFight())
                {
                    Global.LogInfo(
                        $"{selfPos.current}-CanFight:{selfPos.current.CanFight()}\n{enemyPos.current}-CanFight:{enemyPos.current.CanFight()}");
                    Global.LogInfo("有人不能战斗了 回合结束");
                    break;
                }

                await GameMath.ProcessPokemonBeforeRounding(self);
                await GameMath.ProcessPokemonBeforeRounding(enemy);

                _selfOper = await GameMath.ProcessOperationBeforeRounding(self, _selfOper);
                _enemyOper = await GameMath.ProcessOperationBeforeRounding(enemy, _enemyOper);


                // 有人不能战斗了
                if (!selfPos.current.CanFight() || !enemyPos.current.CanFight())
                {
                    Debug.Log($"有人不能战斗了");
                    break;
                }


                // 等待双方操作
                if (_selfOper is not EndRoundOperation)
                {
                    self.ClearOperation();
                    _selfOper = await self.CalOperation();
                }

                if (_enemyOper is not EndRoundOperation)
                {
                    enemy.ClearOperation();
                    Global.Event.Send<BattleStateTipEvent>(new BattleStateTipEvent("对方正在思考"));
                    _enemyOper = await enemy.CalOperation();
                    Global.Event.Send<BattleStateTipEvent>(new BattleStateTipEvent(""));
                }

                // 双方都结束回合 则进入下一阶段
                if (_selfOper is EndRoundOperation && _enemyOper is EndRoundOperation)
                {
                    // Debug.LogWarning("双方结束回合");
                    break;
                }


                Debug.Log($"Self Oper: {_selfOper},Enemy Oper: {_enemyOper}");


                if (_selfOper is ActiveSkillBattleOperation selfAtk &&
                    _enemyOper is ActiveSkillBattleOperation enemyAtk)
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

                    await ExecuteSkill(self, enemy, selfPos, enemyPos, selfAtk);
                    if (enemy.currentBattleData.CanFight())
                    {
                        await ExecuteSkill(enemy, self, enemyPos, selfPos, enemyAtk);
                    }
                    else
                    {
                        Global.LogInfo($"{enemyPos.current}已经死亡 不再放技能");
                    }

                    continue; // 这里双方技能都结算了 所以直接跳到下一回合
                }

                if (_selfOper is EndRoundOperation && _enemyOper is ActiveSkillBattleOperation enemyAtk1)
                {
                    await ExecuteSkill(enemy, self, enemyPos, selfPos, enemyAtk1);
                    continue;
                }

                if (_selfOper is ActiveSkillBattleOperation selfAtk1 && _enemyOper is EndRoundOperation)
                {
                    await ExecuteSkill(self, enemy, selfPos, enemyPos, selfAtk1);
                    continue;
                }


                // TODO 如果同时切换 或许有先后问题 但是和AI玩不用管 自己先切换
                if (_selfOper is ChangeHuluOperation selfChange1 && _enemyOper is ChangeHuluOperation enemyChange1)
                {
                    await ExecuteSwitch(self, selfPos, selfChange1.next);
                    ModifyOperAfterChangeHulu(ref _selfOper);
                    await ExecuteSwitch(enemy, enemyPos, enemyChange1.next);
                    ModifyOperAfterChangeHulu(ref _enemyOper);
                    continue;
                }

                if (_selfOper is ChangeHuluOperation selfChange2 && _enemyOper is ActiveSkillBattleOperation enemyAtk2)
                {
                    await ExecuteSwitch(self, selfPos, selfChange2.next);
                    ModifyOperAfterChangeHulu(ref _selfOper);
                    await ExecuteSkill(enemy, self, enemyPos, selfPos, enemyAtk2);
                    continue;
                }

                if (_selfOper is ActiveSkillBattleOperation selfAtk3 && _enemyOper is ChangeHuluOperation enemyChange2)
                {
                    await ExecuteSwitch(enemy, enemyPos, enemyChange2.next);
                    ModifyOperAfterChangeHulu(ref _enemyOper);
                    await ExecuteSkill(self, enemy, selfPos, enemyPos, selfAtk3);
                    continue;
                }

                if (_selfOper is ChangeHuluOperation selfChange && _enemyOper is EndRoundOperation)
                {
                    await ExecuteSwitch(self, selfPos, selfChange.next);
                    ModifyOperAfterChangeHulu(ref _selfOper);
                    continue;
                }

                if (_selfOper is EndRoundOperation && _enemyOper is ChangeHuluOperation enemyChange)
                {
                    await ExecuteSwitch(enemy, enemyPos, enemyChange.next);
                    ModifyOperAfterChangeHulu(ref _enemyOper);
                    continue;
                }

                throw new NotImplementedException(
                    $"Self Oper: {_selfOper.GetType()},Enemy Oper: {_enemyOper.GetType()} NotImplemented");
            }
        }


        public async UniTask AfterRound()
        {
            if (!selfPos.current.CanFight())
            {
                if (self.trainerData.FindFirstCanFight(out HuluData selfNext))
                {
                    Global.LogInfo($"{selfPos.current}战斗不能，自动切换到{selfNext}");
                    selfPos.PrepareNext(selfNext);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            if (!enemyPos.current.CanFight())
            {
                if (enemy.trainerData.FindFirstCanFight(out HuluData enemyNext))
                {
                    Global.LogInfo($"{enemyPos.current}战斗不能，自动切换到{enemyNext}");
                    enemyPos.PrepareNext(enemyNext);
                }

                else
                {
                    throw new NotImplementedException();
                }
            }

            // 如果有一方G了 则进行自动替换逻辑
            await UniTask.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask RoundEnd()
        {
            await selfPos.RoundEnd();
            await selfPos.current.RoundEnd();

            await enemyPos.RoundEnd();
            await enemyPos.current.RoundEnd();

            await self.RoundEnd();
            await enemy.RoundEnd();

            await envEnvData.RoundEnd();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UniTask Exit(IBattleTrainer winner)
        {
            settlementData.isLocalPlayerWin = winner == self;


            self.OnDiscardCardFromHand -= enemy.OnEnemyTrainerDiscardCard;
            enemy.OnDiscardCardFromHand -= self.OnEnemyTrainerDiscardCard;
            envEnvData.Clear();

            self.ExitBattle();
            enemy.ExitBattle();

            GameBattleMgr.Singleton.StopBGM();

            return UniTask.CompletedTask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Cancel()
        {
            Assert.IsNotNull(_cts);
            _cts.Cancel();
            _cts = null;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public bool TryGetRoundWinner(out IBattleTrainer battleTrainer)
        // {
        //     if (!_self.currentBattleData.CanFight())
        //     {
        //         battleTrainer = _enemy;
        //         return true;
        //     }
        //
        //     if (!_enemy.currentBattleData.CanFight())
        //     {
        //         battleTrainer = _self;
        //         return true;
        //     }
        //
        //     battleTrainer = null;
        //     return false;
        // }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetFinalWinner(out IBattleTrainer battleTrainer)
        {
            if (self.trainerData.canFight && !enemy.trainerData.canFight)
            {
                battleTrainer = self;
                return true;
            }

            if (!self.trainerData.canFight && enemy.trainerData.canFight)
            {
                battleTrainer = enemy;
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
                await ExecuteEnter(self, selfPos, selfPos.next);
            }

            if (enemyPos.next != null)
            {
                await ExecuteEnter(enemy, enemyPos, enemyPos.next);
            }
        }


        private async UniTask BothPokemonSkill(ActiveSkillBattleOperation selfAtk, ActiveSkillBattleOperation enemyAtk)
        {
            var (faster, slower) = GameMath.WhoFirst(self, enemy, selfPos.current, enemyPos.current,
                selfAtk.data,
                enemyAtk.data, envEnvData);

            // 根据顺序结算
            if (faster == selfPos.current)
            {
                await ExecuteSkill(self, enemy, selfPos, enemyPos, selfAtk);
                if (!enemyPos.current.CanFight())
                {
                    return;
                }

                if (slower != enemy.currentBattleData)
                {
                    Debug.Log("走了退场逻辑 不再放技能");
                    return;
                }

                // 如果打死了对方 则不用再打了
                await ExecuteSkill(enemy, self, enemyPos, selfPos, enemyAtk);
            }
            else
            {
                await ExecuteSkill(enemy, self, enemyPos, selfPos, enemyAtk);
                if (!selfPos.current.CanFight())
                {
                    return;
                }

                if (slower != self.currentBattleData)
                {
                    Debug.Log("走了退场逻辑 不再放技能");
                    return;
                }

                // 如果打死了对方 则不用再打了
                await ExecuteSkill(self, enemy, selfPos, enemyPos, selfAtk);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private async UniTask ExecuteSwitch(IBattleTrainer trainer, BattlePosition position, int idx)
        {
            HuluData next = trainer.trainerData.datas[idx];
            position.PrepareNext(next);
            await ExecuteEnter(trainer, position, next);
        }

        private async UniTask ExecuteEnter(IBattleTrainer trainer, BattlePosition position, HuluData next)
        {
            Assert.IsNotNull(next);
            Global.Event.Send(new BattleInfoRecordEvent($"{position}切换到{next}"));
            await position.NextReplaceCurrent();
            await position.ExecuteEnter();
            await trainer.SwitchPokemon(next);
            next.enterTimes += 1;
            Global.Event.Send(new BattleInfoRecordEvent($"{trainer.currentBattleData} 第{next.enterTimes}次登场"));
            Assert.IsTrue(trainer == position.battleTrainer);
            await UglyMath.PostprocessHuluEnterBattle(next);
        }

        private async UniTask ExecuteSkill(IBattleTrainer userTrainer, IBattleTrainer defTrainer,
            BattlePosition userPosition, BattlePosition defPosition, IBattleOperation iOperation)
        {
            Assert.IsTrue(iOperation is ActiveSkillBattleOperation);
            var operation = (ActiveSkillBattleOperation)iOperation;
            Assert.IsTrue(userPosition.current.CanFight());
            Assert.IsTrue(userTrainer.currentBattleData == userPosition.current);
            Assert.IsTrue(defTrainer.currentBattleData == defPosition.current);
            var config = operation.data.config;

            if (userTrainer.ContainsBuff(BattleBuffEnum.结束回合))
            {
                if (userTrainer == self)
                {
                    ModifyOperAfterUseSkill(ref _selfOper);
                }
                else
                {
                    ModifyOperAfterUseSkill(ref _enemyOper);
                }

                return;
            }

            if (!userTrainer.handZone.Contains(operation.data))
            {
                if (userTrainer == self)
                {
                    ModifyOperAfterUseSkill(ref _selfOper);
                }
                else
                {
                    ModifyOperAfterUseSkill(ref _enemyOper);
                }

                Debug.LogWarning($"居然打出了不在手牌里的牌，是因为对方的技能让我弃牌了么");
                return;
            }

            await userTrainer.UseCardFromHand(operation.data);

            await GameMath.ProcessTrainerAfterUseCardFromHandZone(userTrainer);

            // 计算伤害
            Global.Event.Send(
                new BattleInfoRecordEvent($"{userPosition}使用{operation.data}"));

            await UglyMath.PostprocessHuluDataWhenUseSkill(userPosition.current, config);

            #region 指挥牌

            if (config.Type == ActiveSkillTypeEnum.指挥)
            {
                await userPosition.ExecuteSkill(operation);
                if (operation.data.id == ActiveSkillEnum.重整思路)
                {
                    Global.Event.Send(new BattleInfoRecordEvent($"重整思路 弃所有手牌 抽等量牌"));
                    int currentCount = userTrainer.handZone.Count;
                    await userTrainer.DiscardAllHandCards();
                    await userTrainer.DrawSkills(currentCount);
                }

                if (operation.data.id == ActiveSkillEnum.喝茶)
                {
                    Global.Event.Send(new BattleInfoRecordEvent($"喝茶 双方结束回合 自己回50血"));
                    _selfOper = new EndRoundOperation();
                    _enemyOper = new EndRoundOperation();
                }

                if (operation.data.id == ActiveSkillEnum.轮转)
                {
                    HuluData next = userTrainer.trainerData.RandomSelectExpect(userTrainer.currentBattleData);
                    if (next == null)
                    {
                        Global.Event.Send<BattleInfoRecordEvent>(new BattleInfoRecordEvent("轮转没有可用的精灵"));
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
                int times;
                if (config.MulAttackTimes == null || config.MulAttackTimes.Length == 0)
                {
                    times = 1;
                }
                else
                {
                    Assert.IsTrue(config.MulAttackTimes.Length == 2);
                    Assert.IsTrue(config.MulAttackTimes[0] <= config.MulAttackTimes[1]);
                    times = GameMath.CalAtkTimes(userPosition.current, config);
                    Global.Event.Send<BattleInfoRecordEvent>(new BattleInfoRecordEvent($"{userPosition}攻击次数:{times}"));
                }

                GameData gameData = Global.Get<DataSystem>().Get<GameData>();
                Debug.Log($"{userPosition}:Attack Times:{times}");
                for (int i = 0; i < times; i++)
                {
                    await userTrainer.currentBattleData.UseSkill(operation.data, defTrainer, i);
                    if (!userTrainer.currentBattleData.CanFight())
                    {
                        Debug.Log($"{userPosition.current}战斗不能 不再计算伤害");
                        break;
                    }

                    if (!defPosition.current.CanFight())
                    {
                        Debug.Log($"{defPosition.current}战斗不能 不再计算伤害");
                        break;
                    }

                    await userPosition.ExecuteSkill(operation);
                    Global.Event.Send<BattleInfoRecordEvent>(new BattleInfoRecordEvent($"{userPosition}攻击次数:{i + 1}"));
                    // await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

                    await UglyMath.PostprocessHuluDataBeforeUseSkill(userPosition.current, config);
                    bool hitted = GameMath.CalHit(userPosition.current, defPosition.current, operation.data.id,
                        envEnvData);
                    if (hitted && await UglyMath.PostprocessHitRate(userPosition.current, defPosition.current,
                            operation.data.id, envEnvData))
                    {
                        int damage = await GameMath.CalDamage(userPosition.current, defPosition.current,
                            operation.data.id, envEnvData, gameData);
                        Global.Event.Send<BattleInfoRecordEvent>(
                            new BattleInfoRecordEvent(
                                $"{userPosition}对{defPosition.current}使用{operation.data.id}造成{damage}伤害"));
                        Global.Event.Send<OnBattleApplyDamage>(
                            new OnBattleApplyDamage(
                                userTrainer,
                                defTrainer,
                                userTrainer.currentBattleData,
                                defTrainer.currentBattleData,
                                damage));

                        await defPosition.OnTakeSkillFrom(operation.data, userTrainer, userPosition, damage);
                        await defPosition.current.DecreaseHealth(damage);

                        if (!defPosition.current.CanFight())
                        {
                            Global.Event.Send<OnDefeatPokemon>(
                                new OnDefeatPokemon(
                                    userTrainer,
                                    defTrainer,
                                    userTrainer.currentBattleData,
                                    defTrainer.currentBattleData));
                        }

                        IBattleOperation newOper =
                            await UglyMath.CalNewOperWhenPokemonHealthChange(defTrainer);
                        if (newOper != null)
                        {
                            if (newOper is ChangeHuluOperation changeHuluOperation)
                            {
                                await ExecuteSwitch(defTrainer, defPosition, changeHuluOperation.next);
                            }
                        }

                        // Tag 只有命中了才会执行的效果
                        await UglyMath.EffectWhenSkillHitted(userTrainer,
                            defTrainer,
                            config,
                            damage, envEnvData);
                    }
                    else
                    {
                        Global.Event.Send<BattleInfoRecordEvent>(new BattleInfoRecordEvent(
                            $"计算技能伤害,pos:{userPosition},{userPosition.current}对{defPosition.current}使用{operation.data.id} 未命中"));
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

            if (config.UserDiscardCountAnyway != 0)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{userTrainer} 弃牌生效 {config.UserDiscardCountAnyway}"));
                await userTrainer.RandomDiscardCardFromHand(config.UserDiscardCountAnyway);
            }

            if (config.DefDiscardCountAnyway != 0)
            {
                Global.Event.Send(
                    new BattleInfoRecordEvent($"{config} 弃牌生效 {defTrainer}弃{config.DefDiscardCountAnyway} 张"));
                await defTrainer.RandomDiscardCardFromHand(config.DefDiscardCountAnyway);
            }


            if (config.IncreaseHealthPercentAfterUse != 0)
            {
                Debug.Log(
                    $"{userPosition}使用{operation.data.id}回血,百分比:{config.IncreaseHealthPercentAfterUse}");
                await userPosition.current.DecreaseHealth(
                    -(int)(config.IncreaseHealthPercentAfterUse *
                           userPosition.current.hp));
            }

            if (config.IncreaseHealthPointAfterUse != 0)
            {
                Debug.Log(
                    $"{userPosition}使用{operation.data.id}回血,固定值:{config.IncreaseHealthPointAfterUse}");
                await userPosition.current.DecreaseHealth(-config.IncreaseHealthPointAfterUse);
            }

            if (config.DarwCardCountAfterUse > 0)
            {
                Debug.Log($"{userPosition}使用{operation.data.id}抽牌,数量:{config.DarwCardCountAfterUse}");
                await userTrainer.DrawSkills(config.DarwCardCountAfterUse);
            }
            else if (config.DarwCardCountAfterUse == -1)
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
                    Global.Event.Send<BattleInfoRecordEvent>(
                        new BattleInfoRecordEvent(
                            $"{userPosition}使用{operation.data.id}获得{config.SelfBattleBuffAfterUse}"));
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                    Assert.IsFalse(buffConfig.IsTrainerBuff);
                    await userTrainer.currentBattleData.AddBuff(config.SelfBattleBuffAfterUse);
                }
            }

            if (config.SelfTrainerBuffAfterUse != BattleBuffEnum.None)
            {
                var buffConfig = Global.Table.BattleBuffTable.Get(config.SelfTrainerBuffAfterUse);
                for (int i = 0; i < config.SelfTrainerBuffAfterUseCount; i++)
                {
                    Global.Event.Send<BattleInfoRecordEvent>(
                        new BattleInfoRecordEvent(
                            $"{userPosition}使用{operation.data.id}获得{config.SelfTrainerBuffAfterUse}"));
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                    Assert.IsTrue(buffConfig.IsTrainerBuff);

                    await userTrainer.AddBuff(config.SelfTrainerBuffAfterUse);
                }
            }

            if (config.DefTrainerBuffAfterUse != BattleBuffEnum.None)
            {
                var buffConfig = Global.Table.BattleBuffTable.Get(config.DefTrainerBuffAfterUse);
                for (int i = 0; i < config.DefTrainerBuffAfterUseCount; i++)
                {
                    Global.Event.Send<BattleInfoRecordEvent>(
                        new BattleInfoRecordEvent(
                            $"{userPosition}使用{operation.data.id}给对方加buff:{config.DefTrainerBuffAfterUse}"));
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                    Assert.IsTrue(buffConfig.IsTrainerBuff);
                    await defTrainer.AddBuff(config.DefTrainerBuffAfterUse);
                }
            }

            if (config.ChangeBattleEnvAfterUse != BattleEnvironmentEnum.None)
            {
                Global.LogInfo($"{userPosition}使用{operation.data.id}改变战场环境:{config.ChangeBattleEnvAfterUse}");
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

            var fullHpBuffForUserPokemon = config.FullHpBuffForUserPokemon;
            if (userTrainer.currentBattleData.currentHp >= userTrainer.currentBattleData.hp &&
                fullHpBuffForUserPokemon.Buff != BattleBuffEnum.None)
            {
                for (int i = 0; i < fullHpBuffForUserPokemon.Cnt; i++)
                {
                    await userTrainer.currentBattleData.AddBuff(fullHpBuffForUserPokemon.Buff);
                }
            }

            var notFullHpBuffForUserPokemon = config.NotFullHpBuffForUserPokemon;
            if (userTrainer.currentBattleData.currentHp < userTrainer.currentBattleData.hp &&
                notFullHpBuffForUserPokemon.Buff != BattleBuffEnum.None)
            {
                for (int i = 0; i < notFullHpBuffForUserPokemon.Cnt; i++)
                {
                    await userTrainer.currentBattleData.AddBuff(notFullHpBuffForUserPokemon.Buff);
                }
            }

            var fullHpBuffForUser = config.FullHpBuffForUser;
            if (userTrainer.currentBattleData.currentHp >= userTrainer.currentBattleData.hp &&
                fullHpBuffForUser.Buff != BattleBuffEnum.None)
            {
                for (int i = 0; i < fullHpBuffForUser.Cnt; i++)
                {
                    await userTrainer.AddBuff(fullHpBuffForUser.Buff);
                }
            }

            var notFullHpBuffForUser = config.NotFullHpBuffForUser;
            if (userTrainer.currentBattleData.currentHp < userTrainer.currentBattleData.hp &&
                notFullHpBuffForUser.Buff != BattleBuffEnum.None)
            {
                for (int i = 0; i < notFullHpBuffForUser.Cnt; i++)
                {
                    await userTrainer.AddBuff(notFullHpBuffForUser.Buff);
                }
            }

            var whenSpeedLessThanBuff = config.WhenSppedLessThanBuff;
            if (whenSpeedLessThanBuff.Buff != BattleBuffEnum.None && whenSpeedLessThanBuff.Compare != -1)
            {
                float speed = UglyMath.PostprocessRunTimeSpeed(userTrainer, envEnvData);
                if (speed < whenSpeedLessThanBuff.Compare)
                {
                    await userTrainer.AddBuff(whenSpeedLessThanBuff.Buff);
                }
            }

            var whenAdapGreaterThanBuff = config.WhenAdapGeaterThanBuff;
            if (whenAdapGreaterThanBuff.Buff != BattleBuffEnum.None && whenAdapGreaterThanBuff.Compare != -1)
            {
                float heal = whenAdapGreaterThanBuff.Point;
                float adap = GameMath.CalRunTimeAdap(userTrainer.currentBattleData, envEnvData);
                if (adap >= whenAdapGreaterThanBuff.Compare)
                {
                    await userTrainer.AddBuff(whenAdapGreaterThanBuff.Buff);
                }
                else
                {
                    await userTrainer.currentBattleData.DecreaseHealth(-(int)heal);
                }
            }

            var consumeSelfCardBuff = config.ConsumeSelfCardBuffConfig;
            // Debug.Log($"消耗自己的牌，类型:{consumeSelfCardBuff.TargetType} 数量:{consumeSelfCardBuff.Cnt}");
            if (consumeSelfCardBuff.Cnt != 0 && consumeSelfCardBuff.Buff != BattleBuffEnum.None &&
                (consumeSelfCardBuff.TargetType & ActiveSkillTypeEnum.None) == 0)
            {
                Debug.Log($"消耗自己的牌，类型:{consumeSelfCardBuff.TargetType} 数量:{consumeSelfCardBuff.Cnt}");
                int cnt = 0;
                // 消耗自己的牌
                int handTargetCnt = userTrainer.GetConsumeCardInHandCount(consumeSelfCardBuff.TargetType);
                if (consumeSelfCardBuff.Cnt == -1)
                {
                    cnt = handTargetCnt;
                }
                else
                {
                    cnt = Math.Min(consumeSelfCardBuff.Cnt, handTargetCnt);
                }

                List<ActiveSkillData> targets = ListPool<ActiveSkillData>.Get();
                int takeCnt = 0;
                foreach (var skillData in userTrainer.handZone)
                {
                    if (takeCnt >= cnt) break;
                    if ((skillData.config.Type & consumeSelfCardBuff.TargetType) != 0)
                    {
                        targets.Add(skillData);
                        takeCnt++;
                    }
                }

                Assert.IsTrue(targets.Count == cnt);

                foreach (var target in targets)
                {
                    Assert.IsTrue(userTrainer.handZone.Contains(target));
                    await userTrainer.ConsumeCardFromHand(target);
                    await userTrainer.currentBattleData.AddBuff(consumeSelfCardBuff.Buff);
                }

                ListPool<ActiveSkillData>.Release(targets);
            }

            #endregion

            if (userTrainer == self)
            {
                ModifyOperAfterUseSkill(ref _selfOper);
            }
            else
            {
                ModifyOperAfterUseSkill(ref _enemyOper);
            }
        }
    }
}