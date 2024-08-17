using System;
using System.Collections.Generic;
using System.Linq;
using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.GamePlay
{
    [Serializable]
    public class DummyRobot : PlayerBattleTrainer
    {
        // [SerializeField] 
        private float smartRate = 0.8f;
        [SerializeField] private float switchRate = 0.7f;

        public override async UniTask<IBattleOperation> CalOperation()
        {
            Global.LogInfo($"{this} 开始思考操作");
            float thinkingTime = UnityEngine.Random.Range(0.01f, 3f);
            await UniTask.Delay(TimeSpan.FromSeconds(thinkingTime - 0.01f));


            if (handZone.Count == 0)
            {
                if (UnityEngine.Random.value < switchRate)
                {
                    // 找一个不是自己的宝可梦 的 可以战斗的宝可梦 进行切换
                    List<int> targets = ListPool<int>.Get();
                    for (int i = 0; i < trainerData.datas.Count; i++)
                    {
                        var c = trainerData.datas[i];
                        if (c == currentBattleData)
                        {
                            continue;
                        }

                        if (!c.CanFight())
                        {
                            continue;
                        }

                        targets.Add(i);
                    }

                    if (targets.Count > 0)
                    {
                        int nextIdx = targets.Shuffle()[0];
                        return new ChangeHuluOperation()
                        {
                            next = nextIdx
                        };
                    }
                }

                // 不一定要结束回合 可以切换队友
                return new EndRoundOperation();
            }


            PlayerBattleTrainer playerBattleTrainer = GameBattleMgr.Singleton.playerBattleTrainer;
            HuluData enemyData = playerBattleTrainer.currentBattleData;

            if (UnityEngine.Random.value < smartRate)
            {
                // 如果对面没血了
                if (enemyData.currentHp / (float)enemyData.hp < 0.3f)
                {
                    // 放优先级高的技能打死
                    if (TryGetAnyPrioritySkill(enemyData, out ActiveSkillData prioritySkill))
                    {
                        return new ActiveSkillBattleOperation()
                        {
                            data = prioritySkill
                        };
                    }
                }

                // 如果自己没血了 尽可能找治疗技能 放 或者 放守护技能
                if (currentBattleData.currentHp / (float)currentBattleData.hp < 0.5f)
                {
                    // 看看有没有治疗的指挥牌
                    if (TryGetAnyHealCommand(out ActiveSkillData heal))
                    {
                        return new ActiveSkillBattleOperation()
                        {
                            data = heal
                        };
                    }
                }

                // 有counter技能就用counter技能
                if (TryGetAnyCounterSkill(enemyData, out ActiveSkillData data))
                {
                    return new ActiveSkillBattleOperation()
                    {
                        data = data
                    };
                }
            }

            // 没有counter技能就随机用一个
            var target = handZone.RandomTakeWithoutRemove();
            IBattleOperation operation = new ActiveSkillBattleOperation()
            {
                data = target
            };
            return operation;
        }

        private bool CanKill(HuluData enemy, ActiveSkillData skill)
        {
            throw new NotImplementedException();
        }

        private bool TryGetAnyPrioritySkill(HuluData enemyData, out ActiveSkillData prioritySkill)
        {
            prioritySkill = null;
            foreach (var skill in handZone)
            {
                var config = skill.config;
                if ((config.Type & ActiveSkillTypeEnum.伤害技能) == 0)
                {
                    continue;
                }

                if (config.Priority > 0)
                {
                    prioritySkill = skill;
                    return true;
                }
            }

            return false;
        }

        private bool TryGetAnyHealCommand(out ActiveSkillData heal)
        {
            heal = null;
            foreach (var skill in handZone)
            {
                var config = skill.config;
                if ((config.Type & ActiveSkillTypeEnum.指挥) == 0)
                {
                    continue;
                }

                if (config.IncreaseHealthPercentAfterUse > 0 || config.IncreaseHealthPointAfterUse > 0)
                {
                    heal = skill;
                    return true;
                }
            }

            return false;
        }

        private bool TryGetAnyCounterSkill(HuluData enemyData, out ActiveSkillData activeSkillData)
        {
            activeSkillData = null;
            foreach (var skill in handZone)
            {
                var config = skill.config;
                if ((config.Type & ActiveSkillTypeEnum.伤害技能) == 0)
                {
                    continue;
                }

                var rate = GameMath.CalElementFit(config.Element, enemyData.elementEnum);
                if (rate > 1)
                {
                    activeSkillData = skill;
                    return true;
                }
            }

            return false;
        }
    }
}