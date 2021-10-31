using System.Collections;
using System.Collections.Generic;
using Prototype.Animation;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class HitRecoverState : StateBase
    {
        protected bool _canExit;
        protected Vector2 _staggerDir;
        protected GameObject _player;

        public HitRecoverState(Enemy e): base(e)
        {
            _player = GameObject.Find("Player");
        }
        public override void CheckTransition()
        {
            if (_canExit)
            {
                EnemySensorResult res = _enemy.SensorResult;
                if (res.InAttackField == true)
                    OnExitState(StateType.Attack);
                else if (res.InViewField == true)
                    OnExitState(StateType.Chase);
                else
                    OnExitState(StateType.Idle);
            }
        }

        public override void OnEnterState()
        {
            _staggerDir = MathUtility.ToVector2(_enemy.transform.position - _player.transform.position);
            _enemy.Move(_staggerDir, _enemy.StaggerSpeed);
            _enemy.StartCoroutine(EndHitRecover());
            
            _enemy.AnimationController.SetDirection(-_staggerDir);
            _enemy.AnimationController.SetAnimationState(AnimationController.StateHit);
        }

        public override void OnExitState(StateType nextState)
        {
            _canExit = false;
            ChangeEnemyState(nextState);
        }

        IEnumerator EndHitRecover()
        {
            yield return new WaitForSeconds(_enemy.StaggerTime);
            _enemy.StandStill();
            yield return new WaitForSeconds(_enemy.HitRecoverTime);
            _canExit = true;
            CheckTransition();
        }
    }
}


