using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.Enemy;
using Prototype.Gameplay.UI;

namespace Prototype.Gameplay.Player.Attack
{
    [RequireComponent(typeof(Animator))]
    public class WeaponController : MonoBehaviour
    {

        [SerializeField] private bool _isAttacking = false;
        [SerializeField] private float _force;
        private AttackType _attackType = AttackType.NA;

        private Animator _animator;
        private List<int> _checkList = new List<int>();
        private TouchInputs _touchInputs;

        public bool DuringAttack { get { return _isAttacking; } }
        public float BeatForce { get { return _force; } }
        public List<int> CheckList { get { return _checkList; } }
        public AttackType CurrentType { get { return _attackType; } }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _touchInputs = GameObject.Find("TouchInputs").GetComponent<TouchInputs>();
        }

        private void FixedUpdate()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                _isAttacking = false;
                _attackType = AttackType.NA;
                CheckList.Clear();
            }
        }

        public void PointAt(Vector3 dir)
        {
            transform.right = dir;
        }

        public void Attack(AttackType type)
        {
            if(!_isAttacking && _touchInputs.GetCDStatus(type))
            {
                _attackType = type;
                _isAttacking = true;

                _touchInputs.TriggerCoolDownCheck(type);
                _animator.Play("Attack" + type.ToString(), 0, 0);
            }
            else
            {
                _attackType = AttackType.NA;
            }

        }
    }
}

