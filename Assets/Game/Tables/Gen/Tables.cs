
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;

namespace cfg
{
public partial class Tables
{
    public HuluTable HuluTable {get; }
    public ElementFitTable ElementFitTable {get; }
    public ActiveSkillTable ActiveSkillTable {get; }
    public PassiveSkillTable PassiveSkillTable {get; }
    public BattleEnvironmentTable BattleEnvironmentTable {get; }
    public BattleBuffTable BattleBuffTable {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        HuluTable = new HuluTable(loader("hulutable"));
        ElementFitTable = new ElementFitTable(loader("elementfittable"));
        ActiveSkillTable = new ActiveSkillTable(loader("activeskilltable"));
        PassiveSkillTable = new PassiveSkillTable(loader("passiveskilltable"));
        BattleEnvironmentTable = new BattleEnvironmentTable(loader("battleenvironmenttable"));
        BattleBuffTable = new BattleBuffTable(loader("battlebufftable"));
        ResolveRef();
    }
    
    private void ResolveRef()
    {
        HuluTable.ResolveRef(this);
        ElementFitTable.ResolveRef(this);
        ActiveSkillTable.ResolveRef(this);
        PassiveSkillTable.ResolveRef(this);
        BattleEnvironmentTable.ResolveRef(this);
        BattleBuffTable.ResolveRef(this);
    }
}

}
