
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
public sealed partial class ActiveSkillConfig : Luban.BeanBase
{
    public ActiveSkillConfig(JSONNode _buf) 
    {
        { if(!_buf["id"].IsNumber) { throw new SerializationException(); }  Id = (ActiveSkillEnum)_buf["id"].AsInt; }
        { if(!_buf["desc"].IsString) { throw new SerializationException(); }  Desc = _buf["desc"]; }
        { if(!_buf["element"].IsNumber) { throw new SerializationException(); }  Element = (ElementEnum)_buf["element"].AsInt; }
        { if(!_buf["type"].IsNumber) { throw new SerializationException(); }  Type = (ActiveSkillTypeEnum)_buf["type"].AsInt; }
    }

    public static ActiveSkillConfig DeserializeActiveSkillConfig(JSONNode _buf)
    {
        return new ActiveSkillConfig(_buf);
    }

    /// <summary>
    /// id
    /// </summary>
    public readonly ActiveSkillEnum Id;
    /// <summary>
    /// 描述
    /// </summary>
    public readonly string Desc;
    /// <summary>
    /// 属性
    /// </summary>
    public readonly ElementEnum Element;
    /// <summary>
    /// 类型（指挥/技能）
    /// </summary>
    public readonly ActiveSkillTypeEnum Type;
   
    public const int __ID__ = -652779027;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "desc:" + Desc + ","
        + "element:" + Element + ","
        + "type:" + Type + ","
        + "}";
    }
}

}
