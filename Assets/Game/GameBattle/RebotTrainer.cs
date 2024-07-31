﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public HuluData currentBattleData { get; set; }
        public HashSet<ActiveSkillData> handZone { get; }

        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard;
        public event Func<ActiveSkillData, UniTask> OnUseHandCard;
        public event Func<List<ActiveSkillData>, UniTask> OnDestroyCard;
        public event Func<List<ActiveSkillData>, UniTask> OnDiscardCard;
        public event Func<List<ActiveSkillData>, UniTask> OnConsumedCard;
        public event Func<List<ActiveSkillData>, List<ActiveSkillData>, UniTask> OnDiscardToDraw;
        public event Func<UniTask> OnStartCalOperation;
        public event Func<UniTask> OnEndCalOperation;


        public UniTask ChangeCurrentHulu(HuluData robotPosCurrentData)
        {
            currentBattleData = robotPosCurrentData;
            Debug.LogWarning($"机器人切换宝可梦未实现");
            return UniTask.CompletedTask;
        }

        public UniTask DrawSkills(int cnt)
        {
            Debug.LogWarning($"机器人抽牌未实现");
            return UniTask.CompletedTask;
        }

        public UniTask UseCardFromHandZone(ActiveSkillData data)
        {
            Debug.LogWarning($"机器人使用技能未实现");
            return UniTask.CompletedTask;
        }

        public UniTask RandomDiscard(int i)
        {
            Debug.LogWarning($"机器人随机弃牌未实现");
            return UniTask.CompletedTask;
        }

        public int GetHandZoneCount()
        {
            throw new NotImplementedException();
        }

        public UniTask DiscardAllHandCards()
        {
            throw new NotImplementedException();
        }

        public UniTask Discard2DrawZone()
        {
            throw new NotImplementedException();
        }

        public UniTask DrawTarget(ActiveSkillTypeEnum type, int cnt)
        {
            throw new NotImplementedException();
            
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Init(TrainerData trainerData1)
        {
            trainerData = trainerData1;
        }
    }
}