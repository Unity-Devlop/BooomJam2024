using System;
using cfg;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class CardVisualPool : MonoBehaviour
    {
        [SerializeField] SerializableDictionary<ActiveSkillTypeEnum, EasyGameObjectPool> _poolDict;

        public CardVisual Get(ActiveSkillEnum id)
        {
            throw new NotImplementedException();
        }

        public void Release(ActiveSkillTypeEnum type, CardVisual visual)
        {
            throw new NotImplementedException();
        }
    }
}