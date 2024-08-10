using System;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class GameData : IModel
    {
        public PlayerData playerData;
        // TODO 游戏流程的位置信息
        public Type gameOutsideStateType;
        public BattleSettlementData battleSettlementData;
    }
}