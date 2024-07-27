using System;
using System.Collections.Generic;
using cfg;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class HuluData
    {
        public HuluEnum id;
        public BindData<HuluData> bind { get; private set; }

        public HuluConfig config => Global.Table.HuluTable.Get(id);
        public PassiveSkillConfig passiveSkillConfig => Global.Table.PassiveSkillTable.Get(config.PassiveSkill);

        public List<ActiveSkillEnum> ownedSkills;

        /// <summary>
        /// 生命
        /// </summary>
        public int hp;

        /// <summary>
        /// 攻击
        /// </summary>
        public int atk;

        /// <summary>
        /// 防御
        /// </summary>
        public int def;

        /// <summary>
        /// 速度
        /// </summary>
        public int speed;

        /// <summary>
        /// 适应力
        /// </summary>
        public int adap;

        public HuluData()
        {
            bind = new BindData<HuluData>(this);
        }
    }
}