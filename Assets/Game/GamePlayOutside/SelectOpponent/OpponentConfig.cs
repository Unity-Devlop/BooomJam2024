using System;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "OpponentConfig", menuName = "ScriptableObject/OpponentConfig")]
    public class OpponentConfig : ScriptableObject
    {
        public List<TrainerData> opponents = new List<TrainerData>();
    }
}