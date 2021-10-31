using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class RangedHitRecoverState : HitRecoverState 
    {
        public RangedHitRecoverState(Enemy e) : base(e)
        {
            _player = GameObject.Find("Player");
        }

        public override void CheckTransition()
        {
            if (_canExit)
            {
                EnemySensorResult res = _enemy.SensorResult;
                if (res.InViewField)
                    OnExitState(StateType.Attack);
                else
                    OnExitState(StateType.Idle);
            }
            
        }
    }
    
}

