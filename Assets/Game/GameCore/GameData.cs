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
        public bool allowCompeting = true;
        //游戏当前规则配置信息
        public GameRuleConfig ruleConfig;
        public List<GameRuleEnum> rulePool;
        public int championCount = 0;
        public bool haveWatchedChampion = false;
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
            gameData.rulePool = new List<GameRuleEnum>()
            {
                GameRuleEnum.所有伤害技能威力增加20,
                GameRuleEnum.每回合抽牌数量变为2张,
                GameRuleEnum.手牌上限减少到6张,
                GameRuleEnum.每局游戏上场的角色数量改为4,
            };
            gameData.rulePool.Shuffle();
            return gameData;
        }
    }
}