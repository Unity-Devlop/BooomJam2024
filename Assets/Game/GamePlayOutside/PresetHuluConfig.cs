using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "PresetHuluTable", menuName = "ScriptableObject/PresetHuluTable")]
    public class PresetHuluConfig : ScriptableObject
    {
        public List<HuluData> hulus = new List<HuluData>();
    }
}
