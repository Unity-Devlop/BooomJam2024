using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum OpponentId
    {
        ������,
        �����,
        ����,
        �зּ�,
        ˵������,
    }

    [CreateAssetMenu(fileName = "Opponent", menuName = "ScriptableObject/Opponent")]
    public class Opponent : ScriptableObject
    {
        public OpponentId id;
        public List<HuluEnum> hulus = new List<HuluEnum>();
    }
}
