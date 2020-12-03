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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_controller.DuringAttack && collision.gameObject.CompareTag("Enemy"))
            {
                EnemyBase eb = collision.gameObject.GetComponent<EnemyBase>();
                if(eb != null)
                {
                    eb.TakeDamage(_controller.CurrentType.ToString(), _analyzer.ResolveDamageValue(_controller.CurrentType));
                }
                
            }
            
        }

    }

}

