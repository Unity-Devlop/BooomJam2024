using cfg;
using Game.GamePlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public enum Ability
    {
        Health,
        Attack,
        Defence,
        Speed,
        Adaptability,
        Random,
    }

    [CreateAssetMenu(fileName ="TrainContent",menuName ="ScriptableObject/TrainContent")]
    public class TrainContent : ScriptableObject
    {
        public string id;
        public string title;
        public string description;
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
        public int minRandom;
        public int maxRandom;
        public List<Ability> abilities = new List<Ability>();

        public void Train(HuluData hulu)
        {
            for(int i=0;i<abilities.Count;++i)
            {
                switch(abilities[i])
                {
                    case Ability.Health:
                        hulu.hp += Random.Range(minHealth, maxHealth + 1);
                        break;
                    case Ability.Attack:
                        hulu.atk += Random.Range(minAttack, maxAttack + 1);
                        break;
                    case Ability.Defence:
                        hulu.def += Random.Range(minDefence, maxDefence + 1);
                        break;
                    case Ability.Speed:
                        hulu.speed += Random.Range(minSpeed, maxSpeed + 1);
                        break;
                    case Ability.Adaptability:
                        hulu.adap += Random.Range(minAdaptability, maxAdaptability + 1);
                        break;
                    case Ability.Random:
                        RandomAbility(hulu);
                        break;
                    default:
                        break;
                }
            }
        }

        private void RandomAbility(HuluData hulu)
        {
            int r = Random.Range(0, 5);
            int value = Random.Range(minRandom, maxRandom + 1);
            switch(r)
            {
                case 0:
                    hulu.hp += value;
                    break;
                case 1:
                    hulu.atk += value;
                    break;
                case 2:
                    hulu.def += value;
                    break;
                case 3:
                    hulu.speed += value;
                    break;
                case 4:
                    hulu.adap += value;
                    break;
                default:
                    break;
            }
        }
    }
}
