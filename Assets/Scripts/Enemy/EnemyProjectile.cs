using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    [HideInInspector] public Vector3 dir;

    protected override void Start()
    {
        PauseMgr.instance.onTogglePause += TogglePause;

        dir = (PlayerInputMgr.instance.transform.position - transform.position).normalized;
    }

    protected override void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= 10)
        {
            Destroy(gameObject);
        }

        transform.position += dir * speed * Time.deltaTime;
    }
}
