using System;
using System.Collections.Generic;
using System.Linq;
using cfg;
using Cysharp.Threading.Tasks;
using Game.Game;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.GamePlay
{
    [Serializable]
    public class PlayerTrainer : ITrainer
    {
        public bool canFight => trainerData.canFight;
        [field: SerializeField] public TrainerData trainerData { get; private set; }
        public HuluData currentData { get; private set; }


        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard = delegate { return UniTask.CompletedTask; };
        public event Func<List<ActiveSkillData>, UniTask> OnDiscardCard = delegate { return UniTask.CompletedTask; };
        public event Func<List<ActiveSkillData>, UniTask> OnDiscrdToDraw = delegate { return UniTask.CompletedTask; };

        public List<ActiveSkillEnum> deck;

        // Draw zone, hand zone, discard zone
        [ShowInInspector, ReadOnly] public HashSet<ActiveSkillData> drawZone;
        [ShowInInspector, ReadOnly] public HashSet<ActiveSkillData> handZone;
        [ShowInInspector, ReadOnly] public HashSet<ActiveSkillData> discardZone;

        public PlayerTrainer()
        {
            drawZone = new HashSet<ActiveSkillData>();
            handZone = new HashSet<ActiveSkillData>();
            discardZone = new HashSet<ActiveSkillData>();
            deck = new List<ActiveSkillEnum>();
        }
        public async void OnConsume(OnActiveCardConsume obj)
        {
            Assert.IsNotNull(obj.data);
            // Debug.Log($"消耗牌{obj.data} HashCode: {obj.data.GetHashCode()}");
            if (handZone.Contains(obj.data))
            {
                handZone.Remove(obj.data);
                discardZone.Add(obj.data);
                List<ActiveSkillData> list = ListPool<ActiveSkillData>.Get();
                list.Add(obj.data);
                await OnDiscardCard(list);
                ListPool<ActiveSkillData>.Release(list);
            }
            else
            {
                Debug.LogError("消耗的牌不在手牌中");
            }
        }

        public async UniTask ChangeHulu(HuluData data)
        {
            if (currentData != null)
            {
                List<ActiveSkillData> needDelete = ListPool<ActiveSkillData>.Get();

                //TODO 删牌 删除手牌 弃牌区 抽牌区

                foreach (var deleteTar in needDelete)
                {
                    if (drawZone.Contains(deleteTar))
                    {
                        drawZone.Remove(deleteTar);
                    }

                    if (handZone.Contains(deleteTar))
                    {
                        handZone.Remove(deleteTar);
                    }

                    if (discardZone.Contains(deleteTar))
                    {
                        discardZone.Remove(deleteTar);
                    }
                }

                await OnDiscardCard(needDelete);

                ListPool<ActiveSkillData>.Release(needDelete);
            }
            else
            {
                Debug.Log("最初进入战斗 直接抽牌");
            }

            currentData = data;
            RecalculateDeck();
            ReFillDrawZone();
            await DrawSkills(4);
        }

        private void ReFillDrawZone()
        {
            //此时手里是没抽新宝可梦的技能牌的
            foreach (var ownedSkill in currentData.ownedSkills)
            {
                drawZone.Add(ownedSkill);
            }

            foreach (var trainerSkill in trainerData.trainerSkills)
            {
                drawZone.Add(trainerSkill);
            }

            Debug.Log($"重置抽牌区,当前抽牌区有:{drawZone.Count}张牌");
        }

        private async UniTask CleanDiscardZone()
        {
            Debug.Log("清空弃牌区 弃牌区加入抽牌区");
            drawZone.AddRange(discardZone);
            await OnDiscrdToDraw(discardZone.ToList());
            discardZone.Clear();
        }

        /// <summary>
        /// 重新计算卡组
        /// </summary>
        private void RecalculateDeck()
        {
            deck.Clear();
            foreach (var ownedSkill in currentData.ownedSkills)
            {
                deck.Add(ownedSkill.id);
            }

            foreach (var skill in trainerData.trainerSkills)
            {
                deck.Add(skill.id);
            }

            // 
            Debug.Log($"卡组数量:{deck.Count}");
        }

        public async UniTask DrawSkills(int cnt)
        {
            Debug.Log($"{this} 抽牌 {cnt}");
            int cur = handZone.Count; // 手牌数量
            Debug.Log($"当前手牌数量:{cur}");
            int need = Consts.MaxHandCard - cur; // 还可以抽的数量
            need = Mathf.Clamp(need, 0, cnt); // 限制抽牌数量
            if (need == 0)
            {
                Debug.Log("手牌已满");
                return;
            }

            Debug.Log($"可以抽牌:{need}张");
            HashSet<ActiveSkillData> drawList = HashSetPool<ActiveSkillData>.Get();
            // 从抽牌区抽牌
            for (int i = 0; i < need; i++)
            {
                if (drawZone.Count == 0)
                {
                    Debug.Log("抽牌区没牌了");
                    // 弃牌区加入抽牌区
                    await CleanDiscardZone();
                }

                // 随机抽一张 加入手牌
                var drawCard = drawZone.RandomTake();
                drawList.Add(drawCard);
                Debug.Log($"Draw Card HashCode: {drawCard.GetHashCode()}, data: {drawCard}");
            }

            Debug.Log($"抽到了{drawList.Count}张牌");
            handZone.AddRange(drawList);

            await OnDrawCard(drawList.ToList());
            HashSetPool<ActiveSkillData>.Release(drawList);
        }

        public void Init(TrainerData trainerData1)
        {
            trainerData = trainerData1;
        }
    }
}