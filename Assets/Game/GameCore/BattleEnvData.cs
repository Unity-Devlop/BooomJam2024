using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class BattleEnvData : IModel
    {
        public BattleEnvironmentEnum id;
        public BattleEnvironmentConfig config => Global.Table.BattleEnvironmentTable.Get(id);

        public BindData<BattleEnvData, UniTask> bind { get; private set; }
        // [ShowInInspector] private Dictionary<IBattleTrainer, BuffContainer> _containers;

        public BattleEnvData()
        {
            bind = new BindData<BattleEnvData, UniTask>(this);
            // _containers = new Dictionary<IBattleTrainer, BuffContainer>();
        }

        public async UniTask RoundEnd()
        {
            await UniTask.CompletedTask;
        }


        public void Clear()
        {
        }

        public async UniTask Change(BattleEnvironmentEnum id)
        {
            this.id = id;
            await bind.Invoke();
        }
    }
}