using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeWeapon : Weapon
{
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable target = collision.gameObject.GetComponent<IDamageable>();

        if (target != null)
        {
            target.OnDamage(damage);
        }
    }
}
