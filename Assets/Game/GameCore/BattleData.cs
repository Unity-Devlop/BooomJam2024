using System;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
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
    public class BattleData : IModel
    {
        public BattleEnvironmentEnum id;
        public BattleEnvironmentConfig config => Global.Table.BattleEnvironmentTable.Get(id);

        // [ShowInInspector] private Dictionary<IBattleTrainer, BuffContainer> _containers;

        public BattleData()
        {
        }

        public async UniTask RoundEnd()
        {
            await UniTask.CompletedTask;
        }
        

        public void Clear()
        {
            
        }
    }
}