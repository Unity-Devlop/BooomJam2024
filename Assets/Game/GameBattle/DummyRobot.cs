using System;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.GamePlay
{
    [Serializable]
    public class DummyRobot : PlayerBattleTrainer
    {
        public override async UniTask<IBattleOperation> CalOperation()
        {
            await UniTask.DelayFrame(1);
            if (handZone.Count == 0)
            {
                return new EndRoundOperation();
            }
            var target = handZone.RandomTakeWithoutRemove();
            IBattleOperation operation = new ActiveSkillBattleOperation()
            {
                data = target
            };
            return operation;
        }
    }
}