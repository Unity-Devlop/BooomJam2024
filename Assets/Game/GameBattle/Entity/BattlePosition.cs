using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.GamePlay
{
    public class BattlePosition : MonoBehaviour
    {
        public ITrainer trainer;
        public HuluData currentData;// 当前上场的数据
        public HuluData prepareData;// 准备上场的数据

        public async UniTask ExecuteEnter()
        {
            // 执行入场逻辑
            Debug.LogWarning($"{currentData}入场");
            await UniTask.DelayFrame(1);
        }
    }
}