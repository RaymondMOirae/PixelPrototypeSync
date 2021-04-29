using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prototype.Animation;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class AttackState : StateBase
    {
        private Coroutine _coroutine;

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

        public override void CheckTransition()
        {
            EnemySensorResult res = _enemy.SensorResult;
            if (res.InViewField == false)
                OnExitState(StateType.Idle);
            else if (res.InAttackField == false)
                OnExitState(StateType.Chase);
        }

        public override void OnExitState(StateType nextState)
        {
            // _enemy.StopCoroutine(_coroutine);
            _enemy.CurStateType = nextState;
            _enemy.SwitchState(nextState);
        }

        async Task Attack()
        {
            await _enemy.AnimationController.WaitOnAnimationEvent("DamageFrame");
            _enemy.CastAttack();
            await _enemy.AnimationController.WaitAnimationExit();
            // while (true)
            // {
            //     // _enemy.CastAttack();
            //     yield return new WaitForSeconds(_enemy.AttackInterval);
            // }
        }

        IEnumerator AsyncEnableAttack()
        {
            yield return new WaitForSeconds(_enemy.AttackInterval);
            _enemy.CanAttack = true;
        }
    }
}
