using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Prototype.Animation
{
    public enum DirectionRenderType
    {
        FlipX,
        AnimationQuartic,
        AnimationOctal,
    }
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class AnimationController: MonoBehaviour
    {
        public const string StateIdle = "Idle";
        public const string StateWalk = "Walk";
        public const string StateAttack = "Attack";
        public const string StateHit = "Hit";
        public const string StateDead = "Dead";
        
        
        public DirectionRenderType DirectionRenderType = DirectionRenderType.FlipX;
        public Vector2Int Direction { get; private set; }
        public List<Transform> FlipObjects = new List<Transform>();
        
        
        private Animator _animator;
        
        private SpriteRenderer _spriteRenderer;


        private readonly Dictionary<string, TaskCompletionSource<bool>> _taskCompletionSources =
            new Dictionary<string, TaskCompletionSource<bool>>();

        private TaskCompletionSource<bool> _exitCompletionSource;

        public void EmitAnimationEvent(string eventName)
        {
            if (_taskCompletionSources.TryGetValue(eventName, out var completionSource) && completionSource != null)
            {
                completionSource.TrySetResult(true);
            }
        }

        public async Task WaitOnAnimationEvent(string eventName)
        {
            var completionSource = new TaskCompletionSource<bool>();
            _taskCompletionSources[eventName] = completionSource;
            await completionSource.Task;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetDirection(Vector2 dir)
        {
            switch (DirectionRenderType)
            {
                case DirectionRenderType.FlipX:
                    if (dir.x > 0)
                        this.Direction =  Vector2Int.right;
                    else if (dir.x < 0)
                        this.Direction = Vector2Int.left;
                    UpdateObjectFlip();
                    break;
                case DirectionRenderType.AnimationOctal:
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        if (dir.x > 0)
                            this.Direction = Vector2Int.right;
                        else if (dir.x < 0)
                            this.Direction = Vector2Int.left;
                    }
                    else if (Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
                    {
                        if (dir.y > 0)
                            this.Direction = Vector2Int.up;
                        else if (dir.y < 0)
                            this.Direction = Vector2Int.down;
                    }
                    this._animator.SetInteger("dirX", this.Direction.x);
                    this._animator.SetInteger("dirY", this.Direction.y);
                    break;
            }
        }

        public void UpdateObjectFlip()
        {
            _spriteRenderer.flipX = Direction.x < 0;
            foreach (var obj in FlipObjects)
            {
                obj.transform.localScale = obj.transform.localScale.Set(x: Direction.x);
            }
        }
        

        /// <summary>
         /// 
         /// </summary>
         /// <param name="stateName">Possible values are string constants in class AnimationController</param>
        public void SetAnimationState(string stateName)
        {
            this._animator.SetTrigger(stateName);
        }

        public void ResetAnimationState(string stateName)
        {
            this._animator.SetTrigger(stateName);
        }

        public async Task WaitAnimationExit()
        {
            if (_exitCompletionSource != null)
                throw new Exception("Animation await conflict");
            _exitCompletionSource = new TaskCompletionSource<bool>();
            await _exitCompletionSource.Task;
            _exitCompletionSource = null;
        }

        void EmitExit()
        {
            _exitCompletionSource?.SetResult(true);
        }
        
    }
}