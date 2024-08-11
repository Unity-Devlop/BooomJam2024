using System;
using System.Collections.Generic;

namespace Game
{
    [Serializable]
    public class GameRuleConfig
    {
        public int prevCnt;// 变更规则前的规则数量
        public HashSet<GameRuleEnum> ruleList;// 当前规则
    }
}