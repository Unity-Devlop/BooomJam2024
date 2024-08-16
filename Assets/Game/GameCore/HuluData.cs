using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityToolkit;

namespace Game
{
    [Serializable]
    public class HuluData
    {
        [HorizontalGroup("1")] public HuluEnum id;
        public int cost = 0;

        public Guid guid { get;private set;}


        [JsonIgnore] public BindData<HuluData, UniTask> bind { get; private set; }

        [JsonIgnore] public HuluConfig config => Global.Table.HuluTable.Get(id);

        [JsonIgnore]
        public PassiveSkillConfig passiveSkillConfig =>
            Global.Table.PassiveSkillTable.Get(Global.Table.HuluTable.Get(id).PassiveSkill);

        public string name => config.Id.ToString(); // TODO 个性化


        [NonSerialized] public int skillTimes = 0; // Ugly 自由者
        [NonSerialized] public int enterTimes = 0; // 第几次进入战场
        [ShowInInspector] private List<BattleBuffEnum> buffList; // 守护
        private int healP0intBy回满血然后回合结束受到等量伤害 = 0;




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


        public event Func<UniTask> OnHealEvent = delegate { return UniTask.CompletedTask; };
        public event Func<UniTask> OnDamageEvent = delegate { return UniTask.CompletedTask; };

        public event Func<BattleBuffEnum, UniTask> OnAttainBuffEvent = delegate { return UniTask.CompletedTask; };
        public event Func<BattleBuffEnum, UniTask> OnLoseBuffEvent = delegate { return UniTask.CompletedTask; };

        public event Func<int,UniTask> OnIncreaseAtkEvent = delegate { return UniTask.CompletedTask; };
        public event Func<int,UniTask> OnIncreaseSpeedEvent = delegate { return UniTask.CompletedTask; };
        
        public event Func<int,UniTask> OnIncreaseDefEvent = delegate { return UniTask.CompletedTask; };
        
        
        public HuluData()
        {
            guid = Guid.NewGuid();
            bind = new BindData<HuluData, UniTask>(this);
            ownedSkills = new List<ActiveSkillData>();
            buffList = new List<BattleBuffEnum>();
        }

        public HuluData(HuluEnum id)
        {
            bind = new BindData<HuluData, UniTask>(this);
            ownedSkills = new List<ActiveSkillData>();
            buffList = new List<BattleBuffEnum>();
            this.id = id;
            hp = config.BaseHp;
            atk = config.BaseAtk;
            def = config.BaseDef;
            speed = config.BaseSpeed;
            adap = config.BaseAdap;
        }

        public void RemoveOwnedSkill(ActiveSkillEnum id)
        {
            Assert.IsFalse(id == ActiveSkillEnum.None);
            int index = -1;
            for (int i = 0; i < ownedSkills.Count; ++i)
            {
                if (ownedSkills[i].id == id)
                {
                    index = i;
                    break;
                }
            }

            if (index >= 0) ownedSkills.RemoveAt(index);
        }

        public void AddOwnedSkill(ActiveSkillEnum id)
        {
            Assert.IsFalse(id == ActiveSkillEnum.None);
            ActiveSkillData data = new ActiveSkillData();
            data.id = id;
            ownedSkills.Add(data);
            ownedSkills.Sort((a, b) => a.id.CompareTo(b.id));
        }

        public void ReplaceOwnedSkill(ActiveSkillEnum ori, ActiveSkillEnum target)
        {
            Assert.IsFalse(ori == ActiveSkillEnum.None);
            Assert.IsFalse(target == ActiveSkillEnum.None);
            int index = -1;
            for (int i = 0; i < ownedSkills.Count; ++i)
            {
                if (ownedSkills[i].id == ori)
                {
                    ownedSkills[i].id = target;
                    break;
                }
            }
            ownedSkills.Sort((a, b) => a.id.CompareTo(b.id));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask DecreaseHealth(int delta, HuluData attacker = null)
        {
            if (buffList.Contains(BattleBuffEnum.规避弱点) && delta > 0)
            {
                await RemoveBuff(BattleBuffEnum.规避弱点);
                delta /= 2;
            }

            if (attacker != null && ContainsBuff(BattleBuffEnum.自己受到伤害时攻击方也会受到等量伤害))
            {
                await RemoveBuff(BattleBuffEnum.自己受到伤害时攻击方也会受到等量伤害);
                await attacker.DecreaseHealth(delta, null);
            }

            if (delta > 0)
            {
                await OnDamageEvent();
            }
            else if (delta < 0)
            {
                await OnHealEvent();
            }

            currentHp -= delta;
            currentHp = Mathf.Clamp(currentHp, 0, hp);

            if (buffList.Contains(BattleBuffEnum.站起来) && currentHp <= 0)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{this}站起来"));
                await RemoveBuff(BattleBuffEnum.站起来);
                await DecreaseHealth(-1, null);
            }

            if (id == HuluEnum.斯托姆 && passiveSkillConfig.Id == PassiveSkillEnum.狂风不灭 &&
                buffList.Contains(BattleBuffEnum.狂风不灭) && currentHp <= 0)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{this}狂风不灭"));
                await RemoveBuff(BattleBuffEnum.狂风不灭);
                await DecreaseHealth(-hp / 2);
            }

