using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using Unity.VisualScripting;

public abstract class Skill : MonoBehaviour, IEquippable
{
    public AnimationClip anim;
    public MonoBehaviour[] requiredEquipment;
    public float cooldown;
    public float cooldownTimer = 100;

    protected bool dropped = true;

    protected virtual void Update()
    {
        if (dropped)
        {
            transform.Rotate(0, 0, 1, Space.World);
        }
    }

    public abstract void OnEquip();

    public abstract void OnUnequip();

    public abstract void OnUse();
}
