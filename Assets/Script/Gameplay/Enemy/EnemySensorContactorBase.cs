using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy
{
    public interface IEnemySensorContactor<T> where T : Collider2D
    {
        void InitContactor();
    }
}

