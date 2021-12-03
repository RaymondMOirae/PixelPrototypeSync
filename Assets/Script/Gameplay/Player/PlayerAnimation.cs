using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Gameplay.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator _playerAnimator;
        
        public SpriteRenderer PlayerSprite => GetComponent<SpriteRenderer>();
        
        void Start()
        {
            _playerAnimator = GetComponent<Animator>();
        }

        public void SetAnimatorTrigger(string triggerName)
        {
            _playerAnimator.SetTrigger(triggerName);
        }

        public void ResetTrigger(string triggerName)
        {
            _playerAnimator.ResetTrigger(triggerName);
        }
        
        public void PlayWalkAnimation(string animName)
        {
            _playerAnimator.Play("PlayerWalk" + animName);
        }

        public void PlayIdle()
        {
            _playerAnimator.Play("PlayerIdle");
        }
    }
}

