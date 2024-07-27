
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
public sealed partial class HuluConfig : Luban.BeanBase
{
    public HuluConfig(JSONNode _buf) 
    {
        { if(!_buf["id"].IsNumber) { throw new SerializationException(); }  Id = (HuluEnum)_buf["id"].AsInt; }
        { if(!_buf["name"].IsString) { throw new SerializationException(); }  Name = _buf["name"]; }
        { if(!_buf["passive_skill"].IsNumber) { throw new SerializationException(); }  PassiveSkill = (PassiveSkillEnum)_buf["passive_skill"].AsInt; }
        { if(!_buf["elements"].IsNumber) { throw new SerializationException(); }  Elements = (ElementEnum)_buf["elements"].AsInt; }
        { if(!_buf["base_hp"].IsNumber) { throw new SerializationException(); }  BaseHp = _buf["base_hp"]; }
        { if(!_buf["max_hp"].IsNumber) { throw new SerializationException(); }  MaxHp = _buf["max_hp"]; }
        { if(!_buf["base_atk"].IsNumber) { throw new SerializationException(); }  BaseAtk = _buf["base_atk"]; }
        { if(!_buf["max_atk"].IsNumber) { throw new SerializationException(); }  MaxAtk = _buf["max_atk"]; }
        { if(!_buf["base_def"].IsNumber) { throw new SerializationException(); }  BaseDef = _buf["base_def"]; }
        { if(!_buf["max_def"].IsNumber) { throw new SerializationException(); }  MaxDef = _buf["max_def"]; }
        { if(!_buf["base_speed"].IsNumber) { throw new SerializationException(); }  BaseSpeed = _buf["base_speed"]; }
        { if(!_buf["max_speed"].IsNumber) { throw new SerializationException(); }  MaxSpeed = _buf["max_speed"]; }
        { if(!_buf["base_adap"].IsNumber) { throw new SerializationException(); }  BaseAdap = _buf["base_adap"]; }
        { if(!_buf["max_adap"].IsNumber) { throw new SerializationException(); }  MaxAdap = _buf["max_adap"]; }
        { if(!_buf["skill_pool_id"].IsNumber) { throw new SerializationException(); }  SkillPoolId = _buf["skill_pool_id"]; }
    }

    public static HuluConfig DeserializeHuluConfig(JSONNode _buf)
    {
        return new HuluConfig(_buf);
    }

    /// <summary>
    /// ID
    /// </summary>
    public readonly HuluEnum Id;
    /// <summary>
    /// 名字(不填从id解析)
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// 被动技能ID
    /// </summary>
    public readonly PassiveSkillEnum PassiveSkill;
    /// <summary>
    /// 属性
    /// </summary>
    public readonly ElementEnum Elements;
    /// <summary>
    /// 基础生命值
    /// </summary>
    public readonly int BaseHp;
    /// <summary>
    /// 最大生命值
    /// </summary>
    public readonly int MaxHp;
    /// <summary>
    /// 基础攻击力
    /// </summary>
    public readonly int BaseAtk;
    /// <summary>
    /// 最大攻击力
    /// </summary>
    public readonly int MaxAtk;
    /// <summary>
    /// 基础防御力
    /// </summary>
    public readonly int BaseDef;
    /// <summary>
    /// 最大防御力
    /// </summary>
    public readonly int MaxDef;
    /// <summary>
    /// 基础速度
    /// </summary>
    public readonly int BaseSpeed;
    /// <summary>
    /// 最大速度
    /// </summary>
    public readonly int MaxSpeed;
    /// <summary>
    /// 基础适应力
    /// </summary>
    public readonly int BaseAdap;
    /// <summary>
    /// 最大适应力
    /// </summary>
    public readonly int MaxAdap;
    /// <summary>
    /// 技能池
    /// </summary>
    public readonly int SkillPoolId;
   
    public const int __ID__ = -255132072;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "name:" + Name + ","
        + "passiveSkill:" + PassiveSkill + ","
        + "elements:" + Elements + ","
        + "baseHp:" + BaseHp + ","
        + "maxHp:" + MaxHp + ","
        + "baseAtk:" + BaseAtk + ","
        + "maxAtk:" + MaxAtk + ","
        + "baseDef:" + BaseDef + ","
        + "maxDef:" + MaxDef + ","
        + "baseSpeed:" + BaseSpeed + ","
        + "maxSpeed:" + MaxSpeed + ","
        + "baseAdap:" + BaseAdap + ","
        + "maxAdap:" + MaxAdap + ","
        + "skillPoolId:" + SkillPoolId + ","
        + "}";
    }
}

}
