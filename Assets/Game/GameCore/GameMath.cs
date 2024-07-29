using System;
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
            if (rs.config.Priority > ls.config.Priority)
            {
                return (r, l);
            }

            if (rs.config.Priority < ls.config.Priority)
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
            Assert.IsTrue(config.Type == ActiveSkillTypeEnum.伤害技能);
            float baseValue = (atk.currentAtk + config.DamagePoint - def.currentDef)
                              *
                              CalSelfElementFit(atk.config, config) // 本系威力加成
                              *
                              CalDamageElementFit(atk.config.Elements, def.config.Elements // 属性克制
                              );
            baseValue = UglyMath.PostprocessBattleBaseValue(baseValue, atk, def, config);


            float finalValue = baseValue * (1 - Mathf.Clamp(def.currentAdap, 0, 100) / 100f);

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