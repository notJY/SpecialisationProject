using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Dummy
{
    public enum States
    {
        Idle,
        Chase,
        Attack,
    }
    public States currState;

    public float damage;

    [SerializeField] private AIPath aiPath;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask ignoredByRaycast;
    [SerializeField] private Collider2D attackCollider;

    protected override void Update()
    {
        base.Update();

        if (!IsGrounded())
        {
            aiPath.canMove = false;
            currState = States.Idle;
            return;
        }

        switch (currState)
        {
            case States.Idle:
                Idle();
                break;
            case States.Chase:
                Chase();
                break;
            case States.Attack:
                Attack();
                break;
        }

    }

    private void LateUpdate()
    {
        if (aiPath.canMove)
        {
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }
    }

    private void Idle()
    {
        if (Vector3.Magnitude(aiPath.destination - transform.position) < 20)
        {
            aiPath.canMove = true;
            currState = States.Chase;
        }
    }

    private void Chase()
    {
        //Flip zombie in direction of player
        if (Mathf.Sign(transform.position.x - aiPath.destination.x) != Mathf.Sign(transform.localScale.x))
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, 1);
        }

        if (aiPath.reachedDestination)
        {
            aiPath.canMove = false;
            currState = States.Attack;
            animator.SetTrigger("attack");
        }
    }

    private void Attack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            aiPath.canMove = true;
            currState = States.Chase;
        }
    }

    public bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector2.down * 2.1f, Color.red);
        return Physics2D.Raycast(transform.position, Vector2.down, 2.1f, ~ignoredByRaycast);
    }

    //Activated by animation event
    public void EnableHit()
    {
        attackCollider.gameObject.SetActive(true);
    }

    public void DisableHit()
    {
        attackCollider.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponentInParent<IDamageable>();

        if (!attackCollider || (target == null))
        {
            return;
        }

        target.OnDamage(damage);
    }
}