using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWave : Skill
{
    public GameObject projectilePrefab;

    public override void OnEquip()
    {
    }

    public override void OnUnequip()
    {
        
    }

    public override void OnUse()
    {
        //Don't need to check cooldown because already done in PlayerCombat
        GameObject proj = Instantiate(projectilePrefab, PlayerInputMgr.instance.transform.position + new Vector3(1, 0, 0), projectilePrefab.transform.rotation);
    }
}
