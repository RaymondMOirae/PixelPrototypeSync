using System;
using UnityEngine;

namespace Prototype.Utils
{
    public class ResetZ : MonoBehaviour
    {
        public float Z;

        private void OnEnable()
        {
            transform.position = transform.position.Set(z: Z);
        }
    }
}