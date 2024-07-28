using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game.GamePlay
{
    [GlobalConfig("Assets/Resource/Config/TrainerPresetTable")]
    // [CreateAssetMenu(fileName = "TrainerPresetTable", menuName = "Game/TrainerPresetTable")]
    public class TrainerPresetTable : GlobalConfig<TrainerPresetTable>
    {
        public List<TrainerData> datas = new List<TrainerData>();

        public void Add(TrainerData data)
        {
            datas.Add(data);
        }

        public TrainerData Get(int idx)
        {
            return datas[idx];
        }
    }
}