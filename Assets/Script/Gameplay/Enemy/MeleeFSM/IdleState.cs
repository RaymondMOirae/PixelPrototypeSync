using System.Collections;
using System.Collections.Generic;
using Prototype.Animation;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class IdleState : StateBase
    {
        private float StandingThreshold = 0.02f;
        private Coroutine _coroutine;
        public IdleState(Enemy e) : base(e) { }

        public override void OnEnterState()
        {
            _coroutine = _enemy.StartCoroutine(Patrol());
            _enemy.AnimationController.SetAnimationState(AnimationController.StateIdle);
        }

        public override void CheckTransition()
        {
            EnemySensorResult res = _enemy.SensorResult;
            if (res.InAttackField == true)
                OnExitState(StateType.Attack);
            else if (res.InViewField == true)
                OnExitState(StateType.Chase);
        }

        public override void OnExitState(StateType nextState)
        {
            _enemy.StopCoroutine(_coroutine);
            ChangeEnemyState(nextState);
        }

        IEnumerator Patrol()
        {
            while (true)
            {
                if (_enemy.Rigidbdy.velocity.magnitude <= StandingThreshold)
                {
                    var dir = Random.insideUnitCircle;
                    _enemy.Move(dir, _enemy.WalkSpeed);
                    _enemy.AnimationController.SetDirection(dir);
                    _enemy.AnimationController.SetAnimationState(AnimationController.StateWalk);
                    //Debug.Log("Walk");
                }
                else
                {
                    _enemy.StandStill();
                    
                    _enemy.AnimationController.SetAnimationState(AnimationController.StateIdle);
                    // Debug.Log("Stand");
                }
                yield return new WaitForSeconds(_enemy.PatrolInterval);
            }
        }
    }
}
