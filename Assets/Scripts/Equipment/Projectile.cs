using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10;
    public float speed = 5;
    [HideInInspector] public float dirSign = 1;
    private float lifeTimer = 0;

    private void Start()
    {
        dirSign = Mathf.Sign(PlayerInputMgr.instance.transform.localScale.x);
        if (dirSign < 0)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = true;
        }
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= 10)
        {
            Destroy(gameObject);
        }

        transform.position += new Vector3(speed * Time.deltaTime, 0, 0) * dirSign;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable target = collision.gameObject.GetComponent<IDamageable>();

        if (target != null)
        {
            target.OnDamage(damage);
        }

        Destroy(gameObject);
    }
}
