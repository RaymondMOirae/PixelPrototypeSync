using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.Player;
using Prototype.Gameplay.Enemy;

namespace Prototype.Gameplay.Player.Attack
{
    public class AttackContactor : MonoBehaviour
    {
        private WeaponController _controller;
        private AttackAnalyzer _analyzer;

        void Start()
        {
            _controller = transform.parent.transform.parent.GetComponent<WeaponController>();
            _analyzer = transform.parent.Find("Analyzer").GetComponent<AttackAnalyzer>();
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (_controller.isAttacking && collision.collider.CompareTag("Enemy"))
            {
                EnemyBase eb = collision.collider.GetComponent<EnemyBase>();
                if(eb != null)
                {
                    eb.TakeDamage(_controller.CurrentType.ToString(), _analyzer.ResolveDamageValue(_controller.CurrentType));
                }
                
            }

        }
    }

}

