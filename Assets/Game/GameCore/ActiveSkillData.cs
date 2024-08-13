using System;
using cfg;
using Newtonsoft.Json;

namespace Game
{
    
    [Serializable]
    public class ActiveSkillData
    {
        public ActiveSkillEnum id;
        
        [JsonIgnore]
        public ActiveSkillConfig config => Global.Table.ActiveSkillTable.Get(id);

        public override string ToString()
        {
            return id.ToString();
        }
    }
}