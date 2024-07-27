using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEngine;
using UnityToolkit;

namespace Game.GamePlay
{
    [GlobalConfig("Assets/Resource/Config/TrainerPresetTable.asset")]
    [CreateAssetMenu(fileName = "TrainerPresetTable", menuName = "Game/TrainerPresetTable")]
    public class TrainerPresetTable : GlobalConfig<TrainerPresetTable>
    {
        private List<TrainerPreset> _presets;

        public void Add(TrainerData data)
        {
            TrainerPreset preset = ScriptableObject.CreateInstance<TrainerPreset>();
            preset.data = data;
            _presets.Add(preset);
        }

        public TrainerData Get(int idx)
        {
            return _presets[idx].data;
        }
    }

    public class TrainerPreset : ScriptableObject
    {
        public TrainerData data;
    }
}