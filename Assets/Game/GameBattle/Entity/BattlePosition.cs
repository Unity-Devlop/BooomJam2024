using System;
using System.Threading.Tasks;
using cfg;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GamePlay
{
    public class BattlePosition : MonoBehaviour
    {
        public IBattleTrainer battleTrainer;
        public HuluData currentData { get; private set; } // 当前上场的数据
        public HuluData next { get; private set; } // 准备上场的数据

        public Hulu visual;
        public Transform atkPos;

        public async UniTask ExecuteEnter()
        {
            visual.gameObject.SetActive(false);
            await UniTask.DelayFrame(6);
            // 执行入场逻辑
            Debug.LogWarning($"{gameObject.name}-{currentData}入场");
            visual.gameObject.SetActive(true);
        }

        public void SetNext(HuluData data)
        {
            next = data;
        }

        public async UniTask Prepare2Current()
        {
            currentData = next;
            next = null;
            visual.UnBind();
            visual.Bind(currentData);
        }

        public async UniTask ExecuteSkill(ActiveSkillBattleOperation operation)
        {
            if (operation.data.config.Type == ActiveSkillTypeEnum.指挥)
            {
                Debug.LogWarning($"{this}-{currentData}使用指挥技能:{operation.data}");
                return;
            }

            bool flag = false;

            Vector3 origin = visual.transform.position;
            var t = visual.transform.DOMove(atkPos.position, 0.5f).SetEase(Ease.OutBack);
            t.onComplete += () =>
            {
                visual.transform.position = origin;
                flag = true;
            };
            await UniTask.WaitUntil(() => flag);
        }

        public async UniTask RoundEnd()
        {
        }

        public bool CanFight()
        {
            return !currentData.HealthIsZero();
        }

        public override string ToString()
        {
            return $"{gameObject.name}-{currentData}";
        }
    }
}