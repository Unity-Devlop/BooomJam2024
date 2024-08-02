﻿using System;
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
        public static async UniTask PostprocessHuluData(HuluData data)
        {
            if (data.Contains(BattleBuffEnum.站起来) && data.currentHp <= 0)
            {
                Global.Event.Send(new BattleTipEvent("站起来"));
                Debug.Log("站起来");
                data.Remove(BattleBuffEnum.站起来);
                await data.DecreaseHealth(-1);
            }

            if (data.id == HuluEnum.枯木妖 && data.passiveSkillConfig.Id == PassiveSkillEnum.枯木逢春)
            {
                Debug.Log($"枯木逢春");
                Global.Event.Send(new BattleTipEvent("枯木逢春"));
                int damageHp = data.hp - data.currentHp;
                int cnt = damageHp / 100;
                int adapIncreate = cnt * 5;
                adapIncreate = Mathf.Clamp(adapIncreate, 0, 20);
                data.currentAdap = data.adap;
                data.currentAdap += adapIncreate;
            }
        }

        public static async UniTask PostprocessHuluDataWhenAfterUseSkill(IBattleTrainer atkTrainer, HuluData atk,
            IBattleTrainer defTrainer,
            ActiveSkillConfig skill,
            int damagePoint, BattleEnvironmentData environmentData)
        {
            if (Random.value < skill.DefDiscardCardRate)
            {
                Global.Event.Send(new BattleTipEvent($"{skill} 弃牌生效 {defTrainer}弃{skill.DefDiscardCount} 张"));
                await defTrainer.RandomDiscard(skill.DefDiscardCount);
            }

            if (skill.IncreaseSelfSpeedPointAfterUse != 0)
            {
                Global.Event.Send(new BattleTipEvent($"{skill} 速度+{skill.IncreaseSelfSpeedPointAfterUse}"));
                await atk.IncreaseCurrentSpeed(skill.IncreaseSelfSpeedPointAfterUse);
            }

            if (skill.IncreaseSelfDefPointAfterUse != 0)
            {
                Global.Event.Send(new BattleTipEvent($"{skill} 防御+{skill.IncreaseSelfDefPointAfterUse}"));
                await atk.IncreaseDef(skill.IncreaseSelfDefPointAfterUse);
            }

            if (skill.PercentageDamageBySelf != 0)
            {
                Global.Event.Send(
                    new BattleTipEvent($"{skill} 对自己反伤造成{damagePoint * skill.PercentageDamageBySelf}点伤害"));
                await atk.DecreaseHealth((int)(damagePoint * skill.PercentageDamageBySelf));
            }

            if (skill.ChangeElementAfterUse != ElementEnum.None)
            {
                Global.Event.Send(new BattleTipEvent($"{atk} 变成{skill.ChangeElementAfterUse}属性"));
                await atk.ChangeElement(skill.ChangeElementAfterUse);
            }

            if (atk.id == HuluEnum.毒宝宝 && atk.passiveSkillConfig.Id == PassiveSkillEnum.毒素治疗 &&
                skill.Element == ElementEnum.毒)
            {
                int heal = (int)(damagePoint * 0.3f);
                await atk.DecreaseHealth(heal);
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
            ActiveSkillConfig atkSkill)
        {
            // Buff
            if (atk.Contains(BattleBuffEnum.寻找弱点))
            {
                Debug.Log("寻找弱点");
                Global.Event.Send(new BattleTipEvent("寻找弱点"));
                baseValue *= 1.5f;
            }

            if (atkSkill.FullHpIncreaseBaseValueRate != 0 && atk.currentHp >= atk.hp)
            {
                Debug.Log($"满血加成{atkSkill.FullHpIncreaseBaseValueRate}");
                Global.Event.Send(new BattleTipEvent($"满血加成{atkSkill.FullHpIncreaseBaseValueRate}"));
                baseValue *= (1 + atkSkill.FullHpIncreaseBaseValueRate);
            }

            if (Random.value < atkSkill.EffectHitRate)
            {
                Debug.Log($"技能{atkSkill}命中要害");
                Global.Event.Send(new BattleTipEvent($"技能{atkSkill}命中要害"));
                baseValue *= 1.5f;
            }

            if (atk.id == HuluEnum.怒潮龙 && atk.passiveSkillConfig.Id == PassiveSkillEnum.怒火喷发 && atk.currentHp == atk.hp)
            {
                Debug.Log("怒火喷发");
                Global.Event.Send(new BattleTipEvent("怒火喷发"));
                baseValue *= 1.5f;
            }
            else if (atk.id == HuluEnum.烈火领主 && atk.passiveSkillConfig.Id == PassiveSkillEnum.火焰共鸣)
            {
                Debug.Log("火焰共鸣");
                Global.Event.Send(new BattleTipEvent("火焰共鸣"));
                baseValue = baseValue / GameMath.CalSelfElementFit(atk.config, atkSkill) * 1.5f;
            }
            else if (def.id == HuluEnum.吞火熊 && def.passiveSkillConfig.Id == PassiveSkillEnum.内敛 &&
                     atkSkill.Element == ElementEnum.水)
            {
                Debug.Log("内敛");
                Global.Event.Send(new BattleTipEvent("内敛"));
                baseValue /= GameMath.CalDamageElementFit(atkSkill.Element, def.elementEnum);
                return baseValue;
            }

            return baseValue;
        }

        public static bool PostprocessHitRate(HuluData atk, HuluData def, ActiveSkillEnum atkSkill,
            BattleEnvironmentData environmentData)
        {
            bool res = true; // 最终是否能命中
            if (def.Contains(BattleBuffEnum.守护))
            {
                Global.Event.Send(new BattleTipEvent($"{def}守护中，无法被攻击"));
                Debug.Log("守护");
                return false;
            }

            if (res && def.Contains(BattleBuffEnum.快躲开))
            {
                def.Remove(BattleBuffEnum.快躲开);
                res &= !(Random.value < 0.6f);
                if (!res)
                {
                    Global.Event.Send(new BattleTipEvent("快躲开"));
                    Debug.Log("快躲开");
                }
            }

            if (res && atk.id == HuluEnum.一口鲸 && atk.passiveSkillConfig.Id == PassiveSkillEnum.大口吃)
            {
                Debug.Log("大口吃");
                res &= !(Random.value < 0.2f); // 生效了就不命中
                if (!res)
                {
                    Global.Event.Send(new BattleTipEvent("大口吃"));
                    Debug.Log("大口吃");
                }
            }

            return res;
        }

        public static float PostprocessRunTimeSpeed(IBattleTrainer user, HuluData userHulu,
            BattleEnvironmentData environmentData)
        {
            float speed = userHulu.currentSpeed;

            if (environmentData.GetBuff(user).buffList.Contains(BattleBuffEnum.顺风))
            {
                Debug.Log($"{userHulu}顺风+10");
                speed += 10;
            }

            if (environmentData.GetBuff(user).buffList.Contains(BattleBuffEnum.逆风))
            {
                Debug.Log($"{userHulu}逆风-10");
                speed -= 10;
            }

            switch (environmentData.id)
            {
                case BattleEnvironmentEnum.草地:
                    if (userHulu.id == HuluEnum.推土牛 && userHulu.passiveSkillConfig.Id == PassiveSkillEnum.轰隆冲击)
                    {
                        Debug.Log("轰隆冲击");
                        speed = userHulu.currentSpeed * 2f;
                    }

                    break;
                case BattleEnvironmentEnum.沙漠:
                    break;
                case BattleEnvironmentEnum.海洋:
                    if (userHulu.elementEnum == ElementEnum.水)
                    {
                        speed = userHulu.currentSpeed * 1.05f;
                    }

                    break;
                case BattleEnvironmentEnum.火山:
                    break;
                case BattleEnvironmentEnum.雪地:
                    speed = userHulu.currentSpeed * 0.9f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return speed;
        }

        public static async UniTask<IBattleOperation> PostprocessHuluDataWhenHealthChange(HuluData hulu,
            IBattleTrainer trainer)
        {
            IBattleOperation operation;
            if (hulu.id == HuluEnum.电电鼠 && hulu.passiveSkillConfig.Id == PassiveSkillEnum.胆小鬼 &&
                hulu.Contains(BattleBuffEnum.胆小鬼) && hulu.currentHp < hulu.hp / 2 && hulu.currentHp > 0)
            {
                int tar = -1;
                for (int i = 0; i < trainer.trainerData.datas.Count; i++)
                {
                    if (trainer.trainerData.datas[i] == hulu)
                    {
                        continue;
                    }

                    if (trainer.trainerData.datas[i].hp < 0)
                    {
                        continue;
                    }

                    tar = i;
                }

                if (tar != -1)
                {
                    operation = new ChangeHuluOperation()
                    {
                        next = tar
                    };
                    Debug.Log("胆小鬼触发！");
                    Global.Event.Send(new BattleTipEvent($"{hulu}胆小鬼"));
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                    hulu.Remove(BattleBuffEnum.胆小鬼);
                    hulu.Add(BattleBuffEnum.胆小鬼归来);
                    return operation;
                }
            }

            return null;
        }

        public static async UniTask PostprocessHuluDataWhenDead(HuluData huluData)
        {
            if (huluData.id == HuluEnum.斯托姆 && huluData.passiveSkillConfig.Id == PassiveSkillEnum.狂风不灭 &&
                huluData.canReborn)
            {
                Debug.Log("狂风不灭");
                await huluData.DecreaseHealth(-huluData.hp / 2);
                huluData.canReborn = false;
                await huluData.bind.Invoke();
                return;
            }
        }

        public static async UniTask PostprocessHuluDataBeforeUseSkill(HuluData atk, ActiveSkillConfig skill)
        {
            if (atk.id == HuluEnum.小闪光 && atk.passiveSkillConfig.Id == PassiveSkillEnum.集合体 && atk.skillTimes == 0)
            {
                Global.Event.Send(new BattleTipEvent("集合体"));
                Debug.Log("集合体");
                atk.elementEnum = skill.Element;
                atk.skillTimes++;
                return;
            }
        }

        public static int PostprocessPriority(HuluData p0, ActiveSkillData rs)
        {
            int priority = rs.config.Priority;
            if (p0.id == HuluEnum.疾风之翼 && p0.passiveSkillConfig.Id == PassiveSkillEnum.顺风 &&
                rs.config.Element == ElementEnum.风)
            {
                priority += 1;
            }

            return priority;
        }

        public static async UniTask PostprocessHuluEnterBattle(HuluData next)
        {
            // Debug.Log($"{next}进入战场 times:{next.enterTimes}");
            if (next.id == HuluEnum.电电鼠 && next.passiveSkillConfig.Id == PassiveSkillEnum.胆小鬼 && next.enterTimes == 1)
            {
                next.Add(BattleBuffEnum.胆小鬼);
                return;
            }

            if (next.Contains(BattleBuffEnum.胆小鬼归来))
            {
                next.Remove(BattleBuffEnum.胆小鬼归来);
                Debug.Log("胆小鬼归来");
                Global.Event.Send(new BattleTipEvent($"{next}胆小鬼归来"));
                next.hp = (int)(next.hp * 1.5f);
                // next.atk = (int)(next.atk * 1.5f);
                // next.def = (int)(next.def * 1.5f);
                // next.speed = (int)(next.speed * 1.5f);
                // next.adap = (int)(next.adap * 1.5f);

                next.currentHp = (int)(next.currentHp * 1.5f);
                next.currentAtk = (int)(next.currentAtk * 1.5f);
                next.currentDef = (int)(next.currentDef * 1.5f);
                next.currentSpeed = (int)(next.currentSpeed * 1.5f);
                next.currentAdap = (int)(next.currentAdap * 1.5f);
                await next.bind.Invoke();
                return;
            }
        }

        public static int PostprocessDamagePoint(ActiveSkillConfig config, BattleEnvironmentData environmentData)
        {
            if (environmentData.id == BattleEnvironmentEnum.草地 && config.IncreaseDamagePointWhenGrassEnv != 0)
            {
                Global.Event.Send(new BattleTipEvent($"草地增伤:{config.IncreaseDamagePointWhenGrassEnv}"));
                return config.DamagePoint + config.IncreaseDamagePointWhenGrassEnv;
            }

            return config.DamagePoint;
        }

        public static int PostprocessAtkPoint(HuluData atk, ActiveSkillConfig config,
            BattleEnvironmentData environmentData)
        {
            if (config.UsingDefToCalDamage)
            {
                Global.Event.Send(new BattleTipEvent($"{config}使用防御力计算伤害"));
                return atk.currentDef;
            }

            return atk.currentAtk;
        }

        public static float PostprocessBattleFinalValue(float finalValue, HuluData atk, HuluData def,
            ActiveSkillConfig config)
        {
            float res = finalValue;
            if (def.Contains(BattleBuffEnum.规避弱点))
            {
                def.Remove(BattleBuffEnum.规避弱点);
                res *= 0.5f;
                Debug.Log("规避弱点");
            }

            return res;
        }
    }
}