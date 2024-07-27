using System;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine.Pool;

namespace Game.GamePlay
{
    [Serializable]
    public class PlayerTrainer : ITrainer
    {
        public bool canFight => trainerData.canFight;
        [ReadOnly] public TrainerData trainerData { get; private set; }
        public HuluData currentData { get; private set; }


        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard;
        public event Func<List<ActiveSkillData>, UniTask> OnDiscardCard;


        // Draw zone, hand zone, discard zone
        public List<ActiveSkillData> drawZone;
        public List<ActiveSkillData> handZone;
        public List<ActiveSkillData> discardZone;


        public void ChangeHulu(HuluData data)
        {
            if (currentData != null)
            {
                List<ActiveSkillData> needDelete = ListPool<ActiveSkillData>.Get();

                //TODO 删牌
                ListPool<ActiveSkillData>.Release(needDelete);
            }

            currentData = data;
        }

        public void DrawSkills()
        {
            // 开始Roll牌 
            handZone = new List<ActiveSkillData>(8);
            for (int i = 0; i < 8; i++)
            {
                handZone.Add(new ActiveSkillData()
                {
                    id = ActiveSkillEnum.冲击
                });
            }

            OnDrawCard(handZone);
        }

        public void Init(TrainerData trainerData1)
        {
            trainerData = trainerData1;
        }
    }
}