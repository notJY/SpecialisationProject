using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEnd : Skill
{
    public GameObject colliderPrefab;

    public override void OnEquip()
    {
        
    }

    public override void OnUnequip()
    {
        
    }

    public override void OnUse()
    {
        //Don't need to check cooldown because already done in PlayerCombat
        Instantiate(colliderPrefab, Camera.main.transform.position, colliderPrefab.transform.rotation);
    }
}
