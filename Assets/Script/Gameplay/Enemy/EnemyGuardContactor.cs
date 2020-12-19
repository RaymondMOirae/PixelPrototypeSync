using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy
{
    public class EnemyGuardContactor : MonoBehaviour, IEnemySensorContactor<CircleCollider2D>
    {
        private EnemySensor _sensor;
        private CircleCollider2D _collider;

        void Start()
        {
            _sensor = transform.GetComponentInParent<EnemySensor>();
            _collider = GetComponent<CircleCollider2D>();
            InitContactor();
        }

        public void InitContactor()
        {
            _collider.radius = _sensor.GuardRadius;
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _sensor.SetInViewField(true);
            }
        }
        //private void OnTriggerStay2D(Collider2D collision)
        //{
        //    if (collision.CompareTag("Player"))
        //    {
        //        _sensor.SetInViewField(true);
        //    }
        //}

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _sensor.SetInViewField(false);
            }
        }

    }

}

