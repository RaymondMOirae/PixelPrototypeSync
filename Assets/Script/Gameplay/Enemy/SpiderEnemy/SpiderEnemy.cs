using System.Collections;
using System.Collections.Generic;
using Prototype.Gameplay.Enemy;
using Prototype.Gameplay.Enemy.FSM;
using UnityEngine;

namespace Prototype.Gameplay.Enemy
{
    public class SpiderEnemy : RangedEnemy
    {
        protected override void InitStates()
        {
            _states = new Dictionary<StateType, StateBase>
            {
                {StateType.Idle, new SpiderIdleState(this)},
                {StateType.Attack, new RangedAttackState(this)},
                {StateType.HitRecover, new RangedHitRecoverState(this)}
            };

            CurState = _states[StateType.Idle];
            CurState.OnEnterState();
            CurStateType = StateType.Idle;
            CanAttack = true;
        }
    }
        
}

