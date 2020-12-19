using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.UI;
using Prototype.Gameplay.Enemy.FSM;
using Prototype.Gameplay.Player;

namespace Prototype.Gameplay.Enemy
{
    public enum StateType { Idle, Chase, Attack};

    public struct EnemySensorResult
    {
        public bool InViewField;
        public bool InAttackField;
        public void Reset(bool b)
        {
            InViewField = b;
            InAttackField = b;
        }
    }

    public class Enemy : AttackableBase
    {
        public StateBase CurState;
        public StateType CurSense;

        public EnemySensorResult SensorResult;

        private Dictionary<StateType, StateBase> _states;

        public LayerMask PlayerLayer;
        public ContactFilter2D PlayerFilter;
        [HideInInspector] public Rigidbody2D Rigidbody;

        [Header("行为参数")]
        public float WalkSpeed;
        public float ChaseSpeed;
        public float ChaseInterval;
        public float PatrolInterval;
        public float AttackInterval;
        public float Damage;
        [HideInInspector] public bool canAttack = true;

        #region AttackFields
        [Header("感知范围")]
        [HideInInspector] public bool UseMeleeSnesor;
        [HideInInspector] public float _meleeAttackRadius;
        public float MeleeAttackRadius
        {
            get { return UseMeleeSnesor ? _meleeAttackRadius: 0.0f; }
            set { _meleeAttackRadius = value; }
        }

        [HideInInspector] public bool UseRangedSensor;
        [HideInInspector] public float _rangedAttackRadius;
        public float RangedAttackRadius
        {
            get { return UseRangedSensor ? _rangedAttackRadius : 0.0f; }
            set { _rangedAttackRadius = value; }
        }

        public float GuardRadius;

        #endregion

        private void Start()
        {
            InitHealthBar();
            InitStates();

            PlayerFilter = new ContactFilter2D();
            PlayerFilter.SetLayerMask(PlayerLayer);
            PlayerFilter.useLayerMask = true;
            PlayerFilter.useTriggers = true;
            SensorResult.Reset(false);

        }

        protected override void InitHealthBar()
        {
            base.InitHealthBar();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void InitStates()
        {
            _states = new Dictionary<StateType, StateBase>();

            _states.Add(StateType.Idle, new IdleState(this));
            _states.Add(StateType.Chase, new ChaseState(this));
            _states.Add(StateType.Attack , new AttackState(this));

            CurState = _states[StateType.Idle];
            CurState.OnEnterState();
            CurSense = StateType.Idle;

        }

        public void SwitchState(StateType nextState)
        {
            CurState = _states[nextState];
            CurState.OnEnterState();
            Debug.Log(nextState.ToString());
        }

        public void UpdateState()
        {
            CurState.CheckTransition();
        }

        public void Move(Vector2 direction, float speed)
        {
            Rigidbody.velocity = direction * speed;
        }

        public void StandStill()
        {
            Rigidbody.velocity = Vector2.zero;
        }

        public void CastAttack()
        {
            BroadcastMessage("MeleeAttack", Damage);
        }

    }

}

