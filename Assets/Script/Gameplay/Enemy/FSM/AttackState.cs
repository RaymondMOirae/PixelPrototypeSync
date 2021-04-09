using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class AttackState : StateBase
    {
        private Coroutine _coroutine;

        public AttackState(Enemy e) : base(e)
        {

        }

        public override void OnEnterState()
        {
            _enemy.StandStill();
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
            if (res.InViewField == false)
                OnExitState(StateType.Idle);
            else if (res.InAttackField == false)
                OnExitState(StateType.Chase);
        }

        public override void OnExitState(StateType nextState)
        {
            _enemy.StopCoroutine(_coroutine);
            _enemy.CurSense = nextState;
            _enemy.SwitchState(nextState);
        }

        IEnumerator Attack()
        {
            while (true)
            {
                _enemy.CastAttack();
                yield return new WaitForSeconds(_enemy.AttackInterval);
            }
        }

        IEnumerator AsyncEnableAttack()
        {
            yield return new WaitForSeconds(_enemy.AttackInterval);
            _enemy.CanAttack = true;
        }

    }
}
