﻿using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Attack {L, M, R };


public class PlayerController : MonoBehaviour
{
    public Vector2 moveSpeed = new Vector2();
    public float turningThresholdAngle;

    private float _turningThresholdCos;

    public float attackRadius;

    private float _toRadianFactor = 0.0174532925f;
    public float MidOuterAngle;

    public float SideOuterAngle;

    public LayerMask attackLayer;

    public Vector2 curDir = Vector2.right;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _turningThresholdCos = Mathf.Cos(turningThresholdAngle);
    }

    private void Update()
    {
        DrawAttackRange();
    }

    private void DrawAttackRange()
    {
        Debug.DrawRay(transform.position, curDir * attackRadius, Color.cyan);
        symmetryAction(SideOuterAngle, 
                        (float a) => Debug.DrawRay(transform.position, 
                                                    MathUtility.Rotate(curDir, a * _toRadianFactor) * attackRadius, 
                                                    Color.yellow)
                        );
        symmetryAction(MidOuterAngle, 
                        (float a) => Debug.DrawRay(transform.position, 
                                                    MathUtility.Rotate(curDir, a * _toRadianFactor) * attackRadius, 
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

    public void CastDamageSector(float innerAngle,float outerAngle, Attack a)
    {
        // 获取范围内的collider，抽取对应的GameObject
        Collider2D[] res = Physics2D.OverlapCircleAll(MathUtility.ToVector2(transform.position), attackRadius, attackLayer);

        IEnumerable<GameObject> resObj = res.Select(r => r.transform.gameObject);

        foreach(GameObject obj in resObj)
        {
            // 如果是可攻击对象（挂有EnemyBase），则进行攻击判定
            EnemyBase e = obj.GetComponent<EnemyBase>();
            if(e != null)
            {
                BoxCollider2D box = e.GetComponent<BoxCollider2D>();
                Vector2 hitBoxOffset = MathUtility.ToVector2(e.transform.position - transform.position) + box.offset;
                if(InSector(hitBoxOffset, innerAngle, outerAngle) )
                {

                    e.TakeDamage(a.ToString(), 5);
                }
            }
        }

    }

    public bool InSector(Vector2 offset, float innerAngle, float outerAngle)
    {
        float offsetAngle = Vector2.SignedAngle(offset, curDir);
        return (offsetAngle > innerAngle && offsetAngle <= outerAngle);
    }

    public void Move(Vector2 dir)
    {
        rb.velocity = new Vector2(dir.x * moveSpeed.x, dir.y * moveSpeed.y);
    }

    public void Turn(Vector2 dir)
    {
        // 输入向量与当前朝向超过turningThresholdAngle时转向
        // 相当于使用Vector2.SignedAngle()
        if(Vector2.Dot(dir, curDir) < _turningThresholdCos && dir!=Vector2.zero)
        {
            curDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? 
                     Vector2.right * Mathf.Sign(dir.x) : Vector2.up * Mathf.Sign(dir.y);
        }
    }

}
