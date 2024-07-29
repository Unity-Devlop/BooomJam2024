using System;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
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
            RecoverAllAbility();
        }

        [HorizontalGroup("1"), Button]
        public void RecoverAllAbility()
        {
            currentHp = hp;
            currentAtk = atk;
            currentDef = def;
            currentSpeed = speed;
            currentAdap = adap;
            elementEnum = config.Elements;
        }


        public BindData<HuluData, UniTask> bind { get; private set; }

        public HuluConfig config => Global.Table.HuluTable.Get(id);
        public PassiveSkillConfig passiveSkillConfig => Global.Table.PassiveSkillTable.Get(Global.Table.HuluTable.Get(id).PassiveSkill);
        public string name => config.Id.ToString(); // TODO 个性化


        // Ugly 狂风不灭
        [NonSerialized] public bool canReborn = true;
        [NonSerialized] public int skillTimes = 0;


        public ElementEnum elementEnum;
        
        /// <summary>
        /// 生命
        /// </summary>
        [HorizontalGroup("hp")] public int hp;


        [HorizontalGroup("hp")] public int currentHp;

        /// <summary>
        /// 攻击
        /// </summary>
        [HorizontalGroup("atk")] public int atk;


        [HorizontalGroup("atk")] public int currentAtk;

        /// <summary>
        /// 防御
        /// </summary>
        [HorizontalGroup("def")] public int def;

        [HorizontalGroup("def")] public int currentDef;

        /// <summary>
        /// 速度
        /// </summary>
        [HorizontalGroup("speed")] public int speed;

        [HorizontalGroup("speed")] public int currentSpeed;

        /// <summary>
        /// 适应力
        /// </summary>
        [HorizontalGroup("adap")] public int adap;

        [HorizontalGroup("adap")] public int currentAdap;
        public List<ActiveSkillData> ownedSkills;

        public HuluData()
        {
            bind = new BindData<HuluData, UniTask>(this);
        }

        public async UniTask ChangeHealth(int delta)
        {
            currentHp -= delta;
            if (currentHp < 0)
            {
                currentHp = 0;
            }

            if (currentHp > hp)
            {
                currentHp = hp;
            }

            Debug.Log($"{this}当前生命值{currentHp}");
            UglyMath.PostprocessHuluData(this);
            await bind.Invoke();
            await UglyMath.PostprocessHuluDataWhenDead(this);
        }

        public bool HealthIsZero()
        {
            return currentHp <= 0;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(config.Name))
            {
                return id.ToString();
            }

            return config.Name;
        }
    }
}