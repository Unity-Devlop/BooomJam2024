using System;
using cfg;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "StyleToSkillConfig", menuName = "Game/StyleToSkillConfig")]
    public class StyleToSkillConfig : ScriptableObject
    {
        public List<StyleToSkillData> dataList = new List<StyleToSkillData>();

        private void OnValidate()
        {
            foreach (var styleToSkillData in dataList)
            {
                if (styleToSkillData.skills == null)
                {
                    styleToSkillData.skills = new List<ActiveSkillEnum>(4);
                }

                styleToSkillData.skills.RemoveAll(id =>
                    (Global.Table.ActiveSkillTable.Get(id).Type & ActiveSkillTypeEnum.指挥) == 0);
            }
        }
    }

    [Serializable]
    public class StyleToSkillData
    {
        [HorizontalGroup("1")] public string styleName;
        public List<ActiveSkillEnum> skills;

        [Button, HorizontalGroup("1")]
        private void Roll()
        {
            skills = new List<ActiveSkillEnum>(4);
            for (int i = 0; i < 4; ++i)
            {
                skills.Add(GameMath.RandomTrainerSkill());
            }
        }
    }
}