using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.Enemy;

namespace Prototype.Gameplay.Player.Attack
{
    [RequireComponent(typeof(Animator))]
    public class WeaponController : MonoBehaviour
    {
        private bool _isAttacking = false;
        private AttackType _attackType = AttackType.NA;
        private Animator _animator;
        public List<int> CheckList = new List<int>();
        public float force;

        public AttackType CurrentType { get { return _attackType; } }
        public bool DuringAttack { get { return _isAttacking; } }


        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                _isAttacking = false;
                CheckList.Clear();
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
            _animator.Play("Attack" + type.ToString(), 0, 0);
        }
    }

}

