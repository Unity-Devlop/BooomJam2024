using UnityEngine;

namespace Game
{
    public static class Consts
    {
        public const float PointerUpClickTime = .2f;

        public static readonly string LocalPlayerDataPath = $"{Application.persistentDataPath}/LocalPlayerData.json";
        public const int MaxHandCard = 8;
        public const string GameBattleData = "BattleData";
        public const string LocalPlayerTrainerData = "LocalPlayerTrainerData";
        public const string EnemyTrainerData = "EnemyTrainerData";
        public const string GamePlayOutsideStateType = "GamePlayOutsideStateType";
        public const string BattleSettlementData = "BattleSettlementData";
        public const int BattlePerDefeatedPoint = 100;
        public const int BattleBeDefeatedDecreasePoint = 50;
        public const int BattleWinnerBaseAdmirePoint = 200;
        public const int BattleLoserBaseAdmirePoint = 100;


        public static class Animation
        {
            public const string BattlePokemonAttackAnim = "Attack";
            public const string BattlePokemonIdleAnim = "Idle";
        }
    }
}