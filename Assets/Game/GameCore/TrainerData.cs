using System;
using System.Collections.Generic;
using Game.GamePlay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class TrainerData
    {
        public List<HuluData> datas;
        

#if UNITY_EDITOR && ODIN_INSPECTOR

        [Button]
        private void AddToPreset()
        {
            TrainerPresetTable.Instance.Add(this);
        }
#endif
    }
}