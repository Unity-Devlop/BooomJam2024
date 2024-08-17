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
using UnityToolkit;
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
        public static async UniTask<float> CalDamageElementFit(HuluData user, ElementEnum atk, ElementEnum def)
        {
            var fit = Global.Table.ElementTable.Get(atk).Fit;
            float value = fit.GetValueOrDefault(def, 1);
            if (user.ContainsBuff(BattleBuffEnum.技能效果不好时变成一点五倍) && Mathf.Approximately(value, 0.5f))
            {
                await user.RemoveBuff(BattleBuffEnum.技能效果不好时变成一点五倍);
                value = 1.5f;
            }

            return value;
        }

        public static float CalElementFit(ElementEnum atk, ElementEnum def)
        {
            var fit = Global.Table.ElementTable.Get(atk).Fit;
            float value = fit.GetValueOrDefault(def, 1);
            return value;
        }

        public static async UniTask<string> CalElementFitText(HuluData user, ElementEnum atk, ElementEnum def)
        {
            float value = await CalDamageElementFit(user, atk, def);

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

        public static async UniTask<(HuluData, HuluData)> WhoFirst(IBattleTrainer rT, IBattleTrainer lt, HuluData r,
            HuluData l,
            ActiveSkillData rs, ActiveSkillData ls,
            BattleEnvData envData)
        {
            Assert.IsTrue(rs.config.Type != ActiveSkillTypeEnum.指挥);
            Assert.IsTrue(ls.config.Type != ActiveSkillTypeEnum.指挥);

            if (r.ContainsBuff(BattleBuffEnum.轮滑技巧) && l.ContainsBuff(BattleBuffEnum.轮滑技巧))
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{r} 轮滑技巧"));
                await r.RemoveBuff(BattleBuffEnum.轮滑技巧);
                Global.Event.Send(new BattleInfoRecordEvent($"{l} 轮滑技巧"));
                await l.RemoveBuff(BattleBuffEnum.轮滑技巧);

                if (UnityEngine.Random.value > 0.5f)
                {
                    return (l, r);
                }

                return (r, l);
            }

            if (r.ContainsBuff(BattleBuffEnum.轮滑技巧))
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{r} 轮滑技巧"));
                await r.RemoveBuff(BattleBuffEnum.轮滑技巧);
                return (r, l);
            }

            if (l.ContainsBuff(BattleBuffEnum.轮滑技巧))
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{l} 轮滑技巧"));
                await l.RemoveBuff(BattleBuffEnum.轮滑技巧);
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

            int rRuntimeSpeed = (int)UglyMath.PostprocessRunTimeSpeed(rT, envData);
            int lRuntimeSpeed = (int)UglyMath.PostprocessRunTimeSpeed(lt, envData);

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
        public static async UniTask<int> CalDamage(HuluData atk, HuluData def, ActiveSkillEnum skill,
            BattleEnvData envData, GameData gameData)
        {
            ActiveSkillConfig config = Global.Table.ActiveSkillTable.Get(skill);
            Assert.IsTrue(config.DamagePoint != 0);

            Assert.IsTrue(config.Type == ActiveSkillTypeEnum.伤害技能);
            int damagePoint = UglyMath.PostprocessDamagePoint(config, envData, gameData);
            int atkPoint = await UglyMath.PostprocessAtkPoint(atk, config, envData);
            Debug.Log(
                $"攻击力{atkPoint},伤害{damagePoint},防御力{def.currentDef}" +
                $" 本系威力加成{CalSelfElementFit(atk.config, config)} \n" +
                $"属性克制{CalDamageElementFit(atk, config.Element, def.config.Elements)}\n");
            float baseValue = damagePoint * atkPoint / (float)def.currentDef
                              *
                              CalSelfElementFit(atk.config, config) // 本系威力加成
                              *
                              await CalDamageElementFit(atk, config.Element, def.config.Elements // 属性克制
                              );
            Debug.Log($"处理前的基础伤害{baseValue} ");
            baseValue = await UglyMath.PostprocessBattleBaseValue(baseValue, atk, def, config);

            int adap = GameMath.CalRunTimeAdap(def, envData);
            Debug.Log($"处理后的基础伤害{baseValue} 适应度{adap}");
            float finalValue = baseValue * (1 - Mathf.Clamp(adap, 0, 100) / 100f);

            finalValue = UglyMath.PostprocessBattleFinalValue(finalValue, atk, def, config);

            return (int)finalValue;
        }

        public static int CalRunTimeAdap(HuluData def, BattleEnvData envData)
        {
            int baseAdap = def.currentAdap;
            if (def.ContainsBuff(BattleBuffEnum.加十点适应力))
            {
                baseAdap += 10 * def.BuffCount(BattleBuffEnum.加十点适应力);
            }

            return baseAdap;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CalHit(HuluData atk, HuluData def, ActiveSkillEnum dataID,
            BattleEnvData envData)
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
                    Global.LogInfo($"移除所有{buffEnum}");
                    buffList.RemoveAll(x => x == buffEnum);
                    continue;
                }

                for (int i = 0; i < removeCnt; i++)
                {
                    Global.LogInfo($"移除{buffEnum}");
                    buffList.Remove(buffEnum);
                }
            }

            HashSetPool<BattleBuffEnum>.Release(contains);
        }

        public static async UniTask<int> CalAtkTimes(HuluData user, ActiveSkillConfig skillCfg)
        {
            int times = 0;
            if (user.ContainsBuff(BattleBuffEnum.连续技能必定打最多次))
            {
                times = skillCfg.MulAttackTimes[1];
                await user.RemoveBuff(BattleBuffEnum.连续技能必定打最多次);
            }
            else
            {
                times = UnityEngine.Random.Range(skillCfg.MulAttackTimes[0],
                    skillCfg.MulAttackTimes[1]);
            }

            return times;
        }

        public static async UniTask<float> CalDefDiscardCardRateWhenHitted(IBattleTrainer atkTrainer, IBattleTrainer defTrainer,
            ActiveSkillConfig skill)
        {
            float discardRate = skill.DefDiscardCardRateWhenHitted;
            if (atkTrainer.currentBattleData.ContainsBuff(BattleBuffEnum.下一次技能让对方受伤时弃牌概率变成1))
            {
                discardRate = 1;
                await atkTrainer.currentBattleData.RemoveBuff(BattleBuffEnum.下一次技能让对方受伤时弃牌概率变成1);
            }

            return discardRate;
        }

        public static async UniTask<IBattleOperation> ProcessOperationBeforeRounding(IBattleTrainer trainer,
            IBattleOperation operation)
        {
            if (operation is EndRoundOperation && trainer.ContainsBuff(BattleBuffEnum.回合结束后额外获得一个回合))
            {
                Debug.Log("回合结束后额外获得一个回合");
                await trainer.RemoveBuff(BattleBuffEnum.回合结束后额外获得一个回合);
                Global.Table.BattleBuffTable.Get(BattleBuffEnum.回合结束后额外获得一个回合);
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                return null;
            }

            if (trainer.ContainsBuff(BattleBuffEnum.结束回合))
            {
                await trainer.RemoveBuff(BattleBuffEnum.结束回合);
                return new EndRoundOperation();
            }

            return operation;
        }

        public static async UniTask ProcessTrainerAfterUseCardFromHandZone(IBattleTrainer userTrainer)
        {
            if (userTrainer.ContainsBuff(BattleBuffEnum.出牌时有40概率受到50点伤害))
            {
                var buffConfig = Global.Table.BattleBuffTable.Get(BattleBuffEnum.出牌时有40概率受到50点伤害);
                if (Random.value < buffConfig.TriggerRate)
                {
                    await userTrainer.currentBattleData.DecreaseHealth(buffConfig.DamageForCurrentPokemon, null);
                }
            }
        }

        public static async UniTask ProcessPokemonBeforeRounding(IBattleTrainer trainer)
        {
            if (trainer.ContainsBuff(BattleBuffEnum.没有手牌时当前宝可梦生命值归0) && trainer.handZone.Count <= 0)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{trainer}没有手牌!当前宝可梦生命值归0"));
                await trainer.currentBattleData.DecreaseHealth(trainer.currentBattleData.currentHp, null);
            }
        }

        public static List<HuluData> RandomGeneratedFirstPokemon(int cnt)
        {
            Assert.IsTrue(Global.Table.HuluTable.DataList.Count >= cnt);
            List<HuluData> result = new List<HuluData>(cnt);

            float rate = (float)cnt / Global.Table.HuluTable.DataList.Count;
            int current = 0;
            while (current < cnt)
            {
                foreach (var config in Global.Table.HuluTable.DataList)
                {
                    if (current >= cnt) break;
                    if (Random.value < rate)
                    {
                        result.Add(RandomInitializePokemonData(config.Id));
                        current++;
                    }
                }
            }

            return result;
        }

        private static HuluData RandomInitializePokemonData(HuluEnum id)
        {
            // TODO 生成初始宝可梦
            HuluData data = new HuluData(id);
            data.RollAbility();
            int skillCnt = Random.Range(6, 9);
            data.RollTargetSkills(skillCnt);
            int sum = data.hp + data.atk + data.def + data.speed + data.adap;
            if (sum <= 500) data.cost = Random.Range(120, 151);
            else if (sum > 500 && sum <= 600) data.cost = Random.Range(260, 301);
            else if (sum > 600 && sum <= 700) data.cost = Random.Range(380, 441);
            else if (sum > 700 && sum <= 800) data.cost = Random.Range(510, 571);
            else data.cost = Random.Range(650, 721);
            return data;
        }

        public static BattleEnvData RandomBattleEnvData()
        {
            List<BattleEnvironmentConfig> targets = Global.Table.BattleEnvironmentTable.DataList.FindAll(c =>
            {
                return !string.IsNullOrEmpty(c.BackgroundPath) && !string.IsNullOrWhiteSpace(c.BackgroundPath);
            });

            var id = targets[Random.Range(0, targets.Count)].Id;

            return new BattleEnvData() { id = id };
        }

        public static void RollBattleData(out TrainerData playerTrainerData, out TrainerData aiTrainerData,
            out BattleEnvData env)
        {
            HuluEnum[] huluValues = (HuluEnum[])Enum.GetValues(typeof(HuluEnum));
            huluValues.Shuffle();

            playerTrainerData = new TrainerData();
            playerTrainerData.RollTrainerSkill9();
            for (int i = 0; i < 3; i++)
            {
                var item = new HuluData();
                item.id = huluValues[i];
                item.elementEnum = item.config.Elements;
                item.Roll9Skills();
                item.RollAbility();
                playerTrainerData.datas.Add(item);
            }


            aiTrainerData = new TrainerData();
            aiTrainerData.RollTrainerSkill9();
            for (int i = 3; i < 6; i++)
            {
                var item = new HuluData();
                item.id = huluValues[i];
                item.elementEnum = item.config.Elements;
                item.Roll9Skills();
                item.RollAbility();
                aiTrainerData.datas.Add(item);
            }

            env = GameMath.RandomBattleEnvData();
        }

        public static ActiveSkillEnum RandomTrainerSkill()
        {
            var targets = Global.Table.ActiveSkillTable.DataList.FindAll((c) => (c.Type & ActiveSkillTypeEnum.指挥) != 0);

            targets.Shuffle();
            return targets[UnityEngine.Random.Range(0, targets.Count)].Id;
        }
    }
}