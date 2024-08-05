using cfg;
using UnityEngine;
using UnityToolkit;

namespace Game
{
    public class CardVisualPool : MonoBehaviour
    {
        [SerializeField] SerializableDictionary<ActiveSkillTypeEnum, EasyGameObjectPool> _poolDict;

        public CardVisual Get(ActiveSkillTypeEnum type)
        {
            return _poolDict[type].Get().GetComponent<CardVisual>();
        }

        public void Release(ActiveSkillTypeEnum type, CardVisual visual)
        {
            _poolDict[type].Release(visual.gameObject);
        }
    }
}