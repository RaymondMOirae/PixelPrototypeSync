using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveSpeed = new Vector2();
    public float turningThresholdAngle;
    private float turningThresholdCos;

    public Vector2 curDir = Vector2.right;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        turningThresholdCos = Mathf.Cos(turningThresholdAngle);
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, curDir, Color.cyan);
    }

    public void HandleInput(InputAction.CallbackContext cxt)
    {
        Vector2 inputDir= cxt.ReadValue<Vector2>();
        Move(inputDir);
        Turn(inputDir);
    }

    public void Move(Vector2 dir)
    {
        rb.velocity = new Vector2(dir.x * moveSpeed.x, dir.y * moveSpeed.y);
    }

    public void Turn(Vector2 dir)
    {
        if(Vector2.Dot(dir, curDir) < turningThresholdCos && dir!=Vector2.zero)
        {
            curDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y) ? Vector2.right * Mathf.Sign(dir.x) : Vector2.up * Mathf.Sign(dir.y);
        }
    }

}
