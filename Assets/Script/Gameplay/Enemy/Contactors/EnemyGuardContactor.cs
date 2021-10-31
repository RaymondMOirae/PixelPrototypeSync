using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy
{
    public class EnemyGuardContactor : MonoBehaviour, IEnemySensorContactor<CircleCollider2D>
    {
        private Enemy _enemy;
        private CircleCollider2D _collider;

        void Start()
        {
            _enemy = transform.GetComponentInParent<Enemy>();
            _collider = GetComponent<CircleCollider2D>();
            InitContactor();
        }

        public void InitContactor()
        {
            _collider.radius = _enemy.GuardRadius;
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _enemy.SetInViewField(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _enemy.SetInViewField(false);
            }
        }
    }
}

