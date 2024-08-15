
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
    public HuluTable HuluTable { get; private set;}
    public ElementTable ElementTable {get; private set;}
    public ActiveSkillTable ActiveSkillTable {get; private set;}
    public PassiveSkillTable PassiveSkillTable {get; private set;}
    public BattleEnvironmentTable BattleEnvironmentTable {get; private set;}
    public BattleBuffTable BattleBuffTable {get; private set;}

    public Tables(System.Func<string, JSONNode> loader)
    {
        HuluTable = new HuluTable(loader("hulutable"));
        ElementTable = new ElementTable(loader("elementtable"));
        ActiveSkillTable = new ActiveSkillTable(loader("activeskilltable"));
        PassiveSkillTable = new PassiveSkillTable(loader("passiveskilltable"));
        BattleEnvironmentTable = new BattleEnvironmentTable(loader("battleenvironmenttable"));
        BattleBuffTable = new BattleBuffTable(loader("battlebufftable"));
        ResolveRef();
    }
    
    private void ResolveRef()
    {
        HuluTable.ResolveRef(this);
        ElementTable.ResolveRef(this);
        ActiveSkillTable.ResolveRef(this);
        PassiveSkillTable.ResolveRef(this);
        BattleEnvironmentTable.ResolveRef(this);
        BattleBuffTable.ResolveRef(this);
    }
}

}
