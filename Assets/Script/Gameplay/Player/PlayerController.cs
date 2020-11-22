using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Prototype.Gameplay.Enemy;

namespace Prototype.Gameplay.Player
{
    public enum Attack {L, M, R };

    public class PlayerController : MonoBehaviour
    {
        public Vector2 moveSpeed = new Vector2();
        [Range(45, 90)]
        public float TurningThresholdAngle;

        private float _turningThresholdCos;

        public float AttackRadius;
        public float MidOuterAngle;
        public float SideOuterAngle;

        public int MidInterpoNum;
        public int SideInterpoNum;

        private float _sideDeltaAngle;
        private float _midDeltaAngle;

        private readonly float _toRadianFactor = 0.0174532925f;

        public LayerMask AttackLayer;

        public Vector2 CurDir = Vector2.right;

        private Rigidbody2D _rigidBody;
        private Weapon _weapon;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _weapon = transform.Find("Weapon").GetComponent<Weapon>();
            _turningThresholdCos = Mathf.Cos(TurningThresholdAngle);
            _sideDeltaAngle = (SideOuterAngle - MidOuterAngle) / SideInterpoNum;
            _midDeltaAngle = MidOuterAngle * 2 / MidInterpoNum;
        }

        private void Update()
        {
            DrawAttackRange();
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

        public void MoveInput(InputAction.CallbackContext cxt)
        {
            Vector2 inputDir = cxt.ReadValue<Vector2>();
            Move(inputDir);
            Turn(inputDir);
        }

        public void AttackInputL(InputAction.CallbackContext cxt)
        {
            if(cxt.started)
                CastDamageSector(-SideOuterAngle, -MidOuterAngle, Attack.L);
        }
        public void AttackInputM(InputAction.CallbackContext cxt)
        {
            if(cxt.started)
                CastDamageSector(-MidOuterAngle, MidOuterAngle, Attack.M);
        }

        public void AttackInputR(InputAction.CallbackContext cxt)
        {
            if(cxt.started)
                CastDamageSector(MidOuterAngle, SideOuterAngle, Attack.R);
        }

        //public void CastDamageSector(float minAngle,float maxAngle, Attack a)
        //{
        //    // 获取范围内的collider，抽取对应的GameObject
        //    Collider2D[] res = Physics2D.OverlapCircleAll(MathUtility.ToVector2(transform.position), attackRadius, attackLayer);

        //    IEnumerable<GameObject> resObj = res.Select(r => r.transform.gameObject);

        //    foreach(GameObject obj in resObj)
        //    {
        //        // 如果是可攻击对象（挂有EnemyBase），则进行攻击判定
        //        EnemyBase e = obj.GetComponent<EnemyBase>();
        //        if(e != null)
        //        {
        //            BoxCollider2D box = e.GetComponent<BoxCollider2D>();
        //            Vector2 hitBoxOffset = MathUtility.ToVector2(e.transform.position - transform.position) + box.offset;
        //            if(InSector(hitBoxOffset, minAngle, maxAngle) )
        //            {

        //                e.TakeDamage(a.ToString(), 5);
        //            }
        //        }
        //    }

        //}

        public void CastDamageSector(float minAngle, float maxAngle, Attack a)
        {
            List<EnemyBase> res = new List<EnemyBase>();
            float deltaAngle = a == Attack.M ? _midDeltaAngle : _sideDeltaAngle;
            for(float i = minAngle; i <= maxAngle; i += deltaAngle)
            {
                RaycastHit2D hit = RaycastWithGizmos(i);
                EnemyBase eb = hit.collider == null ? null : hit.transform.gameObject.GetComponent<EnemyBase>();
                if (eb != null && !res.Contains(eb))
                {
                    res.Add(eb);
                    eb.TakeDamage(a.ToString(), _weapon.ResolveDamage(a));
                }
            }
        }

        public RaycastHit2D RaycastWithGizmos(float relativeAngle)
        {
            Debug.DrawRay(transform.position, MathUtility.Rotate(CurDir, -relativeAngle * _toRadianFactor) *AttackRadius, Color.green, 1.0f);
            return Physics2D.Raycast(transform.position, MathUtility.Rotate(CurDir, -relativeAngle * _toRadianFactor), AttackRadius, AttackLayer);
        }

        public bool InSector(Vector2 offset, float innerAngle, float outerAngle)
        {
            float offsetAngle = Vector2.SignedAngle(offset, CurDir);
            return (offsetAngle > innerAngle && offsetAngle <= outerAngle);
        }

        public void Move(Vector2 dir)
        {
            _rigidBody.velocity = new Vector2(dir.x * moveSpeed.x, dir.y * moveSpeed.y);
        }

        public void Turn(Vector2 dir)
        {
            // 输入向量与当前朝向超过turningThresholdAngle时转向
            // 相当于使用Vector2.SignedAngle()
            if(Vector2.Dot(dir, CurDir) < _turningThresholdCos && dir!=Vector2.zero)
            {
                CurDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? 
                         Vector2.right * Mathf.Sign(dir.x) : Vector2.up * Mathf.Sign(dir.y);
            }
        }

    }

}

