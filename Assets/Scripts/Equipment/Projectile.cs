using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10;
    public float speed = 5;
    private float lifetimer = 0;

    private void Update()
    {
        lifetimer += Time.deltaTime;

        if (lifetimer >= 10)
        {
            Destroy(gameObject);
        }

        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
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
