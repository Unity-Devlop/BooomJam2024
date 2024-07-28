using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.GamePlay
{
    public class BattlePosition : MonoBehaviour
    {
        public IBattleTrainer battleTrainer;
        public HuluData currentData { get; private set; } // 当前上场的数据
        public HuluData prepareData { get; private set; } // 准备上场的数据

        public Hulu visual;


        public async UniTask ExecuteEnter()
        {
            // 执行入场逻辑
            Debug.LogWarning($"{currentData}入场");
            await UniTask.DelayFrame(1);
        }

        public void Prepare(HuluData data)
        {
            prepareData = data;
        }

        public void Prepare2Current()
        {
            currentData = prepareData;
            prepareData = null;
            visual.UnBind();
            visual.Bind(currentData);
        }

        public async UniTask ExecuteSkill(ActiveSkillBattleOperation operation)
        {
        }

        public async UniTask ClearRoundData()
        {
        }

        public bool CanFight()
        {
            return !currentData.HealthIsZero();
        }
    }
}