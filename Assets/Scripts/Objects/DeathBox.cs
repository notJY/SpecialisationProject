using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable obj = collision.gameObject.GetComponentInParent<IDamageable>();

        if (obj != null)
        {
            obj.OnDamage(100000);
        }
    }
}
