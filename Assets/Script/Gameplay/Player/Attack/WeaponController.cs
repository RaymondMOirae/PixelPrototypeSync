using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Player.Attack
{
    [RequireComponent(typeof(Animator))]
    public class WeaponController : MonoBehaviour
    {
        public bool isAttacking = false;
        private AttackType _attackType = AttackType.NA;
        public AttackType CurrentType
        {
            get { return _attackType; }
        }

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Attack(AttackType type)
        {
            UpdateTypeInfo(type);
            PerformAnimation(type);
        }

        private void UpdateTypeInfo(AttackType type)
        {
            _attackType = type;
        }

        private void PerformAnimation(AttackType type)
        {
            _animator.Play("Attack" + type.ToString());
        }
    }

}

