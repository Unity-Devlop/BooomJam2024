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
                int deadCnt = 0;
                foreach (var data in datas)
                {
                    if (!data.CanFight())
                    {
                        // Global.LogWarning($"{data}战斗不能");
                        deadCnt++;
                    }
                }

                if (deadCnt == datas.Count)
                {
                    return false;
                }

                return true;
            }
        }

        public TrainerData()
        {
            trainerSkills = new List<ActiveSkillData>();
            datas = new List<HuluData>();
        }

        public void RemoveTrainerSkill(ActiveSkillEnum id)
        {
            int index = -1;
            for(int i=0;i<trainerSkills.Count;++i)
            {
                if (trainerSkills[i].id==id)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0) trainerSkills.RemoveAt(index);
        }

        [Button, HorizontalGroup("TrainerDebug")]
        private void AddToPreset()
        {
            TrainerPresetTable.Instance.Add(this);
        }

        [Button, HorizontalGroup("TrainerDebug")]
        public void RollTrainerSkill9()
        {
            if (trainerSkills == null)
                trainerSkills = new List<ActiveSkillData>(9);
            trainerSkills.Clear();
            var targets = Global.Table.ActiveSkillTable.DataList.FindAll((c) => (c.Type & ActiveSkillTypeEnum.指挥) != 0);

            targets.Shuffle();

            for (int i = 0; i < 9; i++)
            {
                trainerSkills.Add(new ActiveSkillData()
                {
                    id = targets[UnityEngine.Random.Range(0, targets.Count)].Id
                });
            }
        }

#if UNITY_EDITOR

        [Button, HorizontalGroup("TrainerDebug")]
        public void All9One(ActiveSkillEnum @enum)
        {
            trainerSkills.Clear();
            for (int i = 0; i < 9; i++)
            {
                trainerSkills.Add(new ActiveSkillData()
                {
                    id = @enum
                });
            }
        }
#endif

        public bool FindFirstCanFight(out HuluData data)
        {
            foreach (var huluData in datas)
            {
                if (huluData.CanFight())
                {
                    Global.LogInfo($"{huluData}可以战斗");
                    data = huluData;
                    return true;
                }
            }

            data = null;
            return false;
        }

        public HuluData RandomSelectExpect(HuluData expect)
        {
            List<HuluData> list = new List<HuluData>();
            foreach (var huluData in datas)
            {
                if (huluData != expect && huluData.CanFight())
                {
                    list.Add(huluData);
                }
            }

            if (list.Count == 0)
            {
                return null;
            }

            return list[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}