using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prototype.Animation;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class RangedAttackState : AttackState
    {
        public RangedAttackState(Enemy e) : base(e) { }

        public override void OnEnterState()
        {
            if (_enemy.CanAttack)
            {
                _coroutine = _enemy.StartCoroutine(Attack());
                _enemy.CanAttack = false;
                _enemy.StartCoroutine(AsyncEnableAttack());
            }
        }

        public override void CheckTransition()
        {
            EnemySensorResult res = _enemy.SensorResult;
            
            if (!res.InViewField)
            {
                _enemy.AnimationController.SetAnimationState(AnimationController.StateIdle);
                OnExitState(StateType.Idle);
            }
        }

        public override void OnExitState(StateType nextState)
        {
            _enemy.StopCoroutine(_coroutine);
            _enemy.AnimationController.ResetAnimationState(AnimationController.StateAttack);
            ChangeEnemyState(nextState);
        }
        
        private IEnumerator Attack()
        {
            while (true)
            {
                _enemy.CastAttack();
                yield return new WaitForSeconds(_enemy.AttackInterval);
            }
        }

    }
        
}

