using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy
{
    public class EnemyMeleeContactor : MonoBehaviour, IEnemySensorContactor<CircleCollider2D>
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
            _collider.radius = _sensor.MeleeRadius;
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _sensor.SetInAttackField(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _sensor.SetInAttackField(false);
            }
        }

        private void MeleeAttack(float d)
        {
            List<Collider2D> res = new List<Collider2D>();
            _collider.OverlapCollider(_sensor.ContactFilter, res);
            foreach(Collider2D c in res)
            {
                if (c.CompareTag("Player"))
                {
                    c.BroadcastMessage("TakeDamage", d);
                    break;
                }
            }
        }


    }

}

