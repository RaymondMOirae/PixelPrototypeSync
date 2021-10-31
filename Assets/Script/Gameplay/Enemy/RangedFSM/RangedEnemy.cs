using System.Collections;
using System.Collections.Generic;
using Prototype.Gameplay.Enemy.FSM;
using Prototype.Animation;
using UnityEngine;

namespace Prototype.Gameplay.Enemy
{
    public class RangedEnemy : Enemy
    {
        // Start is called before the first frame update

        protected override void InitStates()
        {
            _states = new Dictionary<StateType, StateBase>
            {
                {StateType.Idle, new RangedIdleState(this)},
                {StateType.Attack, new RangedAttackState(this)},
                {StateType.HitRecover, new RangedHitRecoverState(this)}
            };

            CurState = _states[StateType.Idle];
            CurState.OnEnterState();
            CurStateType = StateType.Idle;

            CanAttack = true;
        }

        public override void CastAttack()
        {
            _animationController.SetDirection(_player.transform.position - transform.position);
            _animationController.SetAnimationState(AnimationController.StateAttack);
        }
    }
        
}

