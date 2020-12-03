using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Player.Attack
{
    [RequireComponent(typeof(Animator))]
    public class WeaponController : MonoBehaviour
    {
        private bool _isAttacking = false;
        private AttackType _attackType = AttackType.NA;
        public AttackType CurrentType
        {
            get { return _attackType; }
        }

        public bool DuringAttack
        {
            get { return _isAttacking; }
        }

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                _isAttacking = false;
            }
        }

        public void Attack(AttackType type)
        {
            UpdateTypeState(type);
            PerformAnimation(type);
        }

        private void UpdateTypeState(AttackType type)
        {
            _attackType = type;
            _isAttacking = true;
        }

        private void PerformAnimation(AttackType type)
        {
            _animator.Play("Attack" + type.ToString());
        }
    }

}

