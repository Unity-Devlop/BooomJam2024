using System;
using cfg;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class BattleEnvironmentData : IModel
    {
        public BattleEnvironmentEnum id;
        public BattleEnvironmentConfig config => Global.Table.BattleEnvironmentTable.Get(id);
        
    }
}