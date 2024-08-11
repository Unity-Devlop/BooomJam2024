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
            Global.LogInfo($"{this} 开始思考操作");
            float thinkingTime = UnityEngine.Random.Range(0.01f, 3f);
            await UniTask.Delay(TimeSpan.FromSeconds(thinkingTime - 0.01f));
            if (handZone.Count == 0)
            {
                return new EndRoundOperation();
            }

            PlayerBattleTrainer playerBattleTrainer = GameBattleMgr.Singleton.playerBattleTrainer;

            // 如果自己没血了 尽可能找治疗技能 放 或者 放守护技能

            var target = handZone.RandomTakeWithoutRemove();


            IBattleOperation operation = new ActiveSkillBattleOperation()
            {
                data = target
            };
            return operation;
        }
    }
}