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
        bool DontDestroyOnLoad = false;

        private static T _instance = null;
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        public Singleton()
        {
            // if (!_instance)
            //     _instance = this as T;
        }

        protected virtual void Awake()
        {
            if (!_instance)
                _instance = this as T;
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                if (DontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
                _instance = this as T;
            }
        }
    }
}