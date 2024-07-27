using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.GamePlay
{
    [Serializable]
    public class RebotTrainer : ITrainer
    {
        public bool canFight => trainerData.canFight;

        [field: SerializeField] public TrainerData trainerData { get; set; }
        public HuluData currentData { get; set; }

        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard;
        public event Func<List<ActiveSkillData>, UniTask> OnDiscardCard;


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