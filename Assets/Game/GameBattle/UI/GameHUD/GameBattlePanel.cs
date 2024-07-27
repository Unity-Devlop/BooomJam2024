using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.GamePlay;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class GameBattlePanel : UIPanel
    {
        public CardHorizontalContainer selfCardContainer;

        public ITrainer data;

        public void Bind(ITrainer battleData)
        {
            data = battleData;
            data.OnDrawCard += DrawCard;
            data.OnDiscardCard += DiscardCard;
        }

        private UniTask DiscardCard(List<ActiveSkillData> arg)
        {
            return UniTask.CompletedTask;
        }

        private async UniTask DrawCard(List<ActiveSkillData> obj)
        {
            await selfCardContainer.Spawn(obj);
        }

        public void UnBind()
        {
            data.OnDrawCard -= DrawCard;
            data.OnDiscardCard -= DiscardCard;
        }
    }
}