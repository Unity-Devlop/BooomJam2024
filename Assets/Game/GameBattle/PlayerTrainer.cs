﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using cfg;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.GamePlay
{
    [Serializable]
    public class PlayerBattleTrainer : IBattleTrainer
    {
        public bool canFight => trainerData.canFight;
        [field: SerializeField] public TrainerData trainerData { get; private set; }
        [field: NonSerialized] public HuluData currentBattleData { get; private set; }

        public event Func<List<ActiveSkillData>, UniTask> OnDrawCard = delegate { return UniTask.CompletedTask; };
        public event Func<ActiveSkillData, UniTask> OnUseCardFromHand = delegate { return UniTask.CompletedTask; };
        public event Func<List<ActiveSkillData>, UniTask> OnDestroyCard = delegate { return UniTask.CompletedTask; };

        public event Func<List<ActiveSkillData>, IBattleTrainer, UniTask> OnDiscardCardFromHand = delegate
        {
            return UniTask.CompletedTask;
        };

        public event Func<List<ActiveSkillData>, UniTask> OnConsumedCard = delegate { return UniTask.CompletedTask; };
        public event Func<UniTask> OnStartCalOperation = delegate { return UniTask.CompletedTask; };
        public event Func<UniTask> OnEndCalOperation = delegate { return UniTask.CompletedTask; };

        public event Func<List<ActiveSkillData>, List<ActiveSkillData>, UniTask> OnDiscardToDraw = delegate
        {
            return UniTask.CompletedTask;
        };

        [SerializeField] private List<BattleBuffEnum> buffList = new List<BattleBuffEnum>();


        public List<ActiveSkillEnum> deck = new();

        // Draw zone, hand zone, discard zone
        [ShowInInspector, ReadOnly] public HashSet<ActiveSkillData> drawZone = new();

        [ShowInInspector, ReadOnly]
        public HashSet<ActiveSkillData> handZone { get; private set; } = new HashSet<ActiveSkillData>();

        [ShowInInspector, ReadOnly] public HashSet<ActiveSkillData> discardZone = new();
        [ShowInInspector, ReadOnly] public HashSet<ActiveSkillData> consumedZone = new(); // 墓地区域

        private IBattleOperation _operation;

        public void ClearOperation()
        {
            _operation = null;
        }

        public virtual async UniTask<IBattleOperation> CalOperation()
        {
            Debug.Log($"{this} 开始思考操作");
            Assert.IsNull(_operation);

            if (handZone.Count == 0)
            {
                return new EndRoundOperation();
            }

            // Debug.Log("开始计算操作");
            await OnStartCalOperation(); // 通知UI开始计算操作
            await UniTask.WaitUntil(() => _operation != null); // 等待一个操作
            // Debug.Log($"获得了操作{_operation}"); 
            await OnEndCalOperation(); // 通知UI停止计算操作
            // Debug.Log("停止计算操作");
            return _operation;
        }

        public void PushOperation(IBattleOperation operation)
        {
            Assert.IsNull(_operation);
            _operation = operation;
        }

        /// <summary>
        /// 使用手牌
        /// </summary>
        /// <param name="data"></param>
        public async UniTask UseCardFromHand(ActiveSkillData data)
        {
            Assert.IsNotNull(data);
            // Debug.Log(
            // $"handZone:{handZone.Contains(data)},drawZone:{drawZone.Contains(data)},discardZone:{discardZone.Contains(data)}");
            // Debug.Log($"消耗牌{data} HashCode: {data.GetHashCode()}");
            Assert.IsTrue(handZone.Contains(data));
            await OnUseCardFromHand(data);
            if ((data.config.Type2 & CardTypeEnum.消耗) != 0)
            {
                await ConsumeCardFromHand(data);
            }
            else
            {
                await DiscardCardFromHand(data);
            }
        }


        private async UniTask DiscardCardFromHand(ActiveSkillData data)
        {
            // Assert.IsTrue((data.config.Type2 & CardTypeEnum.Normal) != 0);
            Assert.IsTrue(handZone.Contains(data));
            Assert.IsFalse(discardZone.Contains(data));
            Assert.IsFalse(consumedZone.Contains(data));
            Assert.IsFalse(drawZone.Contains(data));
            handZone.Remove(data);
            List<ActiveSkillData> list = new List<ActiveSkillData>(1) { data };
            await OnDiscardCardFromHand(list, this);
            discardZone.Add(data);
        }


        public async UniTask ConsumeCardFromHand(ActiveSkillData data)
        {
            if ((data.config.Type2 & CardTypeEnum.消耗) == 0)
            {
                Debug.LogWarning($"尝试消耗非消耗牌{data}");
            }

            Assert.IsTrue(handZone.Contains(data));
            Assert.IsFalse(discardZone.Contains(data));
            Assert.IsFalse(drawZone.Contains(data));
            Assert.IsFalse(consumedZone.Contains(data));

            handZone.Remove(data);
            consumedZone.Add(data);
            List<ActiveSkillData> list = ListPool<ActiveSkillData>.Get();
            list.Add(data);
            await OnConsumedCard(list);
        }

        public async UniTask MoveDiscardCardToConsumeZone(ActiveSkillData data)
        {
            Assert.IsTrue(discardZone.Contains(data));
            Assert.IsFalse(consumedZone.Contains(data));
            discardZone.Remove(data);
            consumedZone.Add(data);
            Debug.Log($"移动弃牌区到消耗区{data}");
            await UniTask.CompletedTask;
        }

        public bool ContainsBuff(BattleBuffEnum buff)
        {
            return buffList.Contains(buff);
        }

        public int GetConsumeCardInHandCount(ActiveSkillTypeEnum target)
        {
            int cnt = 0;
            foreach (var activeSkillData in handZone)
            {
                if ((activeSkillData.config.Type & target) != 0)
                {
                    cnt++;
                }
            }

            return cnt;
        }

        public async UniTask RandomDiscardCardFromHand(int i)
        {
            Debug.Log($"随机弃牌{i}张");
            int cnt = Mathf.Clamp(handZone.Count, 0, i);
            for (int j = 0; j < cnt; j++)
            {
                var discard = handZone.RandomTakeWithoutRemove();
                await DiscardCardFromHand(discard);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }

        public async UniTask DiscardAllHandCards()
        {
            var copy = handZone.ToList();
            foreach (var activeSkillData in copy)
            {
                await DiscardCardFromHand(activeSkillData);
            }
        }

        public async UniTask SwitchPokemon(HuluData data)
        {
            Assert.IsNotNull(data);
            Debug.Log($"切换当前宝可梦{currentBattleData}->{data}");
            if (currentBattleData != null)
            {
                await DestroyCurrentPokemonSkillCard();
            }
            else
            {
                InitOwnerSkillToDrawZone();
            }

            Debug.Log($"{this}切换成功");
            currentBattleData = data;
            RecalculateDeck();
            ReFillDrawZoneWhenChangeHulu();
            await DrawSkills(4);
        }

        private void InitOwnerSkillToDrawZone()
        {
            foreach (var trainerSkill in trainerData.trainerSkills)
            {
                Assert.IsFalse(drawZone.Contains(trainerSkill));
                Assert.IsFalse(handZone.Contains(trainerSkill));
                Assert.IsFalse(discardZone.Contains(trainerSkill));
                drawZone.Add(trainerSkill);
            }
        }


        private async UniTask DestroyCurrentPokemonSkillCard()
        {
            List<ActiveSkillData> needDelete = ListPool<ActiveSkillData>.Get();

            //删牌 删除手牌 弃牌区 抽牌区

            foreach (var activeSkillData in currentBattleData.ownedSkills)
            {
                if (!consumedZone.Contains(activeSkillData))
                {
                    needDelete.Add(activeSkillData);
                }
            }


            foreach (var deleteTar in needDelete)
            {
                if (drawZone.Contains(deleteTar))
                {
                    drawZone.Remove(deleteTar);
                    Debug.Log($"删除抽牌区中的{deleteTar}");
                }

                if (handZone.Contains(deleteTar))
                {
                    handZone.Remove(deleteTar);
                    Debug.Log($"删除手牌中的{deleteTar}");
                }

                if (discardZone.Contains(deleteTar))
                {
                    discardZone.Remove(deleteTar);
                    Debug.Log($"删除弃牌区中的{deleteTar}");
                }
            }

            await OnDestroyCard(needDelete);

            ListPool<ActiveSkillData>.Release(needDelete);
        }

        private void ReFillDrawZoneWhenChangeHulu()
        {
            //此时手里是没抽新宝可梦的技能牌的
            foreach (var ownedSkill in currentBattleData.ownedSkills)
            {
                Assert.IsFalse(handZone.Contains(ownedSkill));
                Assert.IsFalse(discardZone.Contains(ownedSkill));
                Assert.IsFalse(drawZone.Contains(ownedSkill));
                drawZone.Add(ownedSkill);
            }

            Debug.Log($"为当前宝可梦{currentBattleData}填充抽牌区域");
        }

        public async UniTask Discard2DrawZone()
        {
            Debug.Log("清空弃牌区 弃牌区加入抽牌区");
            drawZone.AddRange(discardZone);
            await OnDiscardToDraw(discardZone.ToList(), drawZone.ToList());
            discardZone.Clear();
        }


        /// <summary>
        /// 重新计算卡组
        /// </summary>
        private void RecalculateDeck()
        {
            deck.Clear();
            foreach (var ownedSkill in currentBattleData.ownedSkills)
            {
                if (consumedZone.Contains(ownedSkill))
                {
                    Debug.Log($"墓地区域有{ownedSkill} 不再加入卡组");
                    continue;
                }

                deck.Add(ownedSkill.id);
            }

            foreach (var skill in trainerData.trainerSkills)
            {
                if (consumedZone.Contains(skill))
                {
                    Debug.Log($"墓地区域有{skill} 不再加入卡组");
                    continue;
                }

                deck.Add(skill.id);
            }

            // 
            // Debug.Log($"卡组数量:{deck.Count}");
        }

        public async UniTask DrawSkills(int cnt)
        {
            // Debug.Log($"{this} 抽牌 {cnt}");
            int cur = handZone.Count; // 手牌数量
            // Debug.Log($"当前手牌数量:{cur}");
            int need = Consts.MaxHandCard - cur; // 还可以抽的数量
            need = Mathf.Clamp(need, 0, cnt); // 限制抽牌数量
            need = Mathf.Clamp(need, 0, deck.Count);
            if (need == 0)
            {
                // Debug.Log("手牌已满");
                return;
            }

            // Debug.Log($"可以抽牌:{need}张");
            HashSet<ActiveSkillData> drawList = HashSetPool<ActiveSkillData>.Get();
            // 从抽牌区抽牌
            for (int i = 0; i < need; i++)
            {
                if (drawZone.Count == 0)
                {
                    // Debug.Log("抽牌区没牌了");
                    // 弃牌区加入抽牌区
                    await Discard2DrawZone();
                }

                if (drawZone.Count == 0)
                {
                    Debug.LogError("抽牌区没牌了");
                    break;
                }

                // 随机抽一张 加入手牌
                var drawCard = drawZone.RandomTake();
                drawList.Add(drawCard);
                // Debug.Log($"Draw Card HashCode: {drawCard.GetHashCode()}, data: {drawCard}");
            }

            // Debug.Log($"抽到了{drawList.Count}张牌");
            handZone.AddRange(drawList);
            // Debug.Log($"当前手牌数量:{handZone.Count}");

            await OnDrawCard(drawList.ToList());
            HashSetPool<ActiveSkillData>.Release(drawList);
        }

        public async UniTask<int> DrawTarget(ActiveSkillTypeEnum type, int cnt)
        {
            HashSet<ActiveSkillData> drawList = HashSetPool<ActiveSkillData>.Get();

            foreach (var activeSkillData in drawZone)
            {
                if (activeSkillData.config.Type == type)
                {
                    drawList.Add(activeSkillData);
                    if (drawList.Count == cnt)
                    {
                        break;
                    }
                }
            }

            foreach (var target in drawList)
            {
                drawZone.Remove(target);
                handZone.Add(target);
            }

            await OnDrawCard(drawList.ToList());
            int result = drawList.Count;
            HashSetPool<ActiveSkillData>.Release(drawList);
            return result;
        }

        public async UniTask<int> DrawTarget(ActiveSkillEnum target, int cnt)
        {
            HashSet<ActiveSkillData> drawList = HashSetPool<ActiveSkillData>.Get();


            foreach (var activeSkillData in drawZone)
            {
                if (activeSkillData.id == target)
                {
                    drawList.Add(activeSkillData);
                    if (drawList.Count == cnt)
                    {
                        break;
                    }
                }
            }

            foreach (var target1 in drawList)
            {
                drawZone.Remove(target1);
                handZone.Add(target1);
            }

            await OnDrawCard(drawList.ToList());
            int result = drawList.Count;
            HashSetPool<ActiveSkillData>.Release(drawList);
            return result;
        }

        public int GetTargetCntInDeck(ActiveSkillTypeEnum targetType)
        {
            int cnt = 0;
            foreach (var skillEnum in deck)
            {
                var config = Global.Table.ActiveSkillTable.Get(skillEnum);
                if ((targetType & config.Type) != 0)
                {
                    cnt++;
                }
            }

            return cnt;
        }

        public UniTask AddCardToDeck(ActiveSkillData added)
        {
            Assert.IsNotNull(added);
            Assert.IsFalse(deck.Contains(added.id));
            Assert.IsFalse(drawZone.Contains(added));
            Assert.IsFalse(handZone.Contains(added));
            Assert.IsFalse(discardZone.Contains(added));

            deck.Add(added.id);
            drawZone.Add(added);
            // TODO 通知UI
            return UniTask.CompletedTask;
        }

        public async UniTask DrawHandFull()
        {
            await DrawSkills(Consts.MaxHandCard - handZone.Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Init(TrainerData trainerData1)
        {
            trainerData = trainerData1;
        }


        public async UniTask OnEnemyTrainerDiscardCard(List<ActiveSkillData> arg, IBattleTrainer trainer)
        {
            if (buffList.Contains(BattleBuffEnum.对手弃牌时自己摸等量手牌))
            {
                await DrawSkills(arg.Count);
            }

            if (buffList.Contains(BattleBuffEnum.回合内消耗对手弃置的牌))
            {
                foreach (var data in arg)
                {
                    if ((data.config.Type2 & CardTypeEnum.消耗) != 0)
                    {
                        await trainer.MoveDiscardCardToConsumeZone(data);
                    }
                }
            }

            await UniTask.CompletedTask;
        }

        public async UniTask RemoveBuff(BattleBuffEnum buff)
        {
            buffList.Remove(buff);
            await UniTask.CompletedTask;
        }

        public async UniTask BeforeRounding()
        {
            await UniTask.CompletedTask;
        }

        public async UniTask AddBuff(BattleBuffEnum buff)
        {
            var buffConfig = Global.Table.BattleBuffTable.Get(buff);

            Assert.IsTrue(buffConfig.IsTrainerBuff);
            
            Debug.Log($"{this} 获得buff {buff}");
            if (buff == BattleBuffEnum.抽两张牌)
            {
                await DrawSkills(2);
            }

            if (Global.Table.BattleBuffTable.Get(buff).NotSave)
                return;
         
            if (buffList.Contains(buff) && !buffConfig.CanStack)
                return;
            int cnt = buffList.Count((x) => x == buff);

            if (buffConfig.MaxStack > 0 && cnt >= buffConfig.MaxStack)
            {
                Debug.Log($"{this} buff {buff} 已经达到最大层数{buffConfig.MaxStack}");
                return;
            }

            buffList.Add(buff);
        }


        public void ExitBattle()
        {
            GameMath.ProcessBuffWhenRoundEnd(buffList);
        }
    }
}