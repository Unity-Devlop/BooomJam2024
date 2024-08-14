using System;
using cfg;
using UnityEngine;
using UnityEngine.Assertions;
using UnityToolkit;

namespace Game
{
    public class CardVisualPool : MonoBehaviour
    {
        [SerializeField] private GameObject commandCardPrefab;
        [SerializeField] private GameObject damageSkillCardPrefab;
        [SerializeField] private GameObject buffSkillCardPrefab;
        [SerializeField] private GameObject specialCardPrefab;


        private EasyGameObjectPool _commandCardPool;
        private EasyGameObjectPool _damageSkillCardPool;
        private EasyGameObjectPool _buffSkillCardPool;
        private EasyGameObjectPool _specialCardPool;

        private void Awake()
        {
            _commandCardPool = gameObject.AddComponent<EasyGameObjectPool>();
            _damageSkillCardPool = gameObject.AddComponent<EasyGameObjectPool>();
            _buffSkillCardPool = gameObject.AddComponent<EasyGameObjectPool>();
            _specialCardPool = gameObject.AddComponent<EasyGameObjectPool>();

            _commandCardPool.Initialize(transform, commandCardPrefab);
            _damageSkillCardPool.Initialize(transform, damageSkillCardPrefab);
            _buffSkillCardPool.Initialize(transform, buffSkillCardPrefab);
            _specialCardPool.Initialize(transform, specialCardPrefab);
        }


        public CardVisual Get(ActiveSkillEnum id)
        {
            var config = Global.Table.ActiveSkillTable.Get(id);
            EasyGameObjectPool pool;
            if (id == ActiveSkillEnum.保时捷的赞助)
            {
                pool = _specialCardPool;
            }
            else
            {
                pool = Id2Pool(config.Type);
            }

            Assert.IsNotNull(pool);
            GameObject go = pool.Get();
            CardVisual visual = go.GetComponent<CardVisual>();
            visual.transform.SetParent(transform);
            return visual;
        }

        public void Release(ActiveSkillEnum id, CardVisual visual)
        {
            EasyGameObjectPool pool;
            if (id == ActiveSkillEnum.保时捷的赞助)
            {
                pool = _specialCardPool;
            }
            else
            {
                pool = Id2Pool(visual.id);
            }

            Assert.IsNotNull(pool);
            pool.Release(visual.gameObject);
        }


        private EasyGameObjectPool Id2Pool(ActiveSkillTypeEnum id)
        {
            switch (id)
            {
                case ActiveSkillTypeEnum.None:
                    break;
                case ActiveSkillTypeEnum.指挥:
                    return _commandCardPool;
                case ActiveSkillTypeEnum.伤害技能:
                    return _damageSkillCardPool;
                case ActiveSkillTypeEnum.变化技能:
                    return _buffSkillCardPool;
                default:
                    throw new ArgumentOutOfRangeException(nameof(id), id, null);
            }

            return null;
        }

        public void Initialize()
        {
        }
    }
}