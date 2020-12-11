using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Prototype.Input;
using Prototype.Gameplay.Enemy;
using Prototype.Gameplay.Player.Attack;
using Prototype.Gameplay.RoomFacility;

namespace Prototype.Gameplay.Player
{
    public enum AttackType {L, M, R, NA};

    public class PlayerController : MonoBehaviour
    {
        // open fields to designer
        public Vector2 moveSpeed = new Vector2();

        [Range(45, 90)] public float TurningThresholdAngle;

        public float AttackRadius;
        public float MidOuterAngle;
        public float SideOuterAngle;

        public int MidInterpoNum;
        public int SideInterpoNum;

        public Vector2 CurDir = Vector2.right;

        // intermediate values 
        private float _turningThresholdCos;
        private float _sideDeltaAngle;
        private float _midDeltaAngle;

        private readonly float _toRadianFactor = 0.0174532925f;

        [SerializeField] private LayerMask _attackLayer;
        [SerializeField] public LayerMask _interactLayer;
        private ContactFilter2D _interactFilter;

        // components
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;
        private WeaponController _wController;
        private SpriteRenderer _sprite;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _wController = transform.Find("WeaponHolder").GetComponent<WeaponController>();

            _interactFilter = new ContactFilter2D();
            _interactFilter.SetLayerMask(_interactLayer);
            _interactFilter.useLayerMask = true;
            _interactFilter.useTriggers = true;

            _turningThresholdCos = Mathf.Cos(TurningThresholdAngle);
            _sideDeltaAngle = (SideOuterAngle - MidOuterAngle) / SideInterpoNum;
            _midDeltaAngle = MidOuterAngle * 2 / MidInterpoNum;
        }

        private void Start()
        {
            InputManager.Inputs.Player.AttackL.performed += (cxt) => _wController.Attack(AttackType.L);
            InputManager.Inputs.Player.AttackM.performed += (cxt) => _wController.Attack(AttackType.M);
            InputManager.Inputs.Player.AttackR.performed += (cxt) => _wController.Attack(AttackType.R);
            InputManager.Inputs.Player.Move.performed += (cxt) => Move(cxt.ReadValue<Vector2>());
            InputManager.Inputs.Player.Move.canceled += (cxt) => Move(cxt.ReadValue<Vector2>());
            InputManager.Inputs.Player.InteractionButton.performed += (cxt) => HandleInteraction();
        }

        void Update()
        {
            DrawAttackRange();
            FlipSprite();
        }

        private void FlipSprite()
        {
            _sprite.flipX = CurDir.x == -1;
        }

        public void Move(Vector2 dir)
        {
            _rigidbody.velocity = new Vector2(dir.x * moveSpeed.x, dir.y * moveSpeed.y);

            // 输入向量与当前朝向超过turningThresholdAngle时转向
            // 相当于使用Vector2.SignedAngle()
            if(Vector2.Dot(dir, CurDir) < _turningThresholdCos && dir!=Vector2.zero)
            {
                Vector2 lastDir = CurDir;
                CurDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? 
                         Vector2.right * Mathf.Sign(dir.x) : Vector2.up * Mathf.Sign(dir.y);
                float delta = Vector2.SignedAngle(lastDir ,CurDir);
                _wController.transform.Rotate(Vector3.forward * delta, Space.Self);
            }
        }

        public void HandleInteraction()
        {
            List<Collider2D> res = new List<Collider2D>();
            _collider.OverlapCollider(_interactFilter, res);
            foreach(Collider2D c in res)
            {
                if (c.CompareTag("InteractiveContactor"))
                {
                    c.transform.parent.GetComponent<InteractiveBase>().HandleInteraction();
                    break;
                }
            }

        }

        private void DrawAttackRange()
        {
            Debug.DrawRay(transform.position, CurDir * AttackRadius, Color.cyan);
            symmetryAction(SideOuterAngle, 
                            (float a) => Debug.DrawRay(transform.position, 
                                                        MathUtility.Rotate(CurDir, a * _toRadianFactor) * AttackRadius, 
                                                        Color.yellow)
                            );
            symmetryAction(MidOuterAngle, 
                            (float a) => Debug.DrawRay(transform.position, 
                                                        MathUtility.Rotate(CurDir, a * _toRadianFactor) * AttackRadius, 
                                                        Color.red)
                            );
        }

        private Action<float, Action<float>> symmetryAction = (angle, f) =>
        {
            f(angle);
            f(-angle);
        };

        #region TrashCode


        public void CastDamageSector(float minAngle, float maxAngle, AttackType a)
        {
            List<EnemyBase> res = new List<EnemyBase>();
            float deltaAngle = a == AttackType.M ? _midDeltaAngle : _sideDeltaAngle;
            for(float i = minAngle; i <= maxAngle; i += deltaAngle)
            {
                RaycastHit2D hit = RaycastWithGizmos(i);
                EnemyBase eb = hit.collider == null ? null : hit.transform.gameObject.GetComponent<EnemyBase>();
                if (eb != null && !res.Contains(eb))
                {
                    res.Add(eb);
                    //eb.TakeDamage(a.ToString(), _weapon.ResolveDamageValue(a));
                }
            }
        }

        public RaycastHit2D RaycastWithGizmos(float relativeAngle)
        {
            Debug.DrawRay(transform.position, MathUtility.Rotate(CurDir, -relativeAngle * _toRadianFactor) *AttackRadius, Color.green, 1.0f);
            return Physics2D.Raycast(transform.position, MathUtility.Rotate(CurDir, -relativeAngle * _toRadianFactor), AttackRadius, _attackLayer);
        }

        public bool InSector(Vector2 offset, float innerAngle, float outerAngle)
        {
            float offsetAngle = Vector2.SignedAngle(offset, CurDir);
            return (offsetAngle > innerAngle && offsetAngle <= outerAngle);
        }

        #endregion

    }

}

