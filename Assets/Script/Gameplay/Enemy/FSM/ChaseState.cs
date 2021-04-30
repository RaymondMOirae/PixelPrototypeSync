using System.Collections;
using System.Collections.Generic;
using Prototype.Animation;
using UnityEngine;
using Prototype.Gameplay.Player;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class ChaseState : StateBase
    {
        private GameObject _player;
        private Coroutine _coroutine;
        public ChaseState(Enemy e): base(e)
        {
            _player = GameObject.Find("Player");
        }

        public override void OnEnterState()
        {
            _coroutine = _enemy.StartCoroutine(Chase());
            _enemy.AnimationController.SetAnimationState(AnimationController.StateWalk);
        }

        public override void CheckTransition()
        {
            EnemySensorResult res = _enemy.SensorResult;
            if (res.InAttackField == true)
                OnExitState(StateType.Attack);
            else if (res.InViewField == false)
                OnExitState(StateType.Idle);
        }

        public override void OnExitState(StateType nextState)
        {
            _enemy.StopCoroutine(_coroutine);
            _enemy.CurStateType = nextState;
            _enemy.SwitchState(nextState);
        }

        IEnumerator Chase()
        {
            while (true)
            {
                Vector2 chaseDir = MathUtility.ToVector2(_player.transform.position - _enemy.transform.position);
                _enemy.ChasePlayer(chaseDir);
                _enemy.AnimationController.SetDirection(chaseDir);
                yield return new WaitForSeconds(_enemy.ChaseInterval);
            }
        }
    }
}

