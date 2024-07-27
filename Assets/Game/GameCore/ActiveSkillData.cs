using System;
using cfg;

namespace Game
{
    
    [Serializable]
    public class ActiveSkillData
    {
        public ActiveSkillEnum id;
        public ActiveSkillConfig config => Global.Table.ActiveSkillTable.Get(id);
    }
}