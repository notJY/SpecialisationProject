using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    public float maxHealth = 100;
    public float currHealth = 100;

    [SerializeField] private SceneMgr sceneMgr;

    public void OnDamage(float damage)
    {
        currHealth -= damage;

        if (currHealth <= 0 )
        {
            if (GetComponent<Boss>() && sceneMgr)
            {
                sceneMgr.SwitchScene(4);
            }

            Destroy(gameObject, 0.1f);
        }
    }
}
