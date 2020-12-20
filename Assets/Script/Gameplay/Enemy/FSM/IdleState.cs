﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class IdleState : StateBase
    {
        private float StandingThreshold = 0.02f;
        private Coroutine _coroutine;
        public IdleState(Enemy e) : base(e)
        {

        }

        public override void OnEnterState()
        {

            _coroutine = _enemy.StartCoroutine(Patrol());
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
            _enemy.CurSense = nextState;
            _enemy.SwitchState(nextState);
        }

        IEnumerator Patrol()
        {
            while (true)
            {
                if (_enemy.Rigidbody.velocity.magnitude <= StandingThreshold)
                {
                    _enemy.Move(Random.insideUnitCircle.normalized, _enemy.WalkSpeed);
                    Debug.Log("Walk");

                }
                else
                {
                    _enemy.StandStill();
                    Debug.Log("Stand");
                }
                yield return new WaitForSeconds(_enemy.PatrolInterval);
            }
        }

    }
}