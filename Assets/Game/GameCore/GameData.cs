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
        public int huluCapacity=4;
        //游戏当前规则配置信息
        public GameRuleConfig ruleConfig;
        [JsonIgnore]
        public BindData<GameData> bind { get; private set; }

        public GameData()
        {
            bind = new BindData<GameData>(this);
        }

        public static GameData CreateEmpty()
        {
            var gameData = new GameData();
            gameData.playerData = new PlayerData(true);
            GameRuleConfig gameRuleConfig = new GameRuleConfig();
            gameRuleConfig.ruleList = new HashSet<GameRuleEnum>();
            gameRuleConfig.prevCnt = 0;
            gameData.ruleConfig = gameRuleConfig;
            return gameData;
        }
    }
}