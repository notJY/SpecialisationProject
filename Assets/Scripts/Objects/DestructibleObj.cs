using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObj : MonoBehaviour, IDamageable
{
    public void OnDamage(float damage)
    {
        Destroy(gameObject);
    }
}
