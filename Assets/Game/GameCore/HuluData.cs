using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class HuluData
    {
        [HorizontalGroup("1")] public HuluEnum id;

        [HorizontalGroup("1"), Button]
        public void Roll9Skills()
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
        public void RollAbility()
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
        [ShowInInspector] private List<BattleBuffEnum> buffList; // 守护


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
            buffList = new List<BattleBuffEnum>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask DecreaseHealth(int delta, HuluData attacker = null)
        {
            if (buffList.Contains(BattleBuffEnum.规避弱点) && delta > 0)
            {
                RemoveBuff(BattleBuffEnum.规避弱点);
                delta /= 2;
            }

            if (attacker != null && ContainsBuff(BattleBuffEnum.自己受到伤害时攻击方也会受到等量伤害))
            {
                RemoveBuff(BattleBuffEnum.自己受到伤害时攻击方也会受到等量伤害);
                await attacker.DecreaseHealth(delta, null);
            }

            currentHp -= delta;
            currentHp = Mathf.Clamp(currentHp, 0, hp);

            if (buffList.Contains(BattleBuffEnum.站起来) && currentHp <= 0)
            {
                Global.Event.Send(new BattleTipEvent("站起来"));
                Debug.Log("站起来");
                buffList.Remove(BattleBuffEnum.站起来);
                await DecreaseHealth(-1, null);
            }

            if (id == HuluEnum.斯托姆 && passiveSkillConfig.Id == PassiveSkillEnum.狂风不灭 &&
                canReborn)
            {
                Debug.Log("狂风不灭");
                canReborn = false;
                await DecreaseHealth(-hp / 2);
            }
            else if (id == HuluEnum.枯木妖 && passiveSkillConfig.Id == PassiveSkillEnum.枯木逢春)
            {
                Debug.Log($"枯木逢春");
                Global.Event.Send(new BattleTipEvent("枯木逢春"));
                int damageHp = hp - currentHp;
                int cnt = damageHp / 100;
                int adaptIncrease = cnt * 5;
                adaptIncrease = Mathf.Clamp(adaptIncrease, 0, 20);
                currentAdap = adap;
                currentAdap += adaptIncrease;
            }

            await bind.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask TakeDamageFromSelfSkillEffect(int point)
        {
            if (buffList.Contains(BattleBuffEnum.阻止自身技能伤害))
                return;
            await DecreaseHealth(point);
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
            if (buffList.Contains(BattleBuffEnum.阻止属性变化))
            {
                Debug.Log("阻止属性变化");
                return;
            }

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

        public async UniTask RoundEnd()
        {
            if (healP0intBy回满血然后回合结束受到等量伤害 > 0)
            {
                await DecreaseHealth(healP0intBy回满血然后回合结束受到等量伤害);
            }

            healP0intBy回满血然后回合结束受到等量伤害 = 0;
            GameMath.ProcessBuffWhenRoundEnd(this.buffList);
            await bind.Invoke();
        }

        private int healP0intBy回满血然后回合结束受到等量伤害 = 0;

        public async UniTask AddBuff(BattleBuffEnum buff)
        {
            if (buff == BattleBuffEnum.回满血然后回合结束受到等量伤害)
            {
                healP0intBy回满血然后回合结束受到等量伤害 = hp - currentHp;
                await DecreaseHealth(-healP0intBy回满血然后回合结束受到等量伤害);
            }


            var buffConfig = Global.Table.BattleBuffTable.Get(buff);
            if (buffConfig.NotSave)
            {
                return;
            }

            if (!buffConfig.CanStack && buffList.Contains(buff))
            {
                return;
            }

            buffList.Add(buff);
        }

        public bool ContainsBuff(BattleBuffEnum buff)
        {
            return buffList.Contains(buff);
        }

        public void RemoveBuff(BattleBuffEnum buff)
        {
            buffList.Remove(buff);
        }

        public void Add(BattleBuffEnum buff)
        {
            buffList.Add(buff);
        }

        public async UniTask UseSkill(ActiveSkillData skill, IBattleTrainer tar)
        {
            if (skill.config.Element == ElementEnum.风 && buffList.Contains(BattleBuffEnum.下次一次使用风属性时速度提高20))
            {
                RemoveBuff(BattleBuffEnum.下次一次使用风属性时速度提高20);
                await IncreaseCurrentSpeed(20);
            }

            if (skill.id == ActiveSkillEnum.喙啄 && buffList.Contains(BattleBuffEnum.使用喙啄时对方丢一张牌))
            {
                RemoveBuff(BattleBuffEnum.使用喙啄时对方丢一张牌);
                await tar.RandomDiscardCardFromHand(1);
            }
        }
    }
}