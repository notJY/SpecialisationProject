using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, IEquippable
{
    public float damage = 10;
    public Vector3 equippedPos, equippedRot, equippedScale;

    [SerializeField] protected Collider2D col;
    protected bool dropped = true;
    protected PlayerCombat owner;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (dropped)
        {
            transform.Rotate(0, 0, 1, Space.World);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        owner = collision.GetComponentInParent<PlayerCombat>();
    }

    public virtual void OnEquip()
    {
        dropped = false;
        gameObject.SetActive(true);
        transform.SetLocalPositionAndRotation(equippedPos, Quaternion.Euler(equippedRot));
        transform.localScale = equippedScale;
        gameObject.layer = LayerMask.NameToLayer("Player");
        col.enabled = false;
        col.isTrigger = false;

        if (!owner)
        {
            return;
        }

        owner.weapon = gameObject;
        owner.AttachWeapon();
    }

    public virtual void OnUnequip()
    {
        col.enabled = true;
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        dropped = true;
        gameObject.SetActive(false);

        if (!owner)
        {
            return;
        }

        owner.DetachWeapon();
    }
}
