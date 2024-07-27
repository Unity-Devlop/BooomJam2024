using System;
using System.Collections.Generic;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class BattleData
    {
        public BindData<BattleData> bind { get; private set; }

        public List<ActiveSkillData> allSkills;

        public BattleData()
        {
            bind = new BindData<BattleData>(this);
        }
    }
}