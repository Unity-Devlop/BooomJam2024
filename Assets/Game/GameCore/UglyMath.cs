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
                Debug.Log($"枯木逢春");
                Global.Event.Send(new BattleTipEvent("枯木逢春"));
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
            int damagePoint)
        {
            if (atk.id == HuluEnum.毒宝宝 && atk.passiveSkillConfig.Id == PassiveSkillEnum.毒素治疗 &&
                skill.Element == ElementEnum.毒)
            {
                int heal = (int)(damagePoint * 0.3f);
                await atk.DecreaseHealth(heal);
                return;
            }

            else if (skill.Id == ActiveSkillEnum.吞吐 && Random.value <= 0.4)
            {
                Global.Event.Send(new BattleTipEvent("吞吐"));
                await defTrainer.RandomDiscard(2);
                return;
            }

            else if (skill.Id == ActiveSkillEnum.火焰冲)
            {
                Debug.Log("火焰冲 +10");
                Global.Event.Send(new BattleTipEvent("火焰冲 +10"));
                await atk.IncreaseAtk(10);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                return;
            }
            else if (skill.Id == ActiveSkillEnum.扎根)
            {
                Debug.Log("扎根 -10");
                Global.Event.Send(new BattleTipEvent("扎根 -10"));
                await atk.DecreaseSpeed(10);
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                return;
            }
            else if (skill.Id == ActiveSkillEnum.滚动)
            {
                Debug.Log("滚动 +10");
                Global.Event.Send(new BattleTipEvent("滚动 +10"));
                await atk.IncreaseDef(10);
                return;
            }
            else if (skill.Id == ActiveSkillEnum.轰隆隆隆隆)
            {
                Debug.Log($"轰隆隆隆隆 反伤:{damagePoint / 2}");
                Global.Event.Send(new BattleTipEvent($"轰隆隆隆隆 反伤:{damagePoint / 2}"));
                await atk.DecreaseHealth(damagePoint / 2);
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
                baseValue /= GameMath.CalDamageElementFit(atk.elementEnum, def.elementEnum);
                return baseValue;
            }
            else if (atkSkill.Id == ActiveSkillEnum.喙啄 && Random.value < 0.2f)
            {
                Debug.Log("喙啄");
                baseValue *= 1.5f;
                return baseValue;
            }
            else if (atkSkill.Id == ActiveSkillEnum.吐火 && atk.currentHp >= atk.hp)
            {
                Debug.Log("吐火");
                baseValue *= 1.5f;
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

        public static async UniTask<IBattleOperation> PostprocessHuluDataWhenHealthChange(HuluData hulu,
            IBattleTrainer trainer)
        {
            IBattleOperation operation;
            if (hulu.id == HuluEnum.电电鼠 && hulu.passiveSkillConfig.Id == PassiveSkillEnum.胆小鬼 &&
                hulu.buffList.Contains(BuffEnum.胆小鬼) && hulu.currentHp < hulu.hp / 2)
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
                    hulu.buffList.Remove(BuffEnum.胆小鬼);
                    hulu.buffList.Add(BuffEnum.胆小鬼归来);
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
                await huluData.DecreaseHealth(huluData.hp / 2);
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
            if (p0.id == HuluEnum.疾风之翼 && p0.passiveSkillConfig.Id == PassiveSkillEnum.顺风 &&
                rs.config.Element == ElementEnum.风)
            {
                return rs.config.Priority + 1;
            }

            return rs.config.Priority;
        }

        public static async UniTask PostprocessHuluEnterBattle(HuluData next)
        {
            // Debug.Log($"{next}进入战场 times:{next.enterTimes}");
            if (next.id == HuluEnum.电电鼠 && next.passiveSkillConfig.Id == PassiveSkillEnum.胆小鬼 && next.enterTimes == 1)
            {
                next.buffList.Add(BuffEnum.胆小鬼);
                return;
            }

            if (next.buffList.Contains(BuffEnum.胆小鬼归来))
            {
                next.buffList.Remove(BuffEnum.胆小鬼归来);
                Debug.Log("胆小鬼归来");
                Global.Event.Send(new BattleTipEvent($"{next}胆小鬼归来"));
                next.hp = (int)(next.hp * 1.5f);
                next.atk = (int)(next.atk * 1.5f);
                next.def = (int)(next.def * 1.5f);
                next.speed = (int)(next.speed * 1.5f);
                next.adap = (int)(next.adap * 1.5f);

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
            if (config.Id == ActiveSkillEnum.轰隆隆隆隆 && environmentData.id == BattleEnvironmentEnum.草地)
            {
                Global.Event.Send(new BattleTipEvent("轰隆隆隆隆"));
                Debug.Log("轰隆隆隆隆");
                return config.DamagePoint + 30;
            }

            return config.DamagePoint;
        }
    }
}