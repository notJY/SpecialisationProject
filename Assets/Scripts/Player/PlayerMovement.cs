using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    public float maxSpeed;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float playerHeight;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask ignoredByRaycast;

    private bool moving, canJump, running, canWallJump;
    private float initSpeed;
    private float runningTime = 1;

    private void Start()
    {
        PlayerInputMgr.instance.moveInput.action.started += OnMove;
        PlayerInputMgr.instance.moveInput.action.canceled += OnStop;

        PlayerInputMgr.instance.jumpInput.action.started += OnJump;

        PlayerInputMgr.instance.runInput.action.started += OnRun;
        PlayerInputMgr.instance.runInput.action.canceled += OnStopRun;

        initSpeed = speed;
    }

    private void OnDestroy()
    {
        PlayerInputMgr.instance.moveInput.action.started -= OnMove;
        PlayerInputMgr.instance.moveInput.action.canceled -= OnStop;

        PlayerInputMgr.instance.jumpInput.action.started -= OnJump;

        PlayerInputMgr.instance.runInput.action.started -= OnRun;
        PlayerInputMgr.instance.runInput.action.canceled -= OnStopRun;
    }

    private void Update()
    {
        //Flip player in direction of movement if not against a wall
        if (moving && (Mathf.Sign(rb.velocity.x) != Mathf.Sign(transform.localScale.x)) && 
            !Physics2D.Raycast(transform.position - new Vector3(1, 1.5f, 0), Vector2.right, 2, ~ignoredByRaycast))
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, 1);
        }
    }

    void FixedUpdate()
    {
        //Movement
        if (moving)
        {
            float horizontalMove = PlayerInputMgr.instance.moveInput.action.ReadValue<float>();
            horizontalMove = Mathf.Sign(horizontalMove);
            
            //Dash
            if (runningTime < 0.3f)
            {
                rb.AddForce(new Vector2(1, 0) * horizontalMove * speed/2, ForceMode2D.Impulse);
                //Reset so player doesn't dash infinitely
                runningTime = 1;
            }
            //Run
            else if (running)
            {
                speed = initSpeed * 2;
            }

            rb.AddForce(new Vector2(1, 0) * horizontalMove * speed, ForceMode2D.Force);
        }
        //Stop player if no movement input
        else
        {
            rb.AddForce(rb.velocity * new Vector2(-5, 1), ForceMode2D.Force);
            
            if (!running)
            {
                speed = initSpeed;
            }
        }

        //Jump
        if (canJump)
        {
            rb.AddForce(new Vector2(0, 1) * jumpHeight, ForceMode2D.Impulse);
            canJump = false;
        }
        //Wall jump
        else if (canWallJump)
        {
            rb.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x), 1) * jumpHeight, ForceMode2D.Impulse);
            canWallJump = false;
        }

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
    }

    private void LateUpdate()
    {
        //Update animator
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));

        if (running)
        {
            animator.SetBool("running", true);
        }
        else
        {
            animator.SetBool("running", false);
        }

        if (!IsGrounded())
        {
            animator.SetBool("jumping", true);
            animator.SetFloat("walkSpeedMult", 0);
            animator.SetLayerWeight(1, 1);

            if (Physics2D.Raycast(transform.position - Vector3.right, Vector2.right, 2, ~ignoredByRaycast))
            {
                animator.SetBool("grabbingWall", true);
            }
            else
            {
                animator.SetBool("grabbingWall", false);
            }
        }
        else
        {
            animator.SetBool("jumping", false);
            animator.SetFloat("walkSpeedMult", 1);
            animator.SetLayerWeight(1, 0);
        }
    }

    #region Input Detection

    private void OnMove(InputAction.CallbackContext context)
    {
        moving = true;
    }

    private void OnStop(InputAction.CallbackContext context)
    {
        moving = false;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        //If on ground
        if (IsGrounded())
        {
            canJump = true;
        }
        //If on a wall
        else if (Physics2D.Raycast(transform.position - new Vector3(1, 1.5f, 0), Vector2.right, 2, ~ignoredByRaycast))
        {
            canWallJump = true;
        }
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        running = true;
        runningTime = (float)Math.Round(Time.time, 2, MidpointRounding.AwayFromZero);
    }

    private void OnStopRun(InputAction.CallbackContext context)
    {
        running = false;

        float runEndTime = (float)Math.Round(Time.time, 2, MidpointRounding.AwayFromZero);
        runningTime = runEndTime - runningTime;
    }

    #endregion

    public bool IsGrounded()
    {
        //Debug.DrawRay(transform.position, Vector2.down * (playerHeight / 2 + 0.1f), Color.red);
        return Physics2D.Raycast(transform.position, Vector2.down, playerHeight/2 + 0.1f, ~ignoredByRaycast);
    }
}
