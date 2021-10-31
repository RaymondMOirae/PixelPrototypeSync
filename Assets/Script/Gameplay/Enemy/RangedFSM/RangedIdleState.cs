using System.Collections;
using System.Collections.Generic;
using Prototype.Animation;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class RangedIdleState : IdleState
    {
        public RangedIdleState(Enemy e) : base(e) { }

        public override void OnEnterState()
        {
            _enemy.AnimationController.SetAnimationState(AnimationController.StateIdle);
        }

        public override void CheckTransition()
        {
            EnemySensorResult res = _enemy.SensorResult;
            if (res.InViewField)
            {
                OnExitState(StateType.Attack);
            }
        }

        public override void OnExitState(StateType nextState)
        {
            ChangeEnemyState(nextState);
        }
    }
        
}

