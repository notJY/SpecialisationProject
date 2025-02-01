using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10;
    public float speed = 5;
    [HideInInspector] public float dirSign = 1;
    protected float lifeTimer = 0;

    protected virtual void Start()
    {
        PauseMgr.instance.onTogglePause += TogglePause;

        dirSign = Mathf.Sign(PlayerInputMgr.instance.transform.localScale.x);
        if (dirSign < 0)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = true;
        }
    }

    protected virtual void OnDestroy()
    {
        PauseMgr.instance.onTogglePause -= TogglePause;
    }

    protected virtual void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= 10)
        {
            Destroy(gameObject);
        }

        transform.position += new Vector3(speed * Time.deltaTime, 0, 0) * dirSign;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable target = collision.gameObject.GetComponent<IDamageable>();

        if (target != null)
        {
            target.OnDamage(damage);
        }

        Destroy(gameObject);
    }

    protected virtual void TogglePause()
    {
        enabled = !enabled;
    }
}
