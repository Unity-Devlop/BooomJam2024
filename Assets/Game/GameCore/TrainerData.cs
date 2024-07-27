using System;
using System.Collections.Generic;
using Game.GamePlay;
using Sirenix.OdinInspector;
using UnityEngine;

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
                    if (data.hp > 0)
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
#endif
    }
}