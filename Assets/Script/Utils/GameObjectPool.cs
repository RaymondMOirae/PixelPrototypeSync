using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Utils
{
    public class GameObjectPool : Singleton<GameObjectPool>
    {
        private static Dictionary<GameObject, Pool> prefabPools = new Dictionary<GameObject, Pool>();
        private static Dictionary<Type, Pool> _perComponentPools = new Dictionary<Type, Pool>();

        #region Types
        
        private static class PerComponentPool<T> where T : Component
        {
            public static Pool ObjectPool;
            private static string DefaultName = "[GameObject]";

            public static T Get()
                => Get(DefaultName);
            public static T Get(string name)
            {
                return GetOrCreatePool().Get(name).GetComponent<T>();
            }

            public static void Release(T component)
            {
                GetOrCreatePool().Release(component.gameObject);
            }

            public static void PreAlloc(int count)
            {
                GetOrCreatePool().PreAlloc(count);
            }

            private static Pool GetOrCreatePool()
            {
                if(ObjectPool is null)
                    CreatePool();
                return ObjectPool;
            }

            private static void CreatePool()
            {
                var container = new GameObject("[Pool]" + typeof(T).Name);
                container.transform.SetParent(GameObjectPool.Instance.transform);
                ObjectPool = new Pool(container, Allocator);
                _perComponentPools.Add(typeof(T), ObjectPool);
            }

            private static GameObject Allocator()
            {
                var obj = new GameObject(DefaultName);
                obj.AddComponent<T>();
                return obj;
            }
        }

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
        
        #endregion
        
        #region PrefabPool

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
        
        #endregion

        #region PerComponentPool

        public static T Get<T>(string name) where T : Component
            => PerComponentPool<T>.Get(name);

        public static T Get<T>() where T : Component
            => PerComponentPool<T>.Get();

        public static void Release<T>(T component) where T : Component
            => PerComponentPool<T>.Release(component);

        public static void PreAlloc<T>(int count) where T : Component
            => PerComponentPool<T>.PreAlloc(count);

        #endregion

        protected override void Awake()
        {
            base.Awake();
            gameObject.ClearChildren();
        }
    }
}