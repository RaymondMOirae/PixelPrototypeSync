using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prototype.Animation;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class AttackState : StateBase
    {
        protected Coroutine _coroutine;

        public AttackState(Enemy e) : base(e) { }

        public override void OnEnterState()
        {
            _enemy.StandStill();
            if (_enemy.CanAttack)
            {
                // _coroutine = _enemy.StartCoroutine(Attack());
                Attack();
                _enemy.CanAttack = false;
                _enemy.StartCoroutine(AsyncEnableAttack());
                _enemy.AnimationController.SetAnimationState(AnimationController.StateAttack);
            }

        }

        public override void CheckTransition() { }

        public override void OnExitState(StateType nextState)
        {
            ChangeEnemyState(nextState);
        }

        protected virtual async Task Attack()
        {
            await _enemy.AnimationController.WaitOnAnimationEvent("DamageFrame");
            _enemy.StandStill();
            _enemy.CastAttack();
            await _enemy.AnimationController.WaitAnimationExit();
            EnemySensorResult res = _enemy.SensorResult;
            if (res.InViewField == true)
                OnExitState(StateType.Chase);
            else
                OnExitState(StateType.Idle);
        }

        protected IEnumerator AsyncEnableAttack()
        {
            yield return new WaitForSeconds(_enemy.AttackInterval * 0.5f);
            _enemy.CanAttack = true;
        }
    }
}
