using System;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.GamePlay
{
    [Serializable]
    public class DummyRobot : PlayerBattleTrainer
    {
        public override async UniTask<IBattleOperation> CalOperation()
        {
            await UniTask.DelayFrame(1);
            List<ActiveSkillEnum> targets = ListPool<ActiveSkillEnum>.Get();
            foreach (var activeSkillConfig in Global.Table.ActiveSkillTable.DataList)
            {
                if (activeSkillConfig.Type == ActiveSkillTypeEnum.伤害技能)
                {
                    targets.Add(activeSkillConfig.Id);
                }
            }

            ActiveSkillEnum target = targets.RandomTake();
            ListPool<ActiveSkillEnum>.Release(targets);

            ActiveSkillData data = new ActiveSkillData()
            {
                id = target
            };
            IBattleOperation operation = new ActiveSkillBattleOperation()
            {
                data = data
            };
            return operation;
        }
    }
}