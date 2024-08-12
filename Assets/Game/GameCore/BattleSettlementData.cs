﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class BattleSettlementData
    {
        public SerializableDictionary<HuluData, int> localPlayerPokemonDamageCount;
        public SerializableDictionary<HuluData, int> remotePlayerPokemonDamageCount;

        public SerializableDictionary<HuluData, int> localPlayerPokemonDefeatCount;
        public SerializableDictionary<HuluData, int> remotePlayerPokemonDefeatCount;


        [HideInInspector] public TrainerData localPlayerTrainerData;

        [HideInInspector] public TrainerData remotePlayerTrainerData;

        [HideInInspector] public TrainerData winner;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool LocalPlayerWin()
        {
            return winner == localPlayerTrainerData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool LocalPlayerLose()
        {
            return winner == remotePlayerTrainerData;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RemotePlayerLose()
        {
            return localPlayerTrainerData == winner;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool RemotePlayerWin()
        {
            return remotePlayerTrainerData == winner;
        }

        public BattleSettlementData()
        {
            localPlayerPokemonDamageCount = new SerializableDictionary<HuluData, int>();
            remotePlayerPokemonDamageCount = new SerializableDictionary<HuluData, int>();

            localPlayerPokemonDefeatCount = new SerializableDictionary<HuluData, int>();
            remotePlayerPokemonDefeatCount = new SerializableDictionary<HuluData, int>();
        }
        public BattleSettlementData(TrainerData localPlayerTrainerData, TrainerData remotePlayerTrainerData)
        {
            this.localPlayerTrainerData = localPlayerTrainerData;
            this.remotePlayerTrainerData = remotePlayerTrainerData;

            localPlayerPokemonDamageCount = new SerializableDictionary<HuluData, int>();
            remotePlayerPokemonDamageCount = new SerializableDictionary<HuluData, int>();

            localPlayerPokemonDefeatCount = new SerializableDictionary<HuluData, int>();
            remotePlayerPokemonDefeatCount = new SerializableDictionary<HuluData, int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddDefeatCount(TrainerData trainerData, HuluData attacker, int cnt = 1)
        {
            Assert.IsTrue(trainerData == localPlayerTrainerData || trainerData == remotePlayerTrainerData);
            if (trainerData == localPlayerTrainerData)
            {
                if (!localPlayerPokemonDefeatCount.TryAdd(attacker, cnt))
                {
                    localPlayerPokemonDefeatCount[attacker] += cnt;
                }
            }
            else if (trainerData == remotePlayerTrainerData)
            {
                if (!remotePlayerPokemonDefeatCount.TryAdd(attacker, cnt))
                {
                    remotePlayerPokemonDefeatCount[attacker] += cnt;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddDamageCount(TrainerData trainerData, HuluData attacker, int damage)
        {
            Assert.IsTrue(trainerData == localPlayerTrainerData || trainerData == remotePlayerTrainerData);
            if (trainerData == localPlayerTrainerData)
            {
                if (!localPlayerPokemonDamageCount.TryAdd(attacker, damage))
                {
                    localPlayerPokemonDamageCount[attacker] += damage;
                }
            }
            else if (trainerData == remotePlayerTrainerData)
            {
                if (!remotePlayerPokemonDamageCount.TryAdd(attacker, damage))
                {
                    remotePlayerPokemonDamageCount[attacker] += damage;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<HuluData, int> MVP()
        {
            if (winner == localPlayerTrainerData)
            {
                int maxValue = localPlayerPokemonDamageCount.Max(x => x.Value);
                return localPlayerPokemonDamageCount.FirstOrDefault(x => x.Value == maxValue);
                //return localPlayerPokemonDamageCount.Max();
            }

            if (winner == remotePlayerTrainerData)
            {
                return remotePlayerPokemonDamageCount.Max();
            }

            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public KeyValuePair<HuluData, int> SVP()
        {
            if (winner == localPlayerTrainerData)
            {
                int maxValue = localPlayerPokemonDamageCount.Max(x => x.Value);
                return localPlayerPokemonDamageCount.FirstOrDefault(x => x.Value == maxValue);
                //return localPlayerPokemonDamageCount.Max();
            }

            if (winner == remotePlayerTrainerData)
            {
                return remotePlayerPokemonDamageCount.Max();
            }

            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LocalDefeatCount()
        {
            int selfDefeatCount = 0;
            foreach (var kv in localPlayerPokemonDefeatCount)
            {
                selfDefeatCount += kv.Value;
            }

            return selfDefeatCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RemoteDefeatCount()
        {
            int enemyDefeatCount = 0;
            foreach (var kv in remotePlayerPokemonDefeatCount)
            {
                enemyDefeatCount += kv.Value;
            }

            return enemyDefeatCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LocalAdmirePoint()
        {
            int selfDefeatCount = LocalDefeatCount();
            int enemyDefeatCount = RemoteDefeatCount();

            if (LocalPlayerWin())
            {
                return selfDefeatCount * Consts.BattlePerDefeatedPoint -
                       enemyDefeatCount * Consts.BattleBeDefeatedDecreasePoint +
                       Consts.BattleWinnerBaseAdmirePoint;
            }

            if (LocalPlayerLose())
            {
                return selfDefeatCount * Consts.BattlePerDefeatedPoint -
                       enemyDefeatCount * Consts.BattleBeDefeatedDecreasePoint +
                       Consts.BattleLoserBaseAdmirePoint;
            }

            throw new NotImplementedException($"winner: {winner} 不合法");
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RemoteAdmirePoint()
        {
            int remoteDefeatCount = RemoteDefeatCount();
            int localDefeatCount = LocalDefeatCount();
            if (RemotePlayerWin())
            {
                return remoteDefeatCount * Consts.BattlePerDefeatedPoint -
                       localDefeatCount * Consts.BattleBeDefeatedDecreasePoint +
                       Consts.BattleWinnerBaseAdmirePoint;
            }

            if (RemotePlayerLose())
            {
                return remoteDefeatCount * Consts.BattlePerDefeatedPoint -
                       localDefeatCount * Consts.BattleBeDefeatedDecreasePoint +
                       Consts.BattleLoserBaseAdmirePoint;
            }

            throw new NotImplementedException($"winner: {winner} 不合法");
        }
    }
}