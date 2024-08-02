using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class BuffContainer
    {
        public IBattleTrainer trainer;

        [FormerlySerializedAs("buffEnums")] public List<BattleBuffEnum> buffList = new List<BattleBuffEnum>();
        // public List<BattleBuffEnum> lastRoundBuffEnums = new List<BattleBuffEnum>();
    }

    [Serializable]
    public class BattleEnvironmentData : IModel
    {
        public BattleEnvironmentEnum id;
        public BattleEnvironmentConfig config => Global.Table.BattleEnvironmentTable.Get(id);

        [ShowInInspector] private Dictionary<IBattleTrainer, BuffContainer> _containers;

        public BattleEnvironmentData()
        {
            _containers = new Dictionary<IBattleTrainer, BuffContainer>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BuffContainer GetBuff(IBattleTrainer trainer)
        {
            var container = _containers[trainer];
            return container;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveBuff(IBattleTrainer trainer, BattleBuffEnum buff)
        {
            var container = _containers[trainer];
            Assert.IsNotNull(container);
            container.buffList.Remove(buff);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask AddBuff(IBattleTrainer atkTrainer, BattleBuffEnum buff)
        {
            var container = _containers[atkTrainer];
            Assert.IsNotNull(container);
            container.buffList.Add(buff);
            // await Global.BattleUI.ShowBuff(atkTrainer, 起风);
        }

        public void AddTrainer(IBattleTrainer trainer)
        {
            Assert.IsFalse(_containers.ContainsKey(trainer));
            _containers.Add(trainer, new BuffContainer()
            {
                trainer = trainer
            });
        }

        public async UniTask RoundEnd()
        {
            foreach (var (trainer, buffContainer) in _containers)
            {
                GameMath.PrcessBuffWhenRoundEnd(buffContainer.buffList);
            }
        }
    }
}