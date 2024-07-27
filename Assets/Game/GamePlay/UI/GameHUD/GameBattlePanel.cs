using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class GameBattlePanel : UIPanel
    {
        public CardHorizontalContainer selfCardContainer;

        public BattleData data;

        public void Bind(BattleData battleData)
        {
            data = battleData;
            selfCardContainer.Spawn(battleData.allSkills);
        }

        public void UnBind()
        {
        }
    }
}