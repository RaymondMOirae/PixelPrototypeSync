using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Player
{
    public class CameraFollow : MonoBehaviour
    {
        public GameObject CentreObject;
        private Transform _trans;

        private void Awake()
        {
            _trans = CentreObject.transform;
        }

        private void Update()
        {
            transform.position = new Vector3(_trans.position.x, _trans.position.y, transform.position.z);
        }

    }

}

