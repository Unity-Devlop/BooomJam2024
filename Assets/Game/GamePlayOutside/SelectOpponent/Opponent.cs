using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum OpponentId
    {
        外星人,
        五恒星,
        命运,
        中分鸡,
        说唱篮球,
    }

    [CreateAssetMenu(fileName = "Opponent", menuName = "ScriptableObject/Opponent")]
    public class Opponent : ScriptableObject
    {
        public OpponentId id;
        public List<HuluEnum> hulus = new List<HuluEnum>();
    }
}
