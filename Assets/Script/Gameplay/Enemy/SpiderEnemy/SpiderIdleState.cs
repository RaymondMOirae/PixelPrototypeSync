using System.Collections;
using System.Collections.Generic;
using Prototype.Gameplay.Enemy.FSM;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class SpiderIdleState : IdleState
    {
        public SpiderIdleState(Enemy e) : base(e) { }
        
        public override void CheckTransition()
        {
            EnemySensorResult res = _enemy.SensorResult;
            if(res.InViewField)
                OnExitState(StateType.Attack);
        }
    }
        
}

