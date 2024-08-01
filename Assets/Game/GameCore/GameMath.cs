﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using cfg;
using UnityEngine;
using UnityEngine.Assertions;

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
        public static float CalDamageElementFit(ElementEnum atk, ElementEnum def)
        {
            var fit = Global.Table.ElementFitTable.Get(atk).Fit;
            return fit.GetValueOrDefault(def, 1);
        }

        public static string CalElementFitText(ElementEnum atk, ElementEnum def)
        {
            var fit = Global.Table.ElementFitTable.Get(atk).Fit;
            float value = fit.GetValueOrDefault(def, 1);

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

        public static (HuluData, HuluData) WhoFirst(HuluData r, HuluData l, ActiveSkillData rs, ActiveSkillData ls,
            BattleEnvironmentData environmentData)
        {
            Assert.IsTrue(rs.config.Type != ActiveSkillTypeEnum.指挥);
            Assert.IsTrue(ls.config.Type != ActiveSkillTypeEnum.指挥);

            if (r.buffList.Contains(BuffEnum.滑轮技巧) && l.buffList.Contains(BuffEnum.滑轮技巧))
            {
                Debug.Log($"{r} {l} 都有滑轮技巧");
                r.buffList.Remove(BuffEnum.滑轮技巧);
                l.buffList.Remove(BuffEnum.滑轮技巧);
                
                if (UnityEngine.Random.value > 0.5f)
                {
                    return (l, r);
                }

                return (r, l);
            }

            if (r.buffList.Contains(BuffEnum.滑轮技巧))
            {
                r.buffList.Remove(BuffEnum.滑轮技巧);
                Debug.Log($"{r} 有滑轮技巧");
                return (r, l);
            }

            if (l.buffList.Contains(BuffEnum.滑轮技巧))
            {
                l.buffList.Remove(BuffEnum.滑轮技巧);
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

            int rRuntimeSpeed = (int)UglyMath.PostprocessRunTimeSpeed(r, environmentData);
            int lRuntimeSpeed = (int)UglyMath.PostprocessRunTimeSpeed(l, environmentData);

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
            BattleEnvironmentData environmentData)
        {
            ActiveSkillConfig config = Global.Table.ActiveSkillTable.Get(skill);
            Assert.IsTrue(config.DamagePoint != 0);

            Assert.IsTrue(config.Type == ActiveSkillTypeEnum.伤害技能);
            int damagePoint = UglyMath.PostprocessDamagePoint(config, environmentData);
            int atkPoint = UglyMath.PostprocessAtkPoint(atk, config, environmentData);
            Debug.Log(
                $"攻击力{atkPoint},伤害{damagePoint},防御力{def.currentDef}" +
                $" 本系威力加成{CalSelfElementFit(atk.config, config)} " +
                $"属性克制{CalDamageElementFit(config.Element, def.config.Elements)}");
            float baseValue = (atkPoint + damagePoint - def.currentDef)
                              *
                              CalSelfElementFit(atk.config, config) // 本系威力加成
                              *
                              CalDamageElementFit(config.Element, def.config.Elements // 属性克制
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
            BattleEnvironmentData environmentData)
        {
            ActiveSkillConfig config = Global.Table.ActiveSkillTable.Get(dataID);
            return UnityEngine.Random.value <= config.HitRate;
        }
    }
}