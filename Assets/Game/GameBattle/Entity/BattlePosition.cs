using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

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
            visual.gameObject.SetActive(false);
            await UniTask.DelayFrame(6);
            // 执行入场逻辑
            Debug.LogWarning($"{gameObject.name}-{current}入场");
            visual.gameObject.SetActive(true);
        }

        public void SetNext(HuluData data)
        {
            next = data;
        }

        public async UniTask Prepare2Current()
        {
            current = next;
            next = null;
            visual.UnBind();
            visual.Bind(current, direction);
            await UniTask.CompletedTask;
        }

        public async UniTask ExecuteSkill(ActiveSkillBattleOperation operation)
        {
            if (operation.data.config.Type == ActiveSkillTypeEnum.指挥)
            {
                Debug.LogWarning($"{this}-{current}使用指挥技能:{operation.data}");
                return;
            }

            if ((operation.data.config.Type & ActiveSkillTypeEnum.变化技能) != 0)
            {
                Debug.LogWarning($"{this}-{current}使用变化技能:{operation.data}");
                return;
            }

            Global.LogInfo($"{this}-{current}使用主动技能:{operation.data}");
            await visual.ExecuteSkill(operation.data);
        }

        public async UniTask RoundEnd()
        {
            Debug.LogWarning($"{gameObject.name}-回合结束");
            await UniTask.CompletedTask;
        }

        public bool CanFight()
        {
            return !current.HealthIsZero();
        }

        public override string ToString()
        {
            return $"{gameObject.name}-{current}";
        }
    }
}