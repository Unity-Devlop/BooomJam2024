using System;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public static class UglyMath
    {
        public static void PostprocessHuluData(HuluData data)
        {
            if (data.id == HuluEnum.枯木妖 && data.passiveSkillConfig.Id == PassiveSkillEnum.枯木逢春)
            {
                Debug.Log($"{data}-{data.passiveSkillConfig.Id}");
                int damageHp = data.hp - data.currentHp;
                int cnt = damageHp / 100;
                int adapIncreate = cnt * 5;
                adapIncreate = Mathf.Clamp(adapIncreate, 0, 20);
                data.currentAdap = data.adap;
                data.currentAdap += adapIncreate;

                return;
            }
        }

        public static async UniTask PostprocessHuluDataWhenAfterUseSkill(HuluData atk, IBattleTrainer defTrainer,
            ActiveSkillConfig skill,
            float damagePoint)
        {
            if (atk.id == HuluEnum.毒宝宝 && atk.passiveSkillConfig.Id == PassiveSkillEnum.毒素治疗 &&
                skill.Element == ElementEnum.毒)
            {
                int heal = (int)(damagePoint * 0.3f);
                await atk.ChangeHealth(heal);
                return;
            }

            if (skill.Id == ActiveSkillEnum.吞吐 && Random.value <= 0.4)
            {
                Debug.Log("吞吐");
                await defTrainer.RandomDiscard(4);
                return;
            }

            if (skill.Id == ActiveSkillEnum.火焰冲)
            {
                Debug.Log("火焰冲 +10");
                Global.Event.Send(new BattleTipEvent("火焰冲 +10"));
                await atk.ChangeAtk(10);
                return;
            }
        }

        public static void PostprocessHuluDataWhenUseSkill(HuluData atk, ActiveSkillConfig skill)
        {
            if (atk.id == HuluEnum.噼啪小将 && atk.passiveSkillConfig.Id == PassiveSkillEnum.噼里啪啦 &&
                skill.Element == ElementEnum.电)
            {
                Debug.Log("噼里啪啦");
                atk.currentAtk += 10;
                atk.currentAtk = Mathf.Clamp(atk.currentAtk, 0, 30 + atk.atk);
                return;
            }
        }

        public static float PostprocessBattleBaseValue(float baseValue, HuluData atk, HuluData def,
            ActiveSkillConfig config)
        {
            if (atk.id == HuluEnum.怒潮龙 && atk.passiveSkillConfig.Id == PassiveSkillEnum.怒火喷发 && atk.currentHp == atk.hp)
            {
                Debug.Log("怒火喷发");
                baseValue *= 1.5f;
            }
            else if (atk.id == HuluEnum.烈火领主 && atk.passiveSkillConfig.Id == PassiveSkillEnum.火焰共鸣)
            {
                Debug.Log("火焰共鸣");
                baseValue = baseValue / GameMath.CalSelfElementFit(atk.config, config) * 1.5f;
            }
            else if (def.id == HuluEnum.吞火熊 && def.passiveSkillConfig.Id == PassiveSkillEnum.内敛 &&
                     config.Element == ElementEnum.水)
            {
                Debug.Log("内敛");
                baseValue /= GameMath.CalDamageElementFit(atk.elementEnum, def.elementEnum);
                return baseValue;
            }

            return baseValue;
        }

        public static bool PostprocessHitRate(HuluData atk, HuluData def, ActiveSkillEnum atkSkill,
            BattleEnvironmentData environmentData)
        {
            if (def.buffList.Contains(BuffEnum.守护))
            {
                Global.Event.Send(new BattleTipEvent($"{def}守护中，无法被攻击"));
                Debug.Log("守护");
                def.buffList.Remove(BuffEnum.守护);
                return false;
            }

            if (atk.id == HuluEnum.一口鲸 && atk.passiveSkillConfig.Id == PassiveSkillEnum.大口吃 && Random.value < 0.2f)
            {
                Debug.Log("大口吃");
                return false;
            }

            return true;
        }

        public static float PostprocessRunTimeSpeed(HuluData p0, BattleEnvironmentData environmentData)
        {
            float speed = p0.currentSpeed;
            switch (environmentData.id)
            {
                case BattleEnvironmentEnum.草地:
                    if (p0.id == HuluEnum.推土牛 && p0.passiveSkillConfig.Id == PassiveSkillEnum.轰隆冲击)
                    {
                        Debug.Log("轰隆冲击");
                        speed = p0.currentSpeed * 2f;
                    }

                    break;
                case BattleEnvironmentEnum.沙漠:
                    break;
                case BattleEnvironmentEnum.海洋:
                    if (p0.elementEnum == ElementEnum.水)
                    {
                        speed = p0.currentSpeed * 1.05f;
                    }

                    break;
                case BattleEnvironmentEnum.火山:
                    break;
                case BattleEnvironmentEnum.雪地:
                    speed = p0.currentSpeed * 0.9f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return speed;
        }

        public static async UniTask PostprocessHuluDataWhenDead(HuluData huluData)
        {
            if (huluData.id == HuluEnum.斯托姆 && huluData.passiveSkillConfig.Id == PassiveSkillEnum.狂风不灭 &&
                huluData.canReborn)
            {
                Debug.Log("狂风不灭");
                await huluData.ChangeHealth(huluData.hp / 2);
                huluData.canReborn = false;
                await huluData.bind.Invoke();
                return;
            }
        }

        public static async UniTask PostprocessHuluDataBeforeUseSkill(HuluData atk, ActiveSkillConfig skill)
        {
            if (atk.id == HuluEnum.小闪光 && atk.passiveSkillConfig.Id == PassiveSkillEnum.集合体 && atk.skillTimes == 0)
            {
                Debug.Log("集合体");
                atk.elementEnum = skill.Element;
                atk.skillTimes++;
                return;
            }
        }

        public static int PostprocessPriority(HuluData p0, ActiveSkillData rs)
        {
            if (p0.id == HuluEnum.疾风之翼 && p0.passiveSkillConfig.Id == PassiveSkillEnum.顺风 &&
                rs.config.Element == ElementEnum.风)
            {
                return rs.config.Priority + 1;
            }

            return rs.config.Priority;
        }
    }
}