using Cysharp.Threading.Tasks;
using SimpleJSON;

namespace cfg
{
    public partial class Tables
    {
        public Tables()
        {
            
        }
        public async UniTask InitAsync(System.Func<string, UniTask<JSONNode>> loader)
        {
            // 反射强行写入
            HuluTable = new HuluTable(await loader("hulutable"));
            ElementTable = new ElementTable(await loader("elementtable"));
            ActiveSkillTable = new ActiveSkillTable(await loader("activeskilltable"));
            PassiveSkillTable = new PassiveSkillTable(await loader("passiveskilltable"));
            BattleEnvironmentTable = new BattleEnvironmentTable(await loader("battleenvironmenttable"));
            BattleBuffTable = new BattleBuffTable(await loader("battlebufftable"));
            ResolveRef();
        }
    }
}