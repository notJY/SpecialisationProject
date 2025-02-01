using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boss : Dummy
{
    public enum States
    {
        Idle,
        Chase,
        Attack,
        Skill,
    }
    public States currState;

    public float baseDamage;
    public float skillCooldown;
    public UnityEvent<Vector2> onUseSkill;
    public UnityEvent onSkillEnd;

    [SerializeField] private AIPath aiPath;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask ignoredByRaycast;
    [SerializeField] private Collider2D mainCol, attackCol;
    [SerializeField] private GameObject skillProjPrefab;

    private List<ContactPoint2D> contactPoints = new List<ContactPoint2D>();
    private float skillCooldownTimer = 0;

    protected override void Update()
    {
        base.Update();

        skillCooldownTimer += Time.deltaTime;

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
            case States.Skill:
                Skill();
                break;
        }

        if (!IsGrounded())
        {
            aiPath.canMove = false;
            currState = States.Idle;
            return;
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
        //Flip in direction of player
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
        else if (skillCooldownTimer > skillCooldown)
        {
            skillCooldownTimer = 0;
            aiPath.canMove = false;
            currState = States.Skill;
            animator.SetTrigger("skill");
            onUseSkill.Invoke(transform.position);
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

    private void Skill()
    {
        onUseSkill.Invoke(transform.position);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        collision.GetContacts(contactPoints);
    }

    public bool IsGrounded()
    {
        foreach (ContactPoint2D contactPoint in contactPoints)
        {
            //Check if there's a contact point below the player
            if (Mathf.Abs(contactPoint.point.y - mainCol.bounds.min.y) <= 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    //Activated by animation event
    public void EnableHit()
    {
        attackCol.gameObject.SetActive(true);
    }

    public void DisableHit()
    {
        attackCol.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable target = collision.GetComponentInParent<IDamageable>();

        if (!attackCol || (target == null))
        {
            return;
        }

        target.OnDamage(baseDamage);
    }

    public override void TogglePause()
    {
        base.TogglePause();

        aiPath.canMove = enabled;
        animator.enabled = enabled;

    }

    public void UseSkill()
    {
        //Calculate the angle to rotate projectile
        Vector2 dir = aiPath.destination - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        Quaternion rot = Quaternion.AngleAxis(angle - 45, Vector3.forward);

        Instantiate(skillProjPrefab, transform.position + new Vector3(1, 0, 0), rot);
    }

    public void OnSkillEnd()
    {
        aiPath.canMove = true;
        currState = States.Chase;
        onSkillEnd.Invoke();
    }
}
