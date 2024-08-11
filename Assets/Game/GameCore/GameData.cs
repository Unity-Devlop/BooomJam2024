using System;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class GameData : IModel
    {
        public PlayerData playerData;
        public int admireNum=1000;
        // TODO 游戏流程的位置信息
        public Type gameOutsideStateType;
        public BattleSettlementData battleSettlementData;
    }
}