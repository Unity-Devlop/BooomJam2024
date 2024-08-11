using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class GameData : IModel
    {
        public PlayerData playerData;
        public int admireNum=1000;
        public Date date;
        //游戏流程的位置信息
        public Type gameOutsideStateType;
        public BattleSettlementData battleSettlementData;
        //游戏当前规则配置信息
        public GameRuleConfig ruleConfig;
        [JsonIgnore]
        public BindData<GameData> bind { get; private set; }

        public GameData()
        {
            bind = new BindData<GameData>(this);
        }
    }
}