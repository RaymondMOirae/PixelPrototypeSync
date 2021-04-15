using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.UI;
using Prototype.Gameplay.Enemy.FSM;
using Prototype.Gameplay.Player;

namespace Prototype.Gameplay.Enemy
{
    public enum StateType { Idle, Chase, Attack, HitRecover};

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
        private GameObject _player;
        private Dictionary<StateType, StateBase> _states;
        private EnemySensorResult _sensorResult;
        private ContactFilter2D _playerFilter;
        [SerializeField] private LayerMask _playerLayer;

        [Header("行为参数")]
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _chaseSpeed;
        [SerializeField] private float _staggerSpeed;
        [SerializeField] private float _chaseInterval;
        [SerializeField] private float _patrolInterval;
        [SerializeField] private float _attackInterval;
        [SerializeField] private float _staggerTime;
        [SerializeField] private float _hitRecoverTime;
        [SerializeField] private float _damage;
        [SerializeField] private float _guardRadius;

        //[Header("感知范围")]

        //[HideInInspector] public bool UseRangedSensor;
        //[HideInInspector] public bool UseMeleeSnesor;
        //[HideInInspector] public float RangedAttackRadius;
        //[HideInInspector] public float MeleeAttackRadius;

        public StateBase CurState;
        public StateType CurStateType;

        public bool CanAttack { get; set; }
        public float WalkSpeed { get => _walkSpeed; }
        public float ChaseSpeed { get => _chaseSpeed; }
        public float StaggerSpeed { get => _staggerSpeed; }
        public float ChaseInterval { get => _chaseInterval; }
        public float PatrolInterval { get =>_patrolInterval; }
        public float AttackInterval { get => _attackInterval; }
        public float StaggerTime { get => _staggerTime; }
        public float HitRecoverTime { get => _hitRecoverTime; }
        public float GuardRadius { get => _guardRadius; }
        public EnemySensorResult SensorResult { get => _sensorResult; }
        public ContactFilter2D PlayerFilter { get => _playerFilter; }
        public Rigidbody2D Rigidbdy { get; private set; }

        private void Start()
        {
            InitHealthBar();
            InitComponents();
            InitStates();
        }

        private void InitStates()
        {
            _states = new Dictionary<StateType, StateBase>();

            _states.Add(StateType.Idle, new IdleState(this));
            _states.Add(StateType.Chase, new ChaseState(this));
            _states.Add(StateType.Attack, new AttackState(this));
            _states.Add(StateType.HitRecover, new HitRecoverState(this));

            CurState = _states[StateType.Idle];
            CurState.OnEnterState();
            CurStateType = StateType.Idle;

            CanAttack = true;
        }

        private void InitComponents()
        {
            Rigidbdy = GetComponent<Rigidbody2D>();
            _player = GameObject.Find("Player");
            _playerFilter = new ContactFilter2D();
            _playerFilter.SetLayerMask(_playerLayer);
            _playerFilter.useLayerMask = true;
            _playerFilter.useTriggers = true;
            _sensorResult.Reset(false);
        }
        protected override void InitHealthBar()
        {
            base.InitHealthBar();
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

        public void Move(Vector2 dir, float speed)
        {
            dir = dir.normalized;
            Rigidbdy.velocity = dir * speed;
        }

        public void StandStill()
        {
            Rigidbdy.velocity = Vector2.zero;
        }
        public void SetInViewField(bool b)
        {
            _sensorResult.InViewField = b;
            UpdateState();
        }

        public void SetInAttackField(bool b)
        {
            _sensorResult.InAttackField = b;
            UpdateState();
        }

        public void CastAttack()
        {
            BroadcastMessage("AimPlayer", _player.transform.position);
            BroadcastMessage("MeleeAttack", _damage);
        }

        public override void TakeDamage(string type, float damage)
        {
            base.TakeDamage(type, damage);
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
            _states[CurStateType].OnExitState(StateType.HitRecover);
        }
    }
}

