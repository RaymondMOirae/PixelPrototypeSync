using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Utils
{
    public static class ScriptableObjectPool<T> where  T : ScriptableObject
    {
        private static readonly List<T> _pool = new List<T>();
        public static T Get()
        {
            if (_pool.IsEmpty())
                return Allocator();

            var obj = _pool.PopBack();
            return obj;
        }

        public static void Release(T obj)
        {
            _pool.Add(obj);
        }

        private static T Allocator()
        {
            return ScriptableObject.CreateInstance<T>();
        }
    }

    public static class ScriptableObjectPool
    {
        public static T Get<T>() where T : ScriptableObject
            => ScriptableObjectPool<T>.Get();

        public static void Release<T>(T obj) where T : ScriptableObject
            => ScriptableObjectPool<T>.Release(obj);
    }
}