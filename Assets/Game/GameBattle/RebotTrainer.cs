using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.GamePlay
{
    [Serializable]
    public class RobotBattleTrainer : IBattleTrainer
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
            Debug.LogError("机器人未实现的方法");
            return 0;
        }

        public UniTask DiscardAllHandCards()
        {
            Debug.LogError("机器人未实现的方法");
            return UniTask.CompletedTask;
        }

        public UniTask Discard2DrawZone()
        {
            Debug.LogError("机器人未实现的方法");
            return UniTask.CompletedTask;
        }


        public UniTask<int> DrawTarget(ActiveSkillEnum target, int cnt)
        {
            Debug.LogError("机器人未实现的方法");
            return new UniTask<int>(cnt);
        }

        public int GetTargetCntInDeck(ActiveSkillTypeEnum targetType)
        {
            Debug.LogError("机器人未实现的方法");
            return 0;
        }

        public UniTask AddCardToDeck(ActiveSkillData added)
        {
            Debug.LogError("机器人未实现的方法");
            return UniTask.CompletedTask;
        }

        public UniTask DrawHandFull()
        {
            Debug.LogError("机器人未实现的方法");
            return UniTask.CompletedTask;
        }

        public UniTask<int> DrawTarget(ActiveSkillTypeEnum type, int cnt)
        {
            Debug.LogError("机器人未实现的方法");
            return new UniTask<int>(cnt);
        }

        public void PushOperation(IBattleOperation operation)
        {
        }

        public void ClearOperation()
        {
        }

        public UniTask<IBattleOperation> CalOperation()
        {
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