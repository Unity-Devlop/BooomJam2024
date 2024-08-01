using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        public PassiveSkillConfig passiveSkillConfig =>
            Global.Table.PassiveSkillTable.Get(Global.Table.HuluTable.Get(id).PassiveSkill);

        public string name => config.Id.ToString(); // TODO 个性化


        [NonSerialized] public bool canReborn = true; // Ugly 狂风不灭
        [NonSerialized] public int skillTimes = 0; // Ugly 自由者
        [NonSerialized] public int enterTimes = 0; // 第几次进入战场
        public List<BuffEnum> buffList = new List<BuffEnum>(); // 守护


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
            ownedSkills = new List<ActiveSkillData>();
            buffList = new List<BuffEnum>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask DecreaseHealth(int delta)
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
            await UglyMath.PostprocessHuluData(this);
            await bind.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask DecreaseCurrentSpeed(int i)
        {
            currentSpeed -= i;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, speed);
            await bind.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask IncreaseAtk(int delta)
        {
            int nextAtk = Mathf.Clamp(currentAtk + delta, 0, atk);
            currentAtk = nextAtk;
            await bind.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask IncreaseDef(int i)
        {
            int nextDef = Mathf.Clamp(currentDef + i, 0, def);
            currentDef = nextDef;
            await bind.Invoke();
        }


        public async UniTask ChangeElement(ElementEnum elementEnum1)
        {
            elementEnum = elementEnum1;
            await bind.Invoke();
        }

        public async UniTask IncreaseCurrentSpeed(int i)
        {
            currentSpeed += i;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, speed);
            Debug.Log($"{this}当前速度{currentSpeed}");
            await bind.Invoke();
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

        public async UniTask ClearRoundData()
        {
            buffList.Remove(BuffEnum.守护);
            buffList.Remove(BuffEnum.站起来);
            buffList.Remove(BuffEnum.规避弱点);
        }
    }
}