            if (id == HuluEnum.枯木妖 && passiveSkillConfig.Id == PassiveSkillEnum.枯木逢春)
            {
                Global.Event.Send(new BattleInfoRecordEvent($"{this}枯木逢春"));
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
        public async UniTask IncreaseCurrentSpeed(int delta)
        {
            currentSpeed += delta;
            currentSpeed = Mathf.Clamp(currentSpeed, 0, speed);
            Global.Event.Send<BattleInfoRecordEvent>(new BattleInfoRecordEvent($"{this}速度提高{delta}"));
            await OnIncreaseSpeedEvent(delta);
            await bind.Invoke();
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask IncreaseAtk(int delta,bool ignoreMax = false)
        {
            int max = int.MaxValue;
            if (!ignoreMax)
            {
                max = atk;
            }
            int nextAtk = Mathf.Clamp(currentAtk + delta, 0, max);
            currentAtk = nextAtk;
            await OnIncreaseAtkEvent(delta);
            await bind.Invoke();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async UniTask IncreaseDef(int delta)
        {
            int nextDef = Mathf.Clamp(currentDef + delta, 0, def);
            currentDef = nextDef;
            await OnIncreaseDefEvent(delta);
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
            // Global.LogInfo($"{this}回合结束");
            if (healP0intBy回满血然后回合结束受到等量伤害 > 0)
            {
                Global.LogInfo($"{this}回合结束受到等量伤害:{healP0intBy回满血然后回合结束受到等量伤害}");
                await DecreaseHealth(healP0intBy回满血然后回合结束受到等量伤害);
            }

            healP0intBy回满血然后回合结束受到等量伤害 = 0;
            GameMath.ProcessBuffWhenRoundEnd(this.buffList);
            await bind.Invoke();
        }



        public async UniTask AddBuff(BattleBuffEnum buff)
        {
            var buffConfig = Global.Table.BattleBuffTable.Get(buff);

            Assert.IsFalse(buffConfig.IsTrainerBuff);
            if (buff == BattleBuffEnum.回满血然后回合结束受到等量伤害)
            {
                healP0intBy回满血然后回合结束受到等量伤害 += hp - currentHp;
                await DecreaseHealth(-healP0intBy回满血然后回合结束受到等量伤害);
            }


            if (buffConfig.NotSave)
            {
                return;
            }

            if (!buffConfig.CanStack && buffList.Contains(buff))
            {
                return;
            }

            await OnAttainBuffEvent(buff);
            buffList.Add(buff);
        }

        public bool ContainsBuff(BattleBuffEnum buff)
        {
            return buffList.Contains(buff);
        }

        public async UniTask RemoveBuff(BattleBuffEnum buff)
        {
            buffList.Remove(buff);
            await OnLoseBuffEvent(buff);
            await UniTask.CompletedTask;
        }

        public async UniTask UseSkill(ActiveSkillData skill, IBattleTrainer tar, int times)
        {
            if (times == 0 && skill.config.Element == ElementEnum.风 &&
                buffList.Contains(BattleBuffEnum.下次一次使用风属性时速度提高20))
            {
                await RemoveBuff(BattleBuffEnum.下次一次使用风属性时速度提高20);
                await IncreaseCurrentSpeed(20);
            }

            if (times == 0 && skill.id == ActiveSkillEnum.喙啄 && buffList.Contains(BattleBuffEnum.使用喙啄时对方丢一张牌))
            {
                await RemoveBuff(BattleBuffEnum.使用喙啄时对方丢一张牌);
                await tar.RandomDiscardCardFromHand(1);
            }
        }

        public int BuffCount(BattleBuffEnum buff)
        {
            int cnt = 0;
            foreach (var battleBuffEnum in buffList)
            {
                if (battleBuffEnum == buff)
                {
                    cnt++;
                }
            }

            return cnt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanFight()
        {
            return currentHp > 0;
        }
        internal void ClearBattleDirtyData()
        {
            skillTimes = 0;
            enterTimes = 0;
            buffList.Clear();
            healP0intBy回满血然后回合结束受到等量伤害 = 0;
        }
        
        [HorizontalGroup("1"), Button]
        public void Roll9Skills()
        {
            RollTargetSkills(9);
        }

        public void RollTargetSkills(int skillCnt)
        {
            if (ownedSkills == null)
                ownedSkills = new List<ActiveSkillData>(9);
            for (int i = 0; i < skillCnt; i++)
            {
                ownedSkills.Add(new ActiveSkillData()
                {
                    id = config.SkillPool[UnityEngine.Random.Range(0, config.SkillPool.Length)]
                });
            }
            ownedSkills.Sort((a, b) => a.id.CompareTo(b.id));
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
    }
}