using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GamePlay
{
    [Serializable]
    public class RebotTrainer : ITrainer
    {
        [field: SerializeField] public TrainerData trainerData { get; set; }
        public event Action<List<ActiveSkillData>> OnDrawCard;

        public void ChangeHulu(HuluData robotPosCurrentData)
        {
            Debug.LogWarning($"机器人切换宝可梦未实现");
        }

        public void DrawSkills()
        {
            Debug.LogWarning($"机器人抽牌未实现");
        }
    }
}