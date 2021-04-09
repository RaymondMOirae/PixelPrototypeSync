using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Enemy.FSM
{
    public class StateBase
    {
        protected Enemy _enemy;

        public StateBase(Enemy e)
        {
            _enemy = e;
        }

        public virtual void OnEnterState()
        {
            Debug.Log(_enemy.name + "Enter State");
        }

        public virtual void PerformAction() { }
        public virtual void CheckTransition() { }
        public virtual void OnExitState(StateType nextState) { }
    }
}

