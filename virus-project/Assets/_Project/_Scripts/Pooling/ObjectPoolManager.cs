using System.Collections.Generic;
using UnityEngine;

namespace Virus
{
    public partial class ObjectPoolManager : Singleton<IPoolingSource>, IPoolingSource
    {
        [SerializeField] private Transform _root = null;

        private Dictionary<string, Queue<GameObject>> _pool = new Dictionary<string, Queue<GameObject>>();

        public GameObject Borrow(GameObject prefab)
        {
            if (prefab == null) return null;

            var prefabName = prefab.name;

            if (!_pool.ContainsKey(prefabName))
            {
                _pool.Add(prefabName, new Queue<GameObject>());
            }

            if (_pool[prefabName].Count == 0)
            {
                var instance = Instantiate(prefab, _root);
                instance.name = prefabName;
                return instance;
            }

            var gameObject = _pool[prefabName].Dequeue();
            gameObject.SetActive(true);
            return gameObject;
        }

        public T Borrow<T>(GameObject prefab) where T : class
        {
            var gameObject = Borrow(prefab);
            return gameObject ? gameObject.GetComponent(typeof(T)) as T : null;
        }

        public void Return(GameObject component)
        {
            if (component == null) return;

            var prefabName = component.name;

            if (!_pool.ContainsKey(prefabName))
            {
                _pool.Add(prefabName, new Queue<GameObject>());
            }

            component.SetActive(false);
            component.transform.SetParent(_root);
            _pool[prefabName].Enqueue(component);
        }
    }

    public partial class ObjectPoolManager
    {
        GameObject IPoolingSource.Borrow(GameObject prefab) => Borrow(prefab);
        T IPoolingSource.Borrow<T>(GameObject prefab) => Borrow<T>(prefab);
        T IPoolingSource.Borrow<T>(T prefab) => Borrow<T>(prefab.gameObject);
        void IPoolingSource.Return(GameObject gameObject) => Return(gameObject);
    }
}
