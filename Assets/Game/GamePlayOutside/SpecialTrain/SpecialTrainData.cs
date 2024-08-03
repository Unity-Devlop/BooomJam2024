using Game.GamePlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "SpecialTrainData", menuName = "ScriptableObject/SpecialTrainData")]
    public class SpecialTrainData : ScriptableObject
    {
        public int minHealth;
        public int maxHealth;
        public int minAttack;
        public int maxAttack;
        public int minDefence;
        public int maxDefence;
        public int minSpeed;
        public int maxSpeed;
        public int minAdaptability;
        public int maxAdaptability;

        public int Train(Ability ability)
        {
            switch (ability)
            {
                case Ability.Health:
                    return Random.Range(minHealth, maxHealth + 1);
                case Ability.Attack:
                    return Random.Range(minAttack, maxAttack + 1);
                case Ability.Defence:
                    return Random.Range(minDefence, maxDefence + 1);
                case Ability.Speed:
                    return Random.Range(minSpeed, maxSpeed + 1);
                case Ability.Adaptability:
                    return Random.Range(minAdaptability, maxAdaptability + 1);
                default:
                    break;
            }
            return 0;
        }
    }
}
