using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Utils
{
    public class Singleton<T> : UnityEngine.MonoBehaviour where T:Singleton<T>
    {
        [UnityEngine.SerializeField]
        bool dontDestroyOnLoad = true;

        private static T instance = null;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        public Singleton()
        {
            if (!instance)
                instance = this as T;
        }

        protected virtual void Awake()
        {
            if (instance && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
                instance = this as T;
            }
        }
    }
}