using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class BuffContainer
    {
        public IBattleTrainer trainer;
        public List<BuffEnum> buffEnums = new List<BuffEnum>();
        public List<BuffEnum> lastRoundBuffEnums = new List<BuffEnum>();
    }

    [Serializable]
    public class BattleEnvironmentData : IModel
    {
        public BattleEnvironmentEnum id;
        public BattleEnvironmentConfig config => Global.Table.BattleEnvironmentTable.Get(id);

        private Dictionary<IBattleTrainer, BuffContainer> _containers;

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
        public void RemoveBuff(IBattleTrainer trainer, BuffEnum buff)
        {
            var container = _containers[trainer];
            Assert.IsNotNull(container);
            container.buffEnums.Remove(buff);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask AddBuff(IBattleTrainer atkTrainer, BuffEnum buff)
        {
            var container = _containers[atkTrainer];
            Assert.IsNotNull(container);
            container.buffEnums.Add(buff);
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
    }
}