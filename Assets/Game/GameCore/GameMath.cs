using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Game
{
    public static class GameMath
    {
        /// <summary>
        /// 计算属性伤害适应度
        /// </summary>
        /// <param name="atk"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalDamageElementFit(HuluData user, ElementEnum atk, ElementEnum def)
        {
            var fit = Global.Table.ElementFitTable.Get(atk).Fit;
            float value = fit.GetValueOrDefault(def, 1);
            if (user.ContainsBuff(BattleBuffEnum.技能效果不好时变成一点五倍) && Mathf.Approximately(value, 0.5f))
            {
                user.RemoveBuff(BattleBuffEnum.技能效果不好时变成一点五倍);
                value = 1.5f;
            }

            return value;
        }

        public static string CalElementFitText(HuluData user, ElementEnum atk, ElementEnum def)
        {
            float value = CalDamageElementFit(user, atk, def);

            switch (value)
            {
                case 0:
                    return "";
                case 0.5f:
                    return "效果不好";
                case 2:
                    return "十分有效";
            }

            throw new NotImplementedException();
        }


        public static (HuluData, HuluData) CompareSpeed(HuluData r, HuluData l)
        {
            if (r.speed > l.speed)
            {
                return (r, l);
            }

            if (r.speed < l.speed)
            {
                return (l, r);
            }

            if (UnityEngine.Random.value > 0.5f)
            {
                return (l, r);
            }

            return (r, l);
        }

        public static (HuluData, HuluData) WhoFirst(IBattleTrainer rT, IBattleTrainer lt, HuluData r, HuluData l,
            ActiveSkillData rs, ActiveSkillData ls,
            BattleData data)
        {
            Assert.IsTrue(rs.config.Type != ActiveSkillTypeEnum.指挥);
            Assert.IsTrue(ls.config.Type != ActiveSkillTypeEnum.指挥);

            if (r.ContainsBuff(BattleBuffEnum.轮滑技巧) && l.ContainsBuff(BattleBuffEnum.轮滑技巧))
            {
                Debug.Log($"{r} {l} 都有滑轮技巧");
                r.RemoveBuff(BattleBuffEnum.轮滑技巧);
                l.RemoveBuff(BattleBuffEnum.轮滑技巧);

                if (UnityEngine.Random.value > 0.5f)
                {
                    return (l, r);
                }

                return (r, l);
            }

            if (r.ContainsBuff(BattleBuffEnum.轮滑技巧))
            {
                r.RemoveBuff(BattleBuffEnum.轮滑技巧);
                Debug.Log($"{r} 有滑轮技巧");
                return (r, l);
            }

            if (l.ContainsBuff(BattleBuffEnum.轮滑技巧))
            {
                l.RemoveBuff(BattleBuffEnum.轮滑技巧);
                Debug.Log($"{l} 有滑轮技巧");
                return (l, r);
            }

            int rPriority = UglyMath.PostprocessPriority(r, rs);
            int lPriority = UglyMath.PostprocessPriority(l, ls);
            if (rPriority > lPriority)
            {
                return (r, l);
            }

            if (rPriority < lPriority)
            {
                return (l, r);
            }

            int rRuntimeSpeed = (int)UglyMath.PostprocessRunTimeSpeed(rT, r, data);
            int lRuntimeSpeed = (int)UglyMath.PostprocessRunTimeSpeed(lt, l, data);

            if (rRuntimeSpeed > lRuntimeSpeed)
            {
                return (r, l);
            }

            if (rRuntimeSpeed < lRuntimeSpeed)
            {
                return (l, r);
            }

            if (UnityEngine.Random.value > 0.5f)
            {
                return (l, r);
            }

            return (r, l);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CalSelfElementFit(HuluConfig huluConfig, ActiveSkillConfig skillConfig)
        {
            if (huluConfig.Elements == skillConfig.Element)
            {
                return 1.2f;
            }

            return 1;
        }


        // https://y0akss0ko93.feishu.cn/wiki/NHMiwoQNPiaRmJkfba5c0celncn?from=from_copylink
        // 选手最终生命值HpF = 选手的生命值Hp；
        // 选手每次技能能造成的伤害Dmg = （己方选手的攻击力Att + 此次使用的技能威力SkDmg - 被攻击的敌方选手的防御力Def） * 本系威力加成1.2【使用的技能属性与己方选手的属性一致】 * 属性克制0.5/2【效果不好为0.5/十分有效为2】；
        // 选手技能最终造成的伤害DmgF = 选手每次技能能造成的伤害Dmg * （1 - 被攻击的敌方选手的适应力Prop / 100）【即适应力百分比，比如适应力为20，那最后得到的数值就是20%，参与计算时的伤害就会变成原本伤害的80%】。

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalDamage(HuluData atk, HuluData def, ActiveSkillEnum skill,
            BattleData data)
        {
            ActiveSkillConfig config = Global.Table.ActiveSkillTable.Get(skill);
            Assert.IsTrue(config.DamagePoint != 0);

            Assert.IsTrue(config.Type == ActiveSkillTypeEnum.伤害技能);
            int damagePoint = UglyMath.PostprocessDamagePoint(config, data);
            int atkPoint = UglyMath.PostprocessAtkPoint(atk, config, data);
            Debug.Log(
                $"攻击力{atkPoint},伤害{damagePoint},防御力{def.currentDef}" +
                $" 本系威力加成{CalSelfElementFit(atk.config, config)} \n" +
                $"属性克制{CalDamageElementFit(atk, config.Element, def.config.Elements)}\n");
            float baseValue = damagePoint * atkPoint / (float)def.currentDef
                              *
                              CalSelfElementFit(atk.config, config) // 本系威力加成
                              *
                              CalDamageElementFit(atk, config.Element, def.config.Elements // 属性克制
                              );
            Debug.Log($"处理前的基础伤害{baseValue} ");
            baseValue = UglyMath.PostprocessBattleBaseValue(baseValue, atk, def, config);

            Debug.Log($"基础伤害{baseValue} 适应度{def.currentAdap}");
            float finalValue = baseValue * (1 - Mathf.Clamp(def.currentAdap, 0, 100) / 100f);

            finalValue = UglyMath.PostprocessBattleFinalValue(finalValue, atk, def, config);

            return (int)finalValue;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CalHit(HuluData atk, HuluData def, ActiveSkillEnum dataID,
            BattleData data)
        {
            ActiveSkillConfig config = Global.Table.ActiveSkillTable.Get(dataID);
            return UnityEngine.Random.value <= config.HitRate;
        }

        public static void ProcessBuffWhenRoundEnd(List<BattleBuffEnum> buffList)
        {
            HashSet<BattleBuffEnum> contains = HashSetPool<BattleBuffEnum>.Get();
            foreach (var buffEnum in buffList)
            {
                contains.Add(buffEnum);
            }

            foreach (var buffEnum in contains)
            {
                var buffConfig = Global.Table.BattleBuffTable.Get(buffEnum);
                int removeCnt = buffConfig.RemoveCountWhenRoundEnd;
                if (removeCnt == -1)
                {
                    buffList.RemoveAll(x => x == buffEnum);
                    continue;
                }

                for (int i = 0; i < removeCnt; i++)
                {
                    buffList.Remove(buffEnum);
                }
            }

            HashSetPool<BattleBuffEnum>.Release(contains);
        }

        public static int CalAtkTimes(HuluData user, ActiveSkillConfig skillCfg)
        {
            int times = 0;
            if (user.ContainsBuff(BattleBuffEnum.连续技能必定打最多次))
            {
                times = skillCfg.MulAttackTimes[1];
                user.RemoveBuff(BattleBuffEnum.连续技能必定打最多次);
            }
            else
            {
                times = UnityEngine.Random.Range(skillCfg.MulAttackTimes[0],
                    skillCfg.MulAttackTimes[1]);
            }

            return times;
        }

        public static float CalDefDiscardCardRateWhenHitted(IBattleTrainer atkTrainer, IBattleTrainer defTrainer,
            ActiveSkillConfig skill)
        {
            float discardRate = skill.DefDiscardCardRateWhenHitted;
            if (atkTrainer.currentBattleData.ContainsBuff(BattleBuffEnum.下一次技能让对方受伤时弃牌概率变成1))
            {
                discardRate = 1;
                atkTrainer.currentBattleData.RemoveBuff(BattleBuffEnum.下一次技能让对方受伤时弃牌概率变成1);
            }

            return discardRate;
        }

        public static async UniTask<IBattleOperation> ProcessOperationBeforeRounding(IBattleTrainer trainer,
            IBattleOperation operation)
        {
            if (trainer.buffList.Contains(BattleBuffEnum.没有手牌时当前宝可梦生命值归0) && trainer.handZone.Count <= 0)
            {
                Debug.Log("没有手牌时当前宝可梦生命值归0");
                await trainer.currentBattleData.DecreaseHealth(trainer.currentBattleData.currentHp);
            }

            if (operation is EndRoundOperation && trainer.buffList.Contains(BattleBuffEnum.回合结束后额外获得一个回合))
            {
                Debug.Log("回合结束后额外获得一个回合");
                await trainer.RemoveBuff(BattleBuffEnum.回合结束后额外获得一个回合);
                Global.Table.BattleBuffTable.Get(BattleBuffEnum.回合结束后额外获得一个回合);
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                return null;
            }

            return operation;
        }

        public static async UniTask ProcessTrainerAfterUseCardFromHandZone(IBattleTrainer userTrainer)
        {
            if (userTrainer.buffList.Contains(BattleBuffEnum.出牌时有40概率受到50点伤害))
            {
                var buffConfig = Global.Table.BattleBuffTable.Get(BattleBuffEnum.出牌时有40概率受到50点伤害);
                if (Random.value < buffConfig.TriggerRate)
                {
                    await userTrainer.currentBattleData.DecreaseHealth(buffConfig.DamageForCurrentPokemon);
                }
            }
        }
    }
}