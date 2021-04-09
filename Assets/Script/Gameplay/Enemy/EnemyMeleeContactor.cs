using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy
{
    public class EnemyMeleeContactor : MonoBehaviour, IEnemySensorContactor<Collider2D>
    { 
        private Enemy _enemy;
        private Collider2D _collider;

        void Start()
        {
            _enemy = transform.GetComponentInParent<Enemy>();
            _collider = GetComponent<Collider2D>();
            InitContactor();
        }

        public void InitContactor()
        {
            _collider.isTrigger = true;
        }

        private void AimPlayer(Vector3 relativePos)
        {
            Vector3 _aim = relativePos - transform.position;
            _aim.z = 0;
            transform.right = _aim;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _enemy.SetInAttackField(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _enemy.SetInAttackField(false);
            }
        }

        private void MeleeAttack(float d)
        {
            List<Collider2D> res = new List<Collider2D>();
            _collider.OverlapCollider(_enemy.PlayerFilter, res);
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

