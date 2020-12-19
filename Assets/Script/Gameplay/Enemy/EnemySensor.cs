using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy
{
    public class EnemySensor: MonoBehaviour
    {
        private Enemy _enemy;

        public bool InView;
        public bool InAttack;

        public float MeleeRadius { get { return _enemy.MeleeAttackRadius; } }
        public float RangedRadius { get { return _enemy.RangedAttackRadius; } }
        public float GuardRadius { get { return _enemy.GuardRadius; } }

        public float DamageValue { get { return _enemy.Damage; } }

        public ContactFilter2D ContactFilter { get { return _enemy.PlayerFilter; } } 

        public void Awake()
        {
            _enemy = transform.GetComponentInParent<Enemy>();
        }

        private void Update()
        {
            InView = _enemy.SensorResult.InViewField;
            InAttack = _enemy.SensorResult.InAttackField;
        }

        public void SetInViewField(bool b)
        {
            _enemy.SensorResult.InViewField = b;
            _enemy.UpdateState();
        }

        public void SetInAttackField(bool b)
        {
            _enemy.SensorResult.InAttackField = b;
            _enemy.UpdateState();
        }

    }

}

