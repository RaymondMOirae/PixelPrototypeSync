using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.Player;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class ChaseState : StateBase
    {
        private PlayerController _player;
        private Coroutine _coroutine;
        public ChaseState(Enemy e): base(e)
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        public override void OnEnterState()
        {
            _coroutine = _enemy.StartCoroutine(Chase()); ;
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
            _enemy.CurSense = nextState;
            _enemy.SwitchState(nextState);
        }

        IEnumerator Chase()
        {
            while (true)
            {
                Vector2 chaseDir = MathUtility.ToVector2(_player.transform.position - _enemy.transform.position).normalized;
                _enemy.Move(chaseDir, _enemy.ChaseSpeed);
                yield return new WaitForSeconds(_enemy.ChaseInterval);
            }
        }


    }
}

