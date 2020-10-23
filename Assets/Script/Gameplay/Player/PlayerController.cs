using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveSpeed = new Vector2();
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(InputAction.CallbackContext cxt)
    {
        Vector2 dir = cxt.ReadValue<Vector2>();
        rb.velocity = new Vector2(dir.x * moveSpeed.x, dir.y * moveSpeed.y);
    }
}
