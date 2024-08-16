using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game
{
    // [Serializable]
    public class BattleSettlementData
    {
        public Dictionary<int, int> localPlayerPokemonDamageCount;
        public Dictionary<int, int> remotePlayerPokemonDamageCount;

        public Dictionary<int, int> localPlayerPokemonDefeatCount;
        public Dictionary<int, int> remotePlayerPokemonDefeatCount;


        [JsonIgnore] public TrainerData localPlayerTrainerData;

        [JsonIgnore, HideInInspector] public TrainerData remotePlayerTrainerData;

        [JsonIgnore]
        public TrainerData winner
        {
            get
            {
                if (isLocalPlayerWin)
                {
                    return localPlayerTrainerData;
                }

                return remotePlayerTrainerData;
            }
        }

        public bool isLocalPlayerWin;

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

        public BattleSettlementData(TrainerData local, TrainerData remotePlayerTrainerData)
        {
            this.localPlayerTrainerData = local;
            this.remotePlayerTrainerData = remotePlayerTrainerData;

            localPlayerPokemonDamageCount = new Dictionary<int, int>();
            remotePlayerPokemonDamageCount = new Dictionary<int, int>();

            localPlayerPokemonDefeatCount = new Dictionary<int, int>();
            remotePlayerPokemonDefeatCount = new Dictionary<int, int>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddDefeatCount(TrainerData trainerData, HuluData attacker, int cnt = 1)
        {
            int idx = trainerData.datas.IndexOf(attacker);
            Assert.IsTrue(idx != -1);
            Assert.IsTrue(trainerData == localPlayerTrainerData || trainerData == remotePlayerTrainerData);
            if (trainerData == localPlayerTrainerData)
            {
                if (!localPlayerPokemonDefeatCount.TryAdd(idx, cnt))
                {
                    localPlayerPokemonDefeatCount[idx] += cnt;
                }
            }
            else if (trainerData == remotePlayerTrainerData)
            {
                if (!remotePlayerPokemonDefeatCount.TryAdd(idx, cnt))
                {
                    remotePlayerPokemonDefeatCount[idx] += cnt;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddDamageCount(TrainerData trainerData, HuluData attacker, int damage)
        {
            int idx = trainerData.datas.IndexOf(attacker);
            Assert.IsTrue(idx != -1);
            Assert.IsTrue(trainerData == localPlayerTrainerData || trainerData == remotePlayerTrainerData);
            if (trainerData == localPlayerTrainerData)
            {
                if (!localPlayerPokemonDamageCount.TryAdd(idx, damage))
                {
                    localPlayerPokemonDamageCount[idx] += damage;
                }
            }
            else if (trainerData == remotePlayerTrainerData)
            {
                if (!remotePlayerPokemonDamageCount.TryAdd(idx, damage))
                {
                    remotePlayerPokemonDamageCount[idx] += damage;
                }
            }
        }
        public struct BattleStatistics
        {
            public Guid guid;
            public int damage;
            
            public static implicit operator BattleStatistics((Guid, int) tuple)
            {
                return new BattleStatistics
                {
                    guid = tuple.Item1,
                    damage = tuple.Item2
                };
            }
        }

        /// <summary>
        /// 获取MVP
        /// </summary>
        /// <returns>原先宝可梦的Guid,伤害值</returns>
        /// <exception cref="NotImplementedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BattleStatistics MVP()
        {
            if (winner == localPlayerTrainerData)
            {
                if (localPlayerPokemonDamageCount.Count == 0)
                {
                    Global.LogWarning($"我一点伤害没打,就赢了?");
                    int idx = UnityEngine.Random.Range(0, localPlayerTrainerData.datas.Count);
                    return (localPlayerTrainerData.datas[idx].guid, 0);
                }

                int maxValue = localPlayerPokemonDamageCount.Max(x => x.Value);
                var kv = localPlayerPokemonDamageCount.FirstOrDefault(x => x.Value == maxValue);
                return (localPlayerTrainerData.datas[kv.Key].guid, kv.Value);
            }

            if (winner == remotePlayerTrainerData)
            {
                if (remotePlayerPokemonDamageCount.Count == 0)
                {
                    Global.LogWarning($"对面一点伤害没打,就赢了?");
                    int idx = UnityEngine.Random.Range(0, remotePlayerTrainerData.datas.Count);
                    return (remotePlayerTrainerData.datas[idx].guid, 0);
                }

                var max = remotePlayerPokemonDamageCount.Max(x => x.Value);
                var kv = remotePlayerPokemonDamageCount.FirstOrDefault(x => x.Value == max);
                return (remotePlayerTrainerData.datas[kv.Key].guid, kv.Value);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取SVP
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BattleStatistics SVP()
        {
            if (winner == localPlayerTrainerData) // 我赢了,对面是SVP
            {
                if (remotePlayerPokemonDamageCount.Count == 0)
                {
                    Global.LogWarning($"对面一点伤害没打,就输了?");
                    int idx = UnityEngine.Random.Range(0, remotePlayerTrainerData.datas.Count);
                    return (remotePlayerTrainerData.datas[idx].guid, 0);
                }

                int maxValue = remotePlayerPokemonDamageCount.Max(x => x.Value);
                var kv = remotePlayerPokemonDamageCount.FirstOrDefault(x => x.Value == maxValue);
                return (remotePlayerTrainerData.datas[kv.Key].guid, kv.Value);
            }

            if (winner == remotePlayerTrainerData) // 对面赢了 我是SVP
            {
                if (localPlayerPokemonDamageCount.Count == 0)
                {
                    Global.LogWarning($"我一点伤害没打,就输了?");
                    int idx = UnityEngine.Random.Range(0, localPlayerTrainerData.datas.Count);
                    return (localPlayerTrainerData.datas[idx].guid, 0);
                }

                int maxValue = localPlayerPokemonDamageCount.Max(x => x.Value);
                var kv = localPlayerPokemonDamageCount.FirstOrDefault(x => x.Value == maxValue);
                return (localPlayerTrainerData.datas[kv.Key].guid, kv.Value);
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