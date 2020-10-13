using System.Collections.Generic;

namespace Prototype.Utils
{
    public static class ObjectPool<T> where T : new()
    {
        private static List<T> pool = new List<T>(64);

        private static T Allocator()
        {
            return new T();
        }

        public static T Get()
        {
            if (pool.IsEmpty())
                return Allocator();
            else
                return pool.PopBack();
        }

        public static void Release(T obj)
        {
            pool.Add(obj);
        }
        
    }
}