using System;
using System.Collections.Generic;
using cfg;
using Game.GamePlay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class TrainerData
    {
        public List<ActiveSkillData> trainerSkills;
        public List<HuluData> datas;

        public bool canFight
        {
            get
            {
                int cnt = 0;
                foreach (var data in datas)
                {
                    if (data.HealthIsZero())
                    {
                        cnt++;
                    }
                }

                if (cnt > 0)
                {
                    return true;
                }

                return false;
            }
        }

#if UNITY_EDITOR && ODIN_INSPECTOR

        [Button]
        private void AddToPreset()
        {
            TrainerPresetTable.Instance.Add(this);
        }

        [Button]
        private void RollTrainerSkill9()
        {
            if (trainerSkills == null)
                trainerSkills = new List<ActiveSkillData>(9);
            trainerSkills.Clear();
            var targets = Global.Table.ActiveSkillTable.DataList.FindAll((c) => c.Type == ActiveSkillTypeEnum.指挥);

            targets.Shuffle();

            for (int i = 0; i < 9; i++)
            {
                trainerSkills.Add(new ActiveSkillData()
                {
                    id = targets[UnityEngine.Random.Range(0, targets.Count)].Id
                });
            }
        }

#endif
        public bool FindFirstCanFight(out HuluData data)
        {
            foreach (var huluData in datas)
            {
                if (!huluData.HealthIsZero())
                {
                    data = huluData;
                    return true;
                }
            }

            data = null;
            return false;
        }
    }
}