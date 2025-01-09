using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Armor : MonoBehaviour, IEquippable
{
    protected bool dropped = true;

    protected virtual void Update()
    {
        if (dropped)
        {
            transform.Rotate(0, 0, 1, Space.World);
        }
    }

    public virtual void OnEquip()
    {

    }

    public virtual void OnUnequip()
    {

    }
}