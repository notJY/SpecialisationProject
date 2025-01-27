using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour
{
    public float damage = 100000;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable obj = collision.gameObject.GetComponentInParent<IDamageable>();

        if (obj != null)
        {
            obj.OnDamage(damage);
        }
    }
}
