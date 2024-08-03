using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class SkillUIItem : MonoBehaviour
    {
        public Image skillImg;
        public Text skillName;
        public Text SkillDescription;
        public Button changeBtn;
        public ActiveSkillEnum id;
    }
}
