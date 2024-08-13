using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

namespace UnityToolkit
{
    public class EasyGameObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform hidden;
        [SerializeField] private int initSize = 10;
        [SerializeField] private int maxSize = 100;

        private ObjectPool<GameObject> _pool;
        private bool _initialized;

        private void Awake()
        {
            if (prefab != null)
            {
                Initialize(hidden, prefab);
            }
        }

        public void Initialize(Transform parent = null, GameObject target = null)
        {
            if (target != null)
            {
                this.prefab = target;
            }

            if (parent == null) parent = hidden;
            Assert.IsNotNull(this.prefab);
            if (_initialized) return;
            _initialized = true;
            _pool = new ObjectPool<GameObject>(
                () => Instantiate(prefab, parent),
                (obj) =>
                {
                    if (obj.TryGetComponent(out IPoolObject poolObject))
                    {
                        poolObject.OnGet();
                    }
                    else
                    {
                        obj.SetActive(true);
                    }
                },
                (obj) =>
                {
                    if (obj.TryGetComponent(out IPoolObject poolObject))
                    {
                        poolObject.OnRelease();
                    }
                    else
                    {
                        obj.SetActive(false);
                    }

                    obj.transform.SetParent(hidden);
                }, DestroyImmediate, true, initSize, maxSize
            );
        }

        public GameObject Get() => _pool.Get();
        public void Release(GameObject obj) => _pool.Release(obj);
    }
}