using System;
using System.Collections;
using System.Collections.Generic;
using Prototype.Animation;
using UnityEngine;
using Random = UnityEngine.Random;
using Prototype.Gameplay.UI;
using Prototype.Gameplay.Enemy.FSM;
using Prototype.Gameplay.Player;
using Prototype.Element;
using Prototype.Settings;

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
        protected GameObject _player;
        protected Dictionary<StateType, StateBase> _states;
        protected EnemySensorResult _sensorResult;
        protected ContactFilter2D _playerFilter;
        protected Coroutine _stateWaitCoroutine;
        protected Vector2 _curHeadingDir;
        [SerializeField] protected LayerMask _playerLayer;

        [SerializeField] protected AnimationController _animationController;
        [SerializeField] protected int _dropAmount;

        [Header("行为参数")]
        [SerializeField] protected float _walkSpeed;
        [SerializeField] protected float _chaseSpeed;
        [SerializeField] protected float _staggerSpeed;
        [SerializeField] protected float _chaseInterval;
        [SerializeField] protected float _patrolInterval;
        [SerializeField] protected float _attackInterval;
        [SerializeField] protected float _staggerTime;
        [SerializeField] protected float _hitRecoverTime;
        [SerializeField] protected float _damage;
        [SerializeField] protected float _guardRadius;
        
        //[Header("感知范围")]

        //[HideInInspector] public bool UseRangedSensor;
        //[HideInInspector] public bool UseMeleeSnesor;
        //[HideInInspector] public float RangedAttackRadius;
        //[HideInInspector] public float MeleeAttackRadius;

        public StateBase CurState;
        public StateType CurStateType;

        private List<PixelType> PixelTypes => PixelAssets.Current.PixelTypes;
        public bool CanAttack { get; set; }
        public bool CanBeInterrupted { get; set; }
        public float WalkSpeed => _walkSpeed;
        public float ChaseSpeed => _chaseSpeed;
        public float StaggerSpeed => _staggerSpeed;
        public float ChaseInterval => _chaseInterval;
        public float PatrolInterval =>_patrolInterval;
        public float AttackInterval => _attackInterval;
        public float StaggerTime => _staggerTime;
        public float HitRecoverTime => _hitRecoverTime;
        public float GuardRadius => _guardRadius;
        public EnemySensorResult SensorResult => _sensorResult;
        public LayerMask PlayerLayer => _playerLayer;
        public ContactFilter2D PlayerFilter => _playerFilter;
        public Rigidbody2D Rigidbdy { get; private set; }

        public AnimationController AnimationController => _animationController;

        protected void Start()
        {
            InitHealthBar();
            InitComponents();
            InitStates();
        }

        protected void FixedUpdate()
        {
            Rigidbdy.velocity = _curHeadingDir;
        }

        protected virtual void InitStates()
        {
            _states = new Dictionary<StateType, StateBase>
            {
                {StateType.Idle, new IdleState(this)},
                {StateType.Chase, new ChaseState(this)},
                {StateType.Attack, new AttackState(this)},
                {StateType.HitRecover, new HitRecoverState(this)}
            };

            CurState = _states[StateType.Idle];
            CurState.OnEnterState();
            CurStateType = StateType.Idle;

            CanAttack = true;
            CanBeInterrupted = true;
        }

        protected void InitComponents()
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

        public void Move(Vector2 dir, float speed)
        {
            _curHeadingDir = dir.normalized * speed;
        }

        public void StandStill()
        {
            _curHeadingDir = Vector2.zero;
        }

        public void SetInViewField(bool b)
        {
            _sensorResult.InViewField = b;
            CurState.CheckTransition();
        }

        public void SetInAttackField(bool b)
        {
            _sensorResult.InAttackField = b;
            CurState.CheckTransition();
        }

        public virtual void CastAttack()
        {
            BroadcastMessage("AimPlayer", _player.transform.position);
            BroadcastMessage("MeleeAttack", _damage);
        }

        public void ChasePlayer(Vector2 chaseDir)
        {
            BroadcastMessage("AimPlayer", _player.transform.position);
            Move(chaseDir, _chaseSpeed);
        }

        public override void TakeDamage(string type, float damage)
        {
            base.TakeDamage(type, damage);
            if (currentHealth <= 0)
            {
                for(int i = 0; i < _dropAmount; i++) 
		        { 
				    int pixelIndex = (int)(Random.value * PixelTypes.Count);
					Vector2 offset = Random.insideUnitCircle * _spawnRadius;
					var pos =  transform.position + MathUtility.ToVector3(offset);
				    base.DropPixel(PixelTypes[pixelIndex], pos);	
		        }
                Destroy(gameObject);
            }
            if(CanBeInterrupted)
                _states[CurStateType].OnExitState(StateType.HitRecover);
        }
    }
}

