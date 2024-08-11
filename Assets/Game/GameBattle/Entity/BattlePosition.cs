using System;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.GamePlay
{
    public class BattlePosition : MonoBehaviour
    {
        public IBattleTrainer battleTrainer;
        public HuluData current { get; private set; } // 当前上场的数据
        public HuluData next { get; private set; } // 准备上场的数据

        public HuluVisual visual;
        public Direction direction;

        public async UniTask ExecuteEnter()
        {
            visual.transform.localScale = Vector3.zero;
            visual.transform.DOScale(Vector3.one, 0.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
            // 执行入场逻辑
            Debug.Log($"{gameObject.name}-{current}入场");
        }

        public void PrepareNext(HuluData data)
        {
            next = data;
        }

        public async UniTask NextReplaceCurrent()
        {
            if (current != null)
            {
                current.OnAttainBuffEvent -= OnAttainBuff;
                current.OnLoseBuffEvent -= OnLoseBuff;
                current.OnDamageEvent -= OnDamage;
                current.OnHealEvent -= OnHeal;

                visual.transform.localScale = Vector3.one;
                visual.transform.DOScale(Vector3.zero, 0.5f);
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: destroyCancellationToken);
                Debug.Log($"{gameObject.name}-{current}离场");
            }

            current = next;
            next = null;
            Assert.IsNotNull(current);
            current.OnAttainBuffEvent += OnAttainBuff;
            current.OnLoseBuffEvent += OnLoseBuff;
            current.OnDamageEvent += OnDamage;
            current.OnHealEvent += OnHeal;


            visual.UnBind();
            visual.Bind(current, direction);


            await UniTask.CompletedTask;
        }

        private async UniTask OnHeal()
        {
            await UniTask.CompletedTask;
        }

        private async UniTask OnLoseBuff(BattleBuffEnum arg)
        {
            await UniTask.CompletedTask;
        }

        private async UniTask OnDamage()
        {
            await UniTask.CompletedTask;
        }

        private async UniTask OnAttainBuff(BattleBuffEnum arg)
        {
            await UniTask.CompletedTask;
        }

        public async UniTask ExecuteSkill(ActiveSkillBattleOperation operation)
        {
            if (operation.data.config.Type == ActiveSkillTypeEnum.指挥)
            {
                Debug.LogWarning($"{this}-{current}使用指挥技能:{operation.data} 未实现动画");
                await Global.Event.SendWithResult<OnExecuteSkill, UniTask>(
                    new OnExecuteSkill(battleTrainer, operation.data));
                return;
            }

            if ((operation.data.config.Type & ActiveSkillTypeEnum.变化技能) != 0)
            {
                Debug.LogWarning($"{this}-{current}使用变化技能:{operation.data} 未实现动画");
                await UniTask.WhenAll(
                    Global.Event.SendWithResult<OnExecuteSkill, UniTask>(
                        new OnExecuteSkill(battleTrainer, operation.data)),
                    visual.ExecuteSkill(operation.data));
                return;
            }

            Global.LogInfo($"{this}-{current}使用主动技能:{operation.data}");
            await UniTask.WhenAll(
                Global.Event.SendWithResult<OnExecuteSkill, UniTask>(
                    new OnExecuteSkill(battleTrainer, operation.data)),
                visual.ExecuteSkill(operation.data));
        }

        public async UniTask RoundEnd()
        {
            // Debug.LogWarning($"{gameObject.name}-回合结束");
            await UniTask.CompletedTask;
        }

        public override string ToString()
        {
            return $"{battleTrainer.trainerData.name}-{current}";
        }

        public async UniTask OnTakeSkillFrom(ActiveSkillData skill, IBattleTrainer userTrainer,
            BattlePosition userPosition, int damage)
        {
            if (skill.config.Type == ActiveSkillTypeEnum.指挥)
            {
                Global.LogInfo($"{this}-{current}受到指挥技能:{skill}");
                return;
            }

            if ((skill.config.Type & ActiveSkillTypeEnum.变化技能) != 0)
            {
                Global.LogInfo($"{this}-{current}受到变化技能:{skill}");
                return;
            }

            if ((skill.config.Type & ActiveSkillTypeEnum.伤害技能) != 0)
            {
                Global.LogInfo($"{this}-{current}受到主动技能:{skill}");
                await visual.PlayTakeDamageAnimation();
                return;
            }
        }
    }
}