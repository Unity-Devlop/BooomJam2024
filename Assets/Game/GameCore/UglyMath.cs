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
        public static async UniTask EffectWhenSkillHitted(IBattleTrainer atkTrainer,
            IBattleTrainer defTrainer,
            ActiveSkillConfig skill,
            int damagePoint, BattleEnvData envData)
        {
            var atk = atkTrainer.currentBattleData;

            float defDiscardCardRateWhenHitted =
                GameMath.CalDefDiscardCardRateWhenHitted(atkTrainer, defTrainer, skill);
            if (Random.value < defDiscardCardRateWhenHitted)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"弃牌生效 {defTrainer}弃{skill.DefDiscardCountWhenHitted} 张"));
                await defTrainer.RandomDiscardCardFromHand(skill.DefDiscardCountWhenHitted);
            }

            if (skill.IncreaseSelfSpeedPointAfterUse != 0)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"速度+{skill.IncreaseSelfSpeedPointAfterUse}"));
                await atk.IncreaseCurrentSpeed(skill.IncreaseSelfSpeedPointAfterUse);
            }

            if (skill.IncreaseSelfDefPointAfterUse != 0)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"防御+{skill.IncreaseSelfDefPointAfterUse}"));
                await atk.IncreaseDef(skill.IncreaseSelfDefPointAfterUse);
            }

            if (skill.PercentageDamageBySelf != 0)
            {
                Global.Event.Send(
                    new BattleInfoRecordEvent($"对自己反伤造成{damagePoint * skill.PercentageDamageBySelf}点伤害"));
                await atk.TakeDamageFromSelfSkillEffect((int)(damagePoint * skill.PercentageDamageBySelf));
            }

            if (skill.ChangeElementAfterUse != ElementEnum.None)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk} 变成{skill.ChangeElementAfterUse}属性"));
                await atk.ChangeElement(skill.ChangeElementAfterUse);
            }

            if (atk.id == HuluEnum.毒宝宝 && atk.passiveSkillConfig.Id == PassiveSkillEnum.毒素治疗 &&
                skill.Element == ElementEnum.毒)
            {
                int heal = (int)(damagePoint * 0.3f);
                await atk.DecreaseHealth(heal);
            }
        }

        public static async UniTask PostprocessHuluDataWhenUseSkill(HuluData atk, ActiveSkillConfig skill)
        {
            if (atk.id == HuluEnum.噼啪小将 && atk.passiveSkillConfig.Id == PassiveSkillEnum.噼里啪啦 &&
                skill.Element == ElementEnum.电)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{PassiveSkillEnum.噼里啪啦}"));
                await atk.IncreaseAtk(10);
                atk.currentAtk = Mathf.Clamp(atk.currentAtk, 0, 30 + atk.atk);
                return;
            }
        }

        public static async UniTask<float> PostprocessBattleBaseValue(float baseValue, HuluData atk, HuluData def,
            ActiveSkillConfig atkSkill)
        {
            while (atk.ContainsBuff(BattleBuffEnum.下一次伤害加80))
            {
                await atk.RemoveBuff(BattleBuffEnum.下一次伤害加80);
                baseValue += 80;
            }

            // Buff
            if (atk.ContainsBuff(BattleBuffEnum.寻找弱点))
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}寻找弱点"));
                baseValue *= 1.5f;
                await atk.RemoveBuff(BattleBuffEnum.寻找弱点);
            }

            if (atk.ContainsBuff(BattleBuffEnum.下一次技能伤害两倍))
            {
                await atk.RemoveBuff(BattleBuffEnum.下一次技能伤害两倍);
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}下一次技能伤害两倍"));
                baseValue *= 2;
            }


            if (atk.ContainsBuff(BattleBuffEnum.技能造成的伤害变成优先级倍))
            {
                await atk.RemoveBuff(BattleBuffEnum.技能造成的伤害变成优先级倍);
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}技能造成的伤害变成优先级倍"));
                // TODO 狗策划
                int priority = Mathf.Clamp(atkSkill.Priority, 1, int.MaxValue);
                baseValue *= priority;
            }

            if (atkSkill.FullHpIncreaseBaseValueRate != 0 && atk.currentHp >= atk.hp)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}满血加成{atkSkill.FullHpIncreaseBaseValueRate}"));
                baseValue *= (1 + atkSkill.FullHpIncreaseBaseValueRate);
            }

            if (Random.value < atkSkill.EffectHitRate)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}技能{atkSkill}命中要害"));
                baseValue *= 1.5f;
            }

            if (atk.id == HuluEnum.怒潮龙 && atk.passiveSkillConfig.Id == PassiveSkillEnum.怒火喷发 && atk.currentHp == atk.hp)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}怒火喷发"));
                baseValue *= 1.5f;
            }
            else if (atk.id == HuluEnum.烈火领主 && atk.passiveSkillConfig.Id == PassiveSkillEnum.火焰共鸣)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}火焰共鸣"));
                baseValue = baseValue / GameMath.CalSelfElementFit(atk.config, atkSkill) * 1.5f;
            }
            else if (def.id == HuluEnum.吞火熊 && def.passiveSkillConfig.Id == PassiveSkillEnum.内敛 &&
                     atkSkill.Element == ElementEnum.水)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}内敛"));
                baseValue /= await GameMath.CalDamageElementFit(atk, atkSkill.Element, def.elementEnum);
            }

            return baseValue;
        }

        public static async UniTask<bool> PostprocessHitRate(HuluData atk, HuluData def, ActiveSkillEnum atkSkill,
            BattleEnvData envData)
        {
            bool res = true; // 最终是否能命中
            if (def.ContainsBuff(BattleBuffEnum.守护))
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{def}守护中，无法被攻击"));
                return false;
            }

            if (res && def.ContainsBuff(BattleBuffEnum.快躲开))
            {
                await def.RemoveBuff(BattleBuffEnum.快躲开);
                res &= !(Random.value < 0.6f);
                if (!res)
                {
                    Global.Event.Send(new BattleInfoRecordEvent($"{atk}快躲开"));
                    Debug.Log("快躲开");
                }
            }

            if (res && def.id == HuluEnum.一口鲸 && def.passiveSkillConfig.Id == PassiveSkillEnum.大口吃)
            {
                res &= !(Random.value < 0.2f); // 生效了就不命中
                if (!res)
                {
                    Global.Event.Send(new BattleInfoRecordEvent($"{def}大口吃 规避技能"));
                }
            }

            return res;
        }

        public static float PostprocessRunTimeSpeed(IBattleTrainer user,
            BattleEnvData envData)
        {
            var pokemon = user.currentBattleData;
            float speed = pokemon.currentSpeed;

            if (user.ContainsBuff(BattleBuffEnum.顺风))
            {
                Debug.Log($"{pokemon}顺风+10");
                speed += 10;
            }

            if (user.ContainsBuff(BattleBuffEnum.逆风))
            {
                Debug.Log($"{pokemon}逆风-10");
                speed -= 10;
            }

            switch (envData.id)
            {
                case BattleEnvironmentEnum.草地:
                    if (pokemon.id == HuluEnum.推土牛 && pokemon.passiveSkillConfig.Id == PassiveSkillEnum.轰隆冲击)
                    {
                        Debug.Log("轰隆冲击");
                        speed = pokemon.currentSpeed * 2f;
                    }

                    break;
                case BattleEnvironmentEnum.沙漠:
                    break;
                case BattleEnvironmentEnum.海洋:
                    if (pokemon.elementEnum == ElementEnum.水)
                    {
                        speed = pokemon.currentSpeed * 1.05f;
                    }

                    break;
                case BattleEnvironmentEnum.火山:
                    break;
                case BattleEnvironmentEnum.雪地:
                    speed = pokemon.currentSpeed * 0.9f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return speed;
        }

        public static async UniTask<IBattleOperation> CalNewOperWhenPokemonHealthChange(IBattleTrainer defTrainer)
        {
            var hulu = defTrainer.currentBattleData;

            IBattleOperation operation;
            if (hulu.id == HuluEnum.电电鼠 && hulu.passiveSkillConfig.Id == PassiveSkillEnum.胆小鬼 &&
                hulu.ContainsBuff(BattleBuffEnum.胆小鬼) && hulu.currentHp < hulu.hp / 2 && hulu.currentHp > 0)
            {
                int tar = -1;
                for (int i = 0; i < defTrainer.trainerData.datas.Count; i++)
                {
                    if (defTrainer.trainerData.datas[i] == hulu)
                    {
                        continue;
                    }

                    if (defTrainer.trainerData.datas[i].hp < 0)
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
                    Global.Event.Send(new BattleInfoRecordEvent($"{hulu} 发动 胆小鬼"));
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                    await hulu.RemoveBuff(BattleBuffEnum.胆小鬼);
                    await hulu.AddBuff(BattleBuffEnum.胆小鬼归来);
                    return operation;
                }
            }

            return null;
        }


        public static async UniTask PostprocessHuluDataBeforeUseSkill(HuluData atk, ActiveSkillConfig skill)
        {
            if (atk.id == HuluEnum.小闪光 && atk.passiveSkillConfig.Id == PassiveSkillEnum.集合体 && atk.skillTimes == 0)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}发动集合体 变换属性{skill.Element}"));
                atk.elementEnum = skill.Element;
                atk.skillTimes++;
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
                await next.AddBuff(BattleBuffEnum.胆小鬼);
            }

            if (next.id == HuluEnum.斯托姆 && next.passiveSkillConfig.Id == PassiveSkillEnum.狂风不灭 && next.enterTimes == 1)
            {
                await next.AddBuff(BattleBuffEnum.狂风不灭);
            }

            if (next.ContainsBuff(BattleBuffEnum.胆小鬼归来))
            {
                await next.RemoveBuff(BattleBuffEnum.胆小鬼归来);
                Global.Event.Send(new BattleInfoRecordEvent($"{next}胆小鬼归来 全属性+50%"));
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
            }

            await next.bind.Invoke();
        }

        public static int PostprocessDamagePoint(ActiveSkillConfig config, BattleEnvData envData)
        {
            if (envData.id == BattleEnvironmentEnum.草地 && config.IncreaseDamagePointWhenGrassEnv != 0)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"草地增伤:{config.IncreaseDamagePointWhenGrassEnv}"));
                return config.DamagePoint + config.IncreaseDamagePointWhenGrassEnv;
            }

            return config.DamagePoint;
        }

        public static async UniTask<int> PostprocessAtkPoint(HuluData atk, ActiveSkillConfig config,
            BattleEnvData envData)
        {
            //TODO 狗策划 边际情况 
            if (atk.ContainsBuff(BattleBuffEnum.用速度代替攻击力进行伤害计算))
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}使用速度代替攻击力计算伤害"));
                await atk.RemoveBuff(BattleBuffEnum.用速度代替攻击力进行伤害计算);
                return atk.currentSpeed;
            }

            if (config.UsingDefToCalDamage)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{atk}使用防御力计算伤害"));
                return atk.currentDef;
            }

            return atk.currentAtk;
        }

        public static float PostprocessBattleFinalValue(float finalValue, HuluData atk, HuluData def,
            ActiveSkillConfig config)
        {
            float res = finalValue;


            return res;
        }
    }
}