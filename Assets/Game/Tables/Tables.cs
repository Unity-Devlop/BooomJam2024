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
            typeof(Tables).GetProperty(nameof(HuluTable)).SetValue(this, new HuluTable(await loader("hulutable")));
            typeof(Tables).GetProperty(nameof(ElementTable)).SetValue(this, new ElementTable(await loader("elementtable")));
            typeof(Tables).GetProperty(nameof(ActiveSkillTable)).SetValue(this, new ActiveSkillTable(await loader("activeskilltable")));
            typeof(Tables).GetProperty(nameof(PassiveSkillTable)).SetValue(this, new PassiveSkillTable(await loader("passiveskilltable")));
            typeof(Tables).GetProperty(nameof(BattleEnvironmentTable)).SetValue(this, new BattleEnvironmentTable(await loader("battleenvironmenttable")));
            typeof(Tables).GetProperty(nameof(BattleBuffTable)).SetValue(this, new BattleBuffTable(await loader("battlebufftable")));
            ResolveRef();
        }
    }
}