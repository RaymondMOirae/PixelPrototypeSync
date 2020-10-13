using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Utils
{
    public class GameObjectPool : Singleton<GameObjectPool>
    {
        private static Dictionary<GameObject, Pool> prefabPools = new Dictionary<GameObject, Pool>();

        private class Pool
        {
            public string defaultObjectName = "[GameObject]";
            
            public Func<GameObject> Allocator;
            public GameObject ObjectCollection;
            Stack<GameObject> objectPool = new Stack<GameObject>();

            public Pool(GameObject objectCollection, Func<GameObject> allocator)
            {
                ObjectCollection = objectCollection;
                Allocator = allocator;
            }
            
            GameObject CreateObject()
            {
                var newObj = Allocator.Invoke();
                newObj.name = defaultObjectName;
                return newObj;
            }

            public GameObject Get()
            {
                if (objectPool.Count > 0)
                {
                    var obj = objectPool.Pop();
                    obj.SetActive(true);
                    // obj.transform.parent = null;
                    obj.transform.SetParent(null);
                    return obj;
                }

                return CreateObject();
            }

            public GameObject Get(string name)
            {
                var obj = Get();
                obj.name = name;
                return obj;
            }

            public void Release(GameObject obj)
            {
                if (!obj)
                    return;
                // obj.transform.parent = transform;
                obj.transform.SetParent(ObjectCollection.transform);
                obj.layer = 0;
                obj.SetActive(false);
                objectPool.Push(obj);
            }

            public void PreAlloc(int count)
            {
                for (var i = 0; i < count; i++)
                {
                    var obj = CreateObject();
                    Release(obj);
                }
            }
        }

        private static GameObject Get(GameObject prefab) => GetOrCreatePrefabPool(prefab).Get();

        private static GameObject Get(GameObject prefab, string name) => GetOrCreatePrefabPool(prefab).Get(name);
        
        private static void Release(GameObject prefab, GameObject obj)
        {
            GetOrCreatePrefabPool(prefab).Release(obj);
        }

        public static T Get<T>(GameObject prefab) where T : Component
            => Get(prefab)?.GetComponent<T>();

        public static T Get<T>(GameObject prefab, string name) where T : Component
            => Get(prefab, name).GetComponent<T>();

        public static void Release<T>(GameObject prefab, T component) where T : Component
        {
            if(component && component.gameObject)
                Release(prefab, component.gameObject);
        }

        public static void PreAlloc(GameObject prefab, int count)
        {
            GetOrCreatePrefabPool(prefab).PreAlloc(count);
        }

        static Pool GetOrCreatePrefabPool(GameObject prefab)
        {
            if (prefabPools.ContainsKey(prefab))
            {
                var existedPool = prefabPools[prefab];
                if (!(existedPool is null))
                    return existedPool;
            }
            var pool = CreatePrefabPool(prefab);
            prefabPools[prefab] = pool;
            return pool;
        }

        static Pool CreatePrefabPool(GameObject prefab)
        {
            var pool = new Pool(new GameObject(), ()=>Instantiate(prefab));
            pool.ObjectCollection.transform.parent = Instance.transform;
            pool.defaultObjectName = prefab.name;
            pool.ObjectCollection.name = "[Pool]" + prefab.name;
            return pool;
        }

        protected override void Awake()
        {
            base.Awake();
            gameObject.ClearChildren();
        }
    }
}