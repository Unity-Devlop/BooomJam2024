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

        public IBattleOperation selfOper = null;
        public IBattleOperation enemyOper = null;

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

                throw new NotImplementedException(
                    $"Self Oper: {selfOper.GetType()},Enemy Oper: {enemyOper.GetType()} NotImplemented");
            }
        }

        private void ModifyOperAfterUseSkill(ref IBattleOperation oper)
        {
            Assert.IsTrue(oper is ActiveSkillBattleOperation);
            var atk = (ActiveSkillBattleOperation)oper;

            if (atk.data.config.Type != ActiveSkillTypeEnum.指挥)
            {
                oper = new EndRoundOperation();
            }
        }

        private void ModifyOperAfterChangeHulu(ref IBattleOperation oper)
        {
            Assert.IsTrue(oper is ChangeHuluOperation);
            oper = new EndRoundOperation();
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
                await ExecuteEnter(_self, selfPos, selfPos.next);
            }

            if (enemyPos.next != null)
            {
                await ExecuteEnter(_enemy, enemyPos, enemyPos.next);
            }
        }


        private async UniTask BothPokemonSkill(ActiveSkillBattleOperation selfAtk, ActiveSkillBattleOperation enemyAtk)
        {
            var (faster, _) = GameMath.WhoFirst(selfPos.currentData, enemyPos.currentData, selfAtk.data,
                enemyAtk.data, _environmentData);

            // 根据顺序结算
            if (faster == selfPos.currentData)
            {
                await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk);
                if (!enemyPos.CanFight())
                {
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

                // 如果打死了对方 则不用再打了
                await ExecuteSkill(_self, _enemy, selfPos, enemyPos, selfAtk);
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

            await userTrainer.OnConsumeSkill(operation.data);
            await userPosition.ExecuteSkill(operation);
            // 计算伤害
            Global.Event.Send<BattleTipEvent>(
                new BattleTipEvent($"{userPosition}使用[{operation.data.config.Type}]{operation}"));

            HuluData user = userPosition.currentData;
            HuluData def;
            if (userPosition == selfPos)
            {
                def = enemyPos.currentData;
            }
            else
            {
                def = selfPos.currentData;
            }

            UglyMath.PostprocessHuluDataWhenUseSkill(user, operation.data.config);

            if (operation.data.config.Type == ActiveSkillTypeEnum.指挥)
            {
                return;
            }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (operation.data.config.Type == ActiveSkillTypeEnum.伤害技能)
            {
                await UglyMath.PostprocessHuluDataBeforeUseSkill(user, operation.data.config);
                bool hitted = GameMath.CalHit(user, def, operation.data.id, _environmentData);
                if (hitted && UglyMath.PostprocessHitRate(user, def, operation.data.id, _environmentData))
                {
                    int damage = GameMath.CalDamage(user, def, operation.data.id, _environmentData);
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{userPosition}对{def}造成{damage}伤害"));
                    Debug.Log($"计算技能伤害,pos:{userPosition},{user}对{def}使用{operation.data.id} 造成{damage}伤害");
                    await def.ChangeHealth(damage);

                    IBattleOperation newOper = await UglyMath.PostprocessHuluDataWhenHealthChange(def, defTrainer);
                    if (newOper != null)
                    {
                        if (newOper is ChangeHuluOperation changeHuluOperation)
                        {
                            await ExecuteSwitch(defTrainer, defPosition, changeHuluOperation.next);
                        }
                    }

                    if (def.HealthIsZero())
                    {
                        await UglyMath.PostprocessHuluDataWhenDead(def);
                    }

                    await UglyMath.PostprocessHuluDataWhenAfterUseSkill(user, defTrainer, operation.data.config,
                        damage);
                }
                else
                {
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{userPosition}未命中"));
                    Debug.Log($"计算技能伤害,pos:{userPosition},{user}对{def}使用{operation.data.id} 未命中");
                }
            }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            else if (operation.data.config.Type == ActiveSkillTypeEnum.变化技能)
            {
                if (operation.data.id == ActiveSkillEnum.守护)
                {
                    user.buffList.Add(BuffEnum.守护);
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{userPosition}使用{operation.data.id}"));
                }
                else if (operation.data.id == ActiveSkillEnum.光合作用)
                {
                    Global.Event.Send<BattleTipEvent>(new BattleTipEvent($"{userPosition}使用{operation.data.id}"));
                    int delta = user.hp / 5;
                    await user.ChangeHealth(-delta);
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