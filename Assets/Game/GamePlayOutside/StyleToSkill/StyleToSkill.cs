using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName ="Style",menuName ="ScriptableObject/Style")]
    public class StyleToSkill : ScriptableObject
    {
        public string styleName;
        public List<ActiveSkillEnum> skills;
    }
}
