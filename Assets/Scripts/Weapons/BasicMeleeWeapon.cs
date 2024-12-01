using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeWeapon : MonoBehaviour, IEquippable
{
    public float damage = 10;
    public Vector3 equippedPos, equippedRot, equippedScale;

    [SerializeField]private Collider2D col;

    private bool unequipped = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (unequipped)
        {
            transform.Rotate(0, 0, 1, Space.World);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<IDamageable>().OnDamage(damage);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.parent)
        {
            return;
        }

        collision.GetComponentInParent<PlayerCombat>().weapon = gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (transform.parent)
        {
            return;
        }

        collision.GetComponentInParent<PlayerCombat>().weapon = null;
    }

    public void OnEquip()
    {
        unequipped = false;
        transform.SetLocalPositionAndRotation(equippedPos, Quaternion.Euler(equippedRot));
        transform.localScale = equippedScale;
        gameObject.layer = LayerMask.NameToLayer("Player");
        col.enabled = false;
        col.isTrigger = false;
    }

    public void OnUnequip()
    {
        col.enabled = true;
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        unequipped = true;
    }
}
