
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
        { if(!_buf["element"].IsNumber) { throw new SerializationException(); }  Element = (ElementEnum)_buf["element"].AsInt; }
        { if(!_buf["desc"].IsString) { throw new SerializationException(); }  Desc = _buf["desc"]; }
        { if(!_buf["type"].IsNumber) { throw new SerializationException(); }  Type = (ActiveSkillTypeEnum)_buf["type"].AsInt; }
        { if(!_buf["damage_point"].IsNumber) { throw new SerializationException(); }  DamagePoint = _buf["damage_point"]; }
        { if(!_buf["priority"].IsNumber) { throw new SerializationException(); }  Priority = _buf["priority"]; }
        { if(!_buf["not_full_hp_buff_for_user_pokemon"].IsObject) { throw new SerializationException(); }  NotFullHpBuffForUserPokemon = AddBuffTuple.DeserializeAddBuffTuple(_buf["not_full_hp_buff_for_user_pokemon"]);  }
        { if(!_buf["full_hp_buff_for_user_pokemon"].IsObject) { throw new SerializationException(); }  FullHpBuffForUserPokemon = AddBuffTuple.DeserializeAddBuffTuple(_buf["full_hp_buff_for_user_pokemon"]);  }
        { if(!_buf["def_trainer_buff_after_use_count"].IsNumber) { throw new SerializationException(); }  DefTrainerBuffAfterUseCount = _buf["def_trainer_buff_after_use_count"]; }
        { if(!_buf["def_trainer_buff_after_use"].IsNumber) { throw new SerializationException(); }  DefTrainerBuffAfterUse = (BattleBuffEnum)_buf["def_trainer_buff_after_use"].AsInt; }
        { if(!_buf["self_trainer_buff_after_use_count"].IsNumber) { throw new SerializationException(); }  SelfTrainerBuffAfterUseCount = _buf["self_trainer_buff_after_use_count"]; }
        { if(!_buf["self_trainer_buff_after_use"].IsNumber) { throw new SerializationException(); }  SelfTrainerBuffAfterUse = (BattleBuffEnum)_buf["self_trainer_buff_after_use"].AsInt; }
        { if(!_buf["self_battle_buff_count_after_use"].IsNumber) { throw new SerializationException(); }  SelfBattleBuffCountAfterUse = _buf["self_battle_buff_count_after_use"]; }
        { if(!_buf["self_battle_buff_after_use"].IsNumber) { throw new SerializationException(); }  SelfBattleBuffAfterUse = (BattleBuffEnum)_buf["self_battle_buff_after_use"].AsInt; }
        { if(!_buf["hit_rate"].IsNumber) { throw new SerializationException(); }  HitRate = _buf["hit_rate"]; }
        { var __json0 = _buf["mul_attack_times"]; if(!__json0.IsArray) { throw new SerializationException(); } int _n0 = __json0.Count; MulAttackTimes = new int[_n0]; int __index0=0; foreach(JSONNode __e0 in __json0.Children) { int __v0;  { if(!__e0.IsNumber) { throw new SerializationException(); }  __v0 = __e0; }  MulAttackTimes[__index0++] = __v0; }   }
        { if(!_buf["change_battle_env_after_use"].IsNumber) { throw new SerializationException(); }  ChangeBattleEnvAfterUse = (BattleEnvironmentEnum)_buf["change_battle_env_after_use"].AsInt; }
        { if(!_buf["using_def_to_cal_damage"].IsBoolean) { throw new SerializationException(); }  UsingDefToCalDamage = _buf["using_def_to_cal_damage"]; }
        { if(!_buf["darw_target_card_config_after_use"].IsObject) { throw new SerializationException(); }  DarwTargetCardConfigAfterUse = DrawTargetCardTuple.DeserializeDrawTargetCardTuple(_buf["darw_target_card_config_after_use"]);  }
        { if(!_buf["darw_leader_card_count_after_use"].IsNumber) { throw new SerializationException(); }  DarwLeaderCardCountAfterUse = _buf["darw_leader_card_count_after_use"]; }
        { if(!_buf["darw_card_count_after_use"].IsNumber) { throw new SerializationException(); }  DarwCardCountAfterUse = _buf["darw_card_count_after_use"]; }
        { if(!_buf["effect_hit_rate"].IsNumber) { throw new SerializationException(); }  EffectHitRate = _buf["effect_hit_rate"]; }
        { if(!_buf["increase_self_def_point_after_use"].IsNumber) { throw new SerializationException(); }  IncreaseSelfDefPointAfterUse = _buf["increase_self_def_point_after_use"]; }
        { if(!_buf["type2"].IsNumber) { throw new SerializationException(); }  Type2 = (CardTypeEnum)_buf["type2"].AsInt; }
        { if(!_buf["increase_health_point_after_use"].IsNumber) { throw new SerializationException(); }  IncreaseHealthPointAfterUse = _buf["increase_health_point_after_use"]; }
        { if(!_buf["increase_health_percent_after_use"].IsNumber) { throw new SerializationException(); }  IncreaseHealthPercentAfterUse = _buf["increase_health_percent_after_use"]; }
        { if(!_buf["increase_self_speed_point_after_use"].IsNumber) { throw new SerializationException(); }  IncreaseSelfSpeedPointAfterUse = _buf["increase_self_speed_point_after_use"]; }
        { if(!_buf["def_discard_card_rate_when_hitted"].IsNumber) { throw new SerializationException(); }  DefDiscardCardRateWhenHitted = _buf["def_discard_card_rate_when_hitted"]; }
        { if(!_buf["def_discard_count_when_hitted"].IsNumber) { throw new SerializationException(); }  DefDiscardCountWhenHitted = _buf["def_discard_count_when_hitted"]; }
        { if(!_buf["def_discard_count_anyway"].IsNumber) { throw new SerializationException(); }  DefDiscardCountAnyway = _buf["def_discard_count_anyway"]; }
        { if(!_buf["user_discard_count_anyway"].IsNumber) { throw new SerializationException(); }  UserDiscardCountAnyway = _buf["user_discard_count_anyway"]; }
        { if(!_buf["full_hp_increase_base_value_rate"].IsNumber) { throw new SerializationException(); }  FullHpIncreaseBaseValueRate = _buf["full_hp_increase_base_value_rate"]; }
        { if(!_buf["percentage_damage_by_self"].IsNumber) { throw new SerializationException(); }  PercentageDamageBySelf = _buf["percentage_damage_by_self"]; }
        { if(!_buf["change_element_after_use"].IsNumber) { throw new SerializationException(); }  ChangeElementAfterUse = (ElementEnum)_buf["change_element_after_use"].AsInt; }
        { if(!_buf["increase_damage_point_when_grass_env"].IsNumber) { throw new SerializationException(); }  IncreaseDamagePointWhenGrassEnv = _buf["increase_damage_point_when_grass_env"]; }
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
    /// 属性
    /// </summary>
    public readonly ElementEnum Element;
    /// <summary>
    /// 描述
    /// </summary>
    public readonly string Desc;
    /// <summary>
    /// 类型（指挥/技能）
    /// </summary>
    public readonly ActiveSkillTypeEnum Type;
    /// <summary>
    /// 伤害值
    /// </summary>
    public readonly int DamagePoint;
    /// <summary>
    /// 优先级
    /// </summary>
    public readonly int Priority;
    /// <summary>
    /// 数量
    /// </summary>
    public readonly AddBuffTuple NotFullHpBuffForUserPokemon;
    /// <summary>
    /// 数量
    /// </summary>
    public readonly AddBuffTuple FullHpBuffForUserPokemon;
    /// <summary>
    /// 数量
    /// </summary>
    public readonly int DefTrainerBuffAfterUseCount;
    /// <summary>
    /// 给对面训练家的buff
    /// </summary>
    public readonly BattleBuffEnum DefTrainerBuffAfterUse;
    /// <summary>
    /// 数量
    /// </summary>
    public readonly int SelfTrainerBuffAfterUseCount;
    /// <summary>
    /// 给自己训练家上的Buff
    /// </summary>
    public readonly BattleBuffEnum SelfTrainerBuffAfterUse;
    /// <summary>
    /// 数量
    /// </summary>
    public readonly int SelfBattleBuffCountAfterUse;
    /// <summary>
    /// 使用后给自己增加的Buff
    /// </summary>
    public readonly BattleBuffEnum SelfBattleBuffAfterUse;
    /// <summary>
    /// 命中率
    /// </summary>
    public readonly float HitRate;
    /// <summary>
    /// 多次攻击次数
    /// </summary>
    public readonly int[] MulAttackTimes;
    /// <summary>
    /// 使用后变更的场地
    /// </summary>
    public readonly BattleEnvironmentEnum ChangeBattleEnvAfterUse;
    /// <summary>
    /// 是否使用防御力代替攻击力计算伤害
    /// </summary>
    public readonly bool UsingDefToCalDamage;
    /// <summary>
    /// 目标牌
    /// </summary>
    public readonly DrawTargetCardTuple DarwTargetCardConfigAfterUse;
    /// <summary>
    /// 使用后抽指挥牌的数量
    /// </summary>
    public readonly int DarwLeaderCardCountAfterUse;
    /// <summary>
    /// 使用后抽牌数量
    /// </summary>
    public readonly int DarwCardCountAfterUse;
    /// <summary>
    /// 会心率
    /// </summary>
    public readonly float EffectHitRate;
    /// <summary>
    /// 使用后增加的防御点数
    /// </summary>
    public readonly int IncreaseSelfDefPointAfterUse;
    /// <summary>
    /// 牌的使用类型
    /// </summary>
    public readonly CardTypeEnum Type2;
    /// <summary>
    /// 使用后恢复生命的点数
    /// </summary>
    public readonly int IncreaseHealthPointAfterUse;
    /// <summary>
    /// 使用后恢复生命的百分比
    /// </summary>
    public readonly float IncreaseHealthPercentAfterUse;
    /// <summary>
    /// 使用后增加的速度值
    /// </summary>
    public readonly int IncreaseSelfSpeedPointAfterUse;
    /// <summary>
    /// 被命中后弃牌率
    /// </summary>
    public readonly float DefDiscardCardRateWhenHitted;
    /// <summary>
    /// 被命中后弃牌数量
    /// </summary>
    public readonly int DefDiscardCountWhenHitted;
    /// <summary>
    /// 防御方弃牌数量
    /// </summary>
    public readonly int DefDiscardCountAnyway;
    /// <summary>
    /// 使用方弃牌数量
    /// </summary>
    public readonly int UserDiscardCountAnyway;
    /// <summary>
    /// 满血时baseValue加成
    /// </summary>
    public readonly float FullHpIncreaseBaseValueRate;
    /// <summary>
    /// 受到自己造成伤害的百分比
    /// </summary>
    public readonly float PercentageDamageBySelf;
    /// <summary>
    /// 使用后修改自己的属性
    /// </summary>
    public readonly ElementEnum ChangeElementAfterUse;
    /// <summary>
    /// 在草属性场地的威力加成
    /// </summary>
    public readonly int IncreaseDamagePointWhenGrassEnv;
   
    public const int __ID__ = -652779027;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        DarwTargetCardConfigAfterUse?.ResolveRef(tables);
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "element:" + Element + ","
        + "desc:" + Desc + ","
        + "type:" + Type + ","
        + "damagePoint:" + DamagePoint + ","
        + "priority:" + Priority + ","
        + "notFullHpBuffForUserPokemon:" + NotFullHpBuffForUserPokemon + ","
        + "fullHpBuffForUserPokemon:" + FullHpBuffForUserPokemon + ","
        + "defTrainerBuffAfterUseCount:" + DefTrainerBuffAfterUseCount + ","
        + "defTrainerBuffAfterUse:" + DefTrainerBuffAfterUse + ","
        + "selfTrainerBuffAfterUseCount:" + SelfTrainerBuffAfterUseCount + ","
        + "selfTrainerBuffAfterUse:" + SelfTrainerBuffAfterUse + ","
        + "selfBattleBuffCountAfterUse:" + SelfBattleBuffCountAfterUse + ","
        + "selfBattleBuffAfterUse:" + SelfBattleBuffAfterUse + ","
        + "hitRate:" + HitRate + ","
        + "mulAttackTimes:" + Luban.StringUtil.CollectionToString(MulAttackTimes) + ","
        + "changeBattleEnvAfterUse:" + ChangeBattleEnvAfterUse + ","
        + "usingDefToCalDamage:" + UsingDefToCalDamage + ","
        + "darwTargetCardConfigAfterUse:" + DarwTargetCardConfigAfterUse + ","
        + "darwLeaderCardCountAfterUse:" + DarwLeaderCardCountAfterUse + ","
        + "darwCardCountAfterUse:" + DarwCardCountAfterUse + ","
        + "effectHitRate:" + EffectHitRate + ","
        + "increaseSelfDefPointAfterUse:" + IncreaseSelfDefPointAfterUse + ","
        + "type2:" + Type2 + ","
        + "increaseHealthPointAfterUse:" + IncreaseHealthPointAfterUse + ","
        + "increaseHealthPercentAfterUse:" + IncreaseHealthPercentAfterUse + ","
        + "increaseSelfSpeedPointAfterUse:" + IncreaseSelfSpeedPointAfterUse + ","
        + "defDiscardCardRateWhenHitted:" + DefDiscardCardRateWhenHitted + ","
        + "defDiscardCountWhenHitted:" + DefDiscardCountWhenHitted + ","
        + "defDiscardCountAnyway:" + DefDiscardCountAnyway + ","
        + "userDiscardCountAnyway:" + UserDiscardCountAnyway + ","
        + "fullHpIncreaseBaseValueRate:" + FullHpIncreaseBaseValueRate + ","
        + "percentageDamageBySelf:" + PercentageDamageBySelf + ","
        + "changeElementAfterUse:" + ChangeElementAfterUse + ","
        + "increaseDamagePointWhenGrassEnv:" + IncreaseDamagePointWhenGrassEnv + ","
        + "}";
    }
}

}
