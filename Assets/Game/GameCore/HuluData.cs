using System;
using System.Collections.Generic;
using cfg;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class HuluData
    {
        [HorizontalGroup("1")] public HuluEnum id;

        [HorizontalGroup("1"), Button]
        private void Roll9Skills()
        {
            if (ownedSkills == null)
                ownedSkills = new List<ActiveSkillData>(9);
            for (int i = 0; i < 9; i++)
            {
                ownedSkills.Add(new ActiveSkillData()
                {
                    id = config.SkillPool[UnityEngine.Random.Range(0, config.SkillPool.Length)]
                });
            }
        }

        [HorizontalGroup("1"), Button]
        private void RollAbility()
        {
            hp = UnityEngine.Random.Range(config.BaseHp, config.MaxHp);
            atk = UnityEngine.Random.Range(config.BaseAtk, config.MaxAtk);
            def = UnityEngine.Random.Range(config.BaseDef, config.MaxDef);
            speed = UnityEngine.Random.Range(config.BaseSpeed, config.MaxSpeed);
            adap = UnityEngine.Random.Range(config.BaseAdap, config.MaxAdap);
            
        }


        public BindData<HuluData> bind { get; private set; }

        public HuluConfig config => Global.Table.HuluTable.Get(id);
        public PassiveSkillConfig passiveSkillConfig => Global.Table.PassiveSkillTable.Get(config.PassiveSkill);

        public List<ActiveSkillData> ownedSkills;

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