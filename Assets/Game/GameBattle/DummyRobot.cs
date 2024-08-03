using System;
using System.Collections.Generic;
using cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityToolkit;

namespace Game.GamePlay
{
    [Serializable]
    public class DummyRobot : PlayerBattleTrainer
    {
        public override async UniTask<IBattleOperation> CalOperation()
        {
            Debug.Log($"{this} 开始思考操作");
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