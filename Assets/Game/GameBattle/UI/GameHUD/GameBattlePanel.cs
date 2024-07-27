using System.Collections.Generic;
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
        }

        private void DrawCard(List<ActiveSkillData> obj)
        {
            selfCardContainer.Spawn(obj);
        }

        public void UnBind()
        {
            data.OnDrawCard -= DrawCard;
        }
    }
}