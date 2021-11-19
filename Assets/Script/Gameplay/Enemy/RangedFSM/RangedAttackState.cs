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
                Attack();
                _enemy.CanAttack = false;
                _enemy.CanBeInterrupted = true;
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
            _enemy.AnimationController.ResetAnimationState(AnimationController.StateAttack);
            ChangeEnemyState(nextState);
        }
        
        protected override async Task Attack()
        {
            await _enemy.AnimationController.WaitOnAnimationEvent("Uninterruptible");
            _enemy.CanBeInterrupted = false;
            _enemy.CastAttack();
            await _enemy.AnimationController.WaitAnimationExit();
            _enemy.CanBeInterrupted = true;
            EnemySensorResult res = _enemy.SensorResult;
            if(res.InViewField == true)
                OnExitState(StateType.Attack);
            else
                OnExitState(StateType.Idle);
        }

    }
        
}

