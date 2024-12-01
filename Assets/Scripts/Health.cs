using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth = 100;
    public float currHealth = 100;

    public void OnDamage(float damage)
    {
        currHealth -= damage;

        if (currHealth <= 0 )
        {
            Destroy(gameObject);
        }
    }
}
