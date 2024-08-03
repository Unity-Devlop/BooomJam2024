﻿using UnityEngine;

namespace Game
{
    public static class Consts
    {
        public const float PointerUpClickTime = .2f;

        public static readonly string LocalPlayerDataPath = $"{Application.persistentDataPath}/LocalPlayerData.json";
        public const int MaxHandCard = 8;

        public static class Animation
        {
            public const string BattlePokemonAttackAnim = "Attack";
            public const string BattlePokemonIdleAnim = "Idle";
        }
    }
}