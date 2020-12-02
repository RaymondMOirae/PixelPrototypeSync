using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Door : MonoBehaviour
{
    public LayerMask playerLayer;
    private BoxCollider2D box;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (box.IsTouchingLayers(playerLayer))
        {
            animator.SetBool("PlayerAround", true);
        }
        else
        {
            animator.SetBool("PlayerAround", false);
        }
    }


}
