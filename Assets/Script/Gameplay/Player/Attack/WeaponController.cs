using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.Gameplay.Enemy;
using Prototype.Gameplay.UI;
using Prototype.Rendering;
using Prototype.Element;

namespace Prototype.Gameplay.Player.Attack
{
    [RequireComponent(typeof(Animator))]
    public class WeaponController : MonoBehaviour
    {

        [SerializeField] private bool _isAttacking = false;
        [SerializeField] private float _force;
        private AttackType _attackType = AttackType.NA;

        private Animator _animator;
        private List<int> _checkList = new List<int>();
        private TouchInputs _touchInputs; 
        
        private WeaponDisplay _wDisplayUI;
        private SpriteRenderer _wSpriteRenderer;

        private PixelImageRenderer _wPixelRenderer;
        //private AttackAnalyzer _wAnalyser;

        public bool DuringAttack { get { return _isAttacking; } }
        public float BeatForce { get { return _force; } }
        public List<int> CheckList { get { return _checkList; } }
        public AttackType CurrentType { get { return _attackType; } }
        public PixelImage WeaponImage { get { return _wPixelRenderer.Image; } }
        public PixelImageRenderer WeaponRenderer { get { return _wPixelRenderer; } }

        private void Awake()
        {
            _wDisplayUI = GameObject.Find("WeaponDisplay").GetComponent<WeaponDisplay>();
            GameObject analyser = GameObject.Find("Analyzer");
            _wSpriteRenderer = analyser.GetComponent<SpriteRenderer>();
            _wPixelRenderer = analyser.GetComponent<PixelImageRenderer>();
            //_wAnalyser = analyser.GetComponent<AttackAnalyzer>();

            _animator = GetComponent<Animator>();
            _touchInputs = GameObject.Find("TouchInputs").GetComponent<TouchInputs>();
        }

        private void Start()
        {
            //_wPixelRenderer.SetPixelImage(_wPixelRenderer.Image);
            _wDisplayUI.SetWeaponDisplay(_wSpriteRenderer.sprite);
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
                _animator.Play("Attack" + type.ToString(), 0, 0);
            }
            else
            {
                _attackType = AttackType.NA;
            }

        }

        public void UpdateWeapon() 
        {
            WeaponImage.UpdateTexture();
            _wPixelRenderer.SetPixelImage(_wPixelRenderer.Image);
            _wDisplayUI.SetWeaponDisplay(_wPixelRenderer.Sprite);
		}

    }
}

