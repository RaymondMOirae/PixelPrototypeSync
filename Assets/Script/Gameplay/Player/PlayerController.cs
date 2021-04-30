using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prototype.Element;
using UnityEngine;
using UnityEngine.InputSystem;
using Prototype.Input;
using Prototype.Gameplay.Enemy;
using Prototype.Gameplay.Player.Attack;
using Prototype.Gameplay.RoomFacility;
using Prototype.Gameplay.UI;
using Prototype.Inventory;
using Prototype.Script.Test;

namespace Prototype.Gameplay.Player
{
    public enum AttackType {L, M, R, Rotate, NA};

    public class PlayerController : AttackableBase
    {
        [SerializeField] private bool _canMove;
        [SerializeField] private bool _isDashing;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _dashSpeed;
        [SerializeField] private Vector2 _curDir = Vector2.right;

        // gizmo's display parameters
        [SerializeField] private float _attackRadius;
        [SerializeField] private float _midOuterAngle;
        [SerializeField] private float _sideOuterAngle;

        // layers and filters
        [SerializeField] private LayerMask _attackLayer;
        [SerializeField] private LayerMask _interactLayer;
        private ContactFilter2D _interactFilter;


        // components
        private Rigidbody2D _rigidbody;
        private Collider2D _collider;
        private SpriteRenderer _sprite;

        private WeaponController _wController;
        private InGamePixelEditor _pixelEditor;

        public float AttackRadius => _attackRadius;
        public float MidOuterAngle => _midOuterAngle;
        public float SideOuterAngle => _sideOuterAngle;
        public bool CanMove { get { return _canMove; } set { _canMove = value; } }
        public Vector2 CurDir => _curDir;
        public Vector2 MoveSpeed => _curDir * _moveSpeed;
        public Vector2 DashSpeed => _wController.transform.right * _dashSpeed;
        public WeaponController Weapon => _wController;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _sprite = GetComponent<SpriteRenderer>();
            _pixelEditor = GameObject.Find("InGamePixelEditor").GetComponent<InGamePixelEditor>();
            _wController = transform.Find("WeaponHolder").GetComponent<WeaponController>();

            _interactFilter = new ContactFilter2D();
            _interactFilter.SetLayerMask(_interactLayer);
            _interactFilter.useLayerMask = true;
            _interactFilter.useTriggers = true;
            
            GetComponent<ItemPicker>().OnItemPicked.AddListener(GetComponent<PlayerPackage>().OnItemPicked);
        }

        private void Start()
        {
            // Take weapon from package and use for main weapon
            var package = GetComponent<PlayerPackage>();
            var item = package.Inventory.ItemGroups
                .FirstOrDefault(group => group.ItemType is PixelWeaponType)
                ?.GetOne<PixelWeapon>();
            _wController.CurrentWeapon = item;
        
            InitInputs();
            InitHealthBar();
        }

        private void Update()
        {
            // For test switching weapons
            var package = GetComponent<PlayerPackage>();
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                _wController.CurrentWeapon = package.Inventory.ItemGroups
                    .Where(group => group.ItemType is PixelWeaponType)
                    .Skip(0)
                    .FirstOrDefault()
                    ?.GetOne<PixelWeapon>();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
            {
                _wController.CurrentWeapon = package.Inventory.ItemGroups
                    .Where(group => group.ItemType is PixelWeaponType)
                    .Skip(1)
                    .FirstOrDefault()
                    ?.GetOne<PixelWeapon>();
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
            {
                _wController.CurrentWeapon = package.Inventory.ItemGroups
                    .Where(group => group.ItemType is PixelWeaponType)
                    .Skip(2)
                    .FirstOrDefault()
                    ?.GetOne<PixelWeapon>();
            }
        }

        private void FixedUpdate()
        {
            var inputDir = InputManager.Inputs.Player.Move.ReadValue<Vector2>();
            if ((Mathf.Abs(inputDir.x) > 0.0001f || Mathf.Abs(inputDir.y) > 0.0001f) && 
                !_wController.DuringAttack)
            {
                _curDir = inputDir;
                _sprite.flipX = CurDir.x <= 0;
                _wController.PointAt(CurDir);
            }
            else if(_wController.CurrentType == AttackType.Rotate)
            {
                _curDir = inputDir;
                _sprite.flipX = CurDir.x <= 0;
            }
            else if (_wController.CurrentType == AttackType.M)
                _curDir = inputDir;
            else
                _curDir = Vector2.zero;
            
            if (!_wController.DuringAttack || _wController.CurrentType == AttackType.Rotate)
            {
                _isDashing = false;
                _rigidbody.velocity = MoveSpeed;
            }
            else if(_isDashing)
            {
                _rigidbody.velocity = DashSpeed;
            }
            else
            {
                _rigidbody.velocity = Vector2.zero;
            }
        }

        public void HandleDirectionInput(Vector2 dir)
        {
            dir = dir.normalized;

            if ((Mathf.Abs(dir.x) > 0.0001f || Mathf.Abs(dir.y) > 0.0001f) && 
                      !_wController.DuringAttack)
            {
                _curDir = dir;
                _sprite.flipX = CurDir.x <= 0;
                _wController.PointAt(CurDir);
            }
            else if(_wController.CurrentType == AttackType.Rotate)
            {
                _curDir = dir;
                _sprite.flipX = CurDir.x <= 0;
            }
            else if (_wController.CurrentType == AttackType.M)
            {
                _curDir = dir;
            }
            else
            {
                _curDir = Vector2.zero;
            }
        }

        private void LaunchAttack(AttackType type)
        {
            _wController.Attack(type);
            if (_wController.CurrentType == AttackType.M) _isDashing = true;
        }

        public void TakeDamage(float d)
        {
            base.TakeDamage("Enemy", d);
            Debug.Log("Player Received" + d + "Damage");
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

        private void InitInputs()
        {
            InputManager.Inputs.Player.AttackL.performed += (cxt) => LaunchAttack(AttackType.L);
            InputManager.Inputs.Player.AttackM.performed += (cxt) => LaunchAttack(AttackType.M);
            InputManager.Inputs.Player.AttackR.performed += (cxt) => LaunchAttack(AttackType.R);
            InputManager.Inputs.Player.AttackRotate.performed += (cxt) => LaunchAttack(AttackType.Rotate);
            InputManager.Inputs.Player.Move.started      += (cxt) => HandleDirectionInput(cxt.ReadValue<Vector2>());
            InputManager.Inputs.Player.Move.performed    += (cxt) => HandleDirectionInput(cxt.ReadValue<Vector2>());
            InputManager.Inputs.Player.Move.canceled     += (cxt) => HandleDirectionInput(cxt.ReadValue<Vector2>());
            //InputManager.Inputs.Player.InteractionButton.performed += (cxt) => HandleInteraction();
            InputManager.Inputs.Player.Editor.performed += (cxt) => LaunchEditor();
        }

        private void LaunchEditor()
        {
            _pixelEditor.ShowEditor();
        }

        protected override void FindHealthBar()
        {
            healthBar = GameObject.Find("PlayerHealthBar").GetComponent<HealthBar>();
        }
    }
}
