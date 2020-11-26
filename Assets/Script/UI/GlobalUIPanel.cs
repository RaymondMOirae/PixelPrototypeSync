using System;
using System.Threading.Tasks;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GlobalUIPanel<T> : UIPanel where T : GlobalUIPanel<T>
    {
        public static T Instance;

        protected override void Awake()
        {
            base.Awake();
            Instance = this as T;
        }
    }
}