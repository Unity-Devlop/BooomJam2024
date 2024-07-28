using System;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.GamePlay
{
    [Serializable]
    public class RebotBattleTrainer : IBattleTrainer
    {
        public bool canFight => trainerData.canFight;

        [field: SerializeField] public TrainerData trainerData { get; set; }
        public HuluData currentData { get; set; }

        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard;
        public event Func<ActiveSkillData, UniTask> OnUseCard;
        public event Func<List<ActiveSkillData>, UniTask> OnDiscardCard;
        public event Func<UniTask> OnStartCalOperation;
        public event Func<UniTask> OnEndCalOperation;


        public UniTask ChangeHulu(HuluData robotPosCurrentData)
        {
            Debug.LogWarning($"机器人切换宝可梦未实现");
            return UniTask.CompletedTask;
        }

        public UniTask DrawSkills(int cnt)
        {
            Debug.LogWarning($"机器人抽牌未实现");
            return UniTask.CompletedTask;
        }

        public UniTask OnConsumeSkill(ActiveSkillData data)
        {
            Debug.LogWarning($"机器人使用技能未实现");
            return UniTask.CompletedTask;
        }

        public void PushOperation(IBattleOperation operation)
        {
        }

        public void ClearOperation()
        {
            
        }

        public UniTask<IBattleOperation> CalOperation()
        {
            ActiveSkillData data = new ActiveSkillData()
            {
                id = ActiveSkillEnum.冲浪
            };
            IBattleOperation operation = new ActiveSkillBattleOperation()
            {
                data = data
            };
            return new UniTask<IBattleOperation>(operation);
        }

        // public void OnConsume(OnActiveCardConsume onActiveCardConsume)
        // {
        //     Debug.LogWarning($"机器人消耗牌未实现");
        // }
    }
}