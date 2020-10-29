using JetBrains.Annotations;
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
    private Vector2 _midOuterBoundR;
    private Vector2 _midOuterBoundL;

    public float SideOuterAngle;
    private Vector2 _sideOuterBoundR;
    private Vector2 _sideOuterBoundL;

    public LayerMask attackLayer;

    public Vector2 curDir = Vector2.right;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _turningThresholdCos = Mathf.Cos(turningThresholdAngle);

        //_midOuterBoundR = MathUtility.Rotate(curDir, MidOuterAngle * _toRadianFactor);
        //_midOuterBoundL = MathUtility.Rotate(curDir, -MidOuterAngle * _toRadianFactor);

        //_sideOuterBoundR = MathUtility.Rotate(curDir, SideOuterAngle * _toRadianFactor);
        //_sideOuterBoundL = MathUtility.Rotate(curDir, -SideOuterAngle * _toRadianFactor);

    }

    private void Update()
    {
        DrawGizmos();
    }

    private void DrawGizmos()
    {
        Debug.DrawRay(transform.position, curDir * attackRadius, Color.cyan);
        Debug.DrawRay(transform.position, MathUtility.Rotate(curDir, SideOuterAngle * _toRadianFactor)* attackRadius, Color.yellow);
        Debug.DrawRay(transform.position, MathUtility.Rotate(curDir, -SideOuterAngle * _toRadianFactor)* attackRadius, Color.yellow);
        Debug.DrawRay(transform.position, MathUtility.Rotate(curDir, MidOuterAngle * _toRadianFactor)* attackRadius, Color.red);
        Debug.DrawRay(transform.position, MathUtility.Rotate(curDir, -MidOuterAngle * _toRadianFactor)* attackRadius, Color.red);
    }

    public void MoveInput(InputAction.CallbackContext cxt)
    {
        Vector2 inputDir = cxt.ReadValue<Vector2>();
        Move(inputDir);
        Turn(inputDir);
    }

    public void AttackInputL(InputAction.CallbackContext cxt)
    {
        CastDamageSector(-SideOuterAngle, -MidOuterAngle, Attack.L);
    }
    public void AttackInputM(InputAction.CallbackContext cxt)
    {
        CastDamageSector(-MidOuterAngle, MidOuterAngle, Attack.M);
    }

    public void AttackInputR(InputAction.CallbackContext cxt)
    {
        CastDamageSector(MidOuterAngle, SideOuterAngle, Attack.R);
    }

    public void CastDamageSector(float innerAngle,float outerAngle, Attack a)
    {
        RaycastHit2D[] res = Physics2D.CircleCastAll(MathUtility.ToVector2(transform.position), attackRadius, curDir, 0, attackLayer);
        IEnumerable<GameObject> resObj = res.Select(r => r.transform.gameObject);

        foreach(GameObject obj in resObj)
        {
            EnemyBase e = obj.GetComponent<EnemyBase>();
            if(e != null && InSector(e.transform.position - transform.position, innerAngle, outerAngle) )
            {
                e.TakeDamage(a.ToString());
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
        if(Vector2.Dot(dir, curDir) < _turningThresholdCos && dir!=Vector2.zero)
        {
            curDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? 
                     Vector2.right * Mathf.Sign(dir.x) : Vector2.up * Mathf.Sign(dir.y);
        }
    }

}
