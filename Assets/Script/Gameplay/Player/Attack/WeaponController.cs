using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.Enemy;
using Prototype.Gameplay.UI;
using Prototype.Rendering;
using Prototype.Element;
using Script.GameSystem;

namespace Prototype.Gameplay.Player.Attack
{
    [RequireComponent(typeof(Animator), typeof(AttackAnalyzer))]
    public class WeaponController : MonoBehaviour
    {
        // [SerializeField] private PixelImageAsset _testWeaponImageAsset;

        [SerializeField] private bool _isAttacking = false;
        [SerializeField] private float _force;
        [SerializeField] private AudioClip _attackSound;
        private AttackType _attackType = AttackType.NA;

        private Animator _animator;
        private List<int> _checkList = new List<int>();
        private TouchInputs _touchInputs; 
        
        private WeaponDisplay _wDisplayUI;
        private SpriteRenderer _wSpriteRenderer;
      
        private AttackAnalyzer _wAnalyser;

        public bool DuringAttack { get { return _isAttacking; } }
        public float BeatForce { get { return _force; } }
        public List<int> CheckList { get { return _checkList; } }
        public AttackType CurrentType { get { return _attackType; } }

        private PixelWeapon _currentWeapon;

        public PixelWeapon CurrentWeapon
        {
            get => _currentWeapon;
            set
            {
                _currentWeapon = value;
                _wAnalyser.CurrentWeapon = CurrentWeapon;
                _wDisplayUI.SetWeaponDisplay(_wSpriteRenderer.sprite);
            }
        }

        private void Awake()
        {
            _wDisplayUI = GameObject.Find("WeaponDisplay").GetComponent<WeaponDisplay>();
            _wAnalyser = GetComponent<AttackAnalyzer>();
            _wSpriteRenderer = GetComponentInChildren<PixelImageRenderer>().GetComponent<SpriteRenderer>();

            _animator = GetComponent<Animator>();
            _touchInputs = GameObject.Find("TouchInputs").GetComponent<TouchInputs>();
        }

        private void FixedUpdate()
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                _isAttacking = false;
                _attackType = AttackType.NA;
                CheckList.Clear();
            }
        }

        public void PointAt(Vector3 dir)
        {
            transform.right = dir;
        }

        public void Attack(AttackType type)
        {
            if(!_isAttacking && _touchInputs.GetCDStatus(type))
            {
                _attackType = type;
                _isAttacking = true;

                _touchInputs.TriggerCoolDownCheck(type);
                SoundManager.Instance.PlayPlayerSound(_attackSound);
                _animator.Play("Attack" + type.ToString(), 0, 0);
            }
            // else
            // {
            //     _attackType = AttackType.NA;
            // }

        }

        public void UpdateWeapon() 
        {
            CurrentWeapon.UpdateTexture();
		}

    }
}

