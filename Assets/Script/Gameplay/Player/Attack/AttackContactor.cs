﻿using System.Collections;
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
            _analyzer = GetComponentInParent<AttackAnalyzer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            QueryNAttack(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            QueryNAttack(collision);
        }

        private void QueryNAttack(Collider2D collision)
        {
            if (_controller.DuringAttack && collision.gameObject.CompareTag("Enemy"))
            {
                AttackableBase ab = collision.gameObject.GetComponent<AttackableBase>();
                if(ab != null && !_controller.CheckList.Contains(ab.GetInstanceID()))
                {
                    ab.TakeDamage(_controller.CurrentType.ToString(), _analyzer.ResolveDamageValue(_controller.CurrentType));
                    _controller.CheckList.Add(ab.GetInstanceID());
                }
            }
        }
    }
}

