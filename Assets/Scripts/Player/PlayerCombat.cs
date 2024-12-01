using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public GameObject weapon;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject weaponPivot;

    private bool endAnim = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInputMgr.instance.attackInput.action.started += OnAttack;

        PlayerInputMgr.instance.equipWeaponInput.action.started += OnEquipWeaponToggle;
    }

    private void OnDestroy()
    {
        PlayerInputMgr.instance.attackInput.action.started -= OnAttack;

        PlayerInputMgr.instance.equipWeaponInput.action.started -= OnEquipWeaponToggle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (!weapon)
        {
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(2).IsName("Idle"))
        {
            animator.SetLayerWeight(2, 0);
        }
        else
        {
            animator.SetLayerWeight(2, 1);
        }
    }

    #region Input Detection

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!weapon || !weapon.transform.parent)
        {
            return;
        }

        animator.SetTrigger("attack");
    }

    private void OnEquipWeaponToggle(InputAction.CallbackContext context)
    {
        if (!weapon)
        {
            return;
        }

        Collider2D weaponCol = weapon.GetComponent<Collider2D>();

        //Detach
        if (weapon.transform.parent)
        {
            animator.SetTrigger("endAnim");
            endAnim = true;
            weapon.GetComponent<IEquippable>().OnUnequip();
            weaponPivot.transform.DetachChildren();
            weapon = null;
        }
        //Attach
        else
        {
            endAnim = false;
            weapon.transform.SetParent(weaponPivot.transform, true);
            weapon.GetComponent<IEquippable>().OnEquip();
        }
    }

    #endregion

    //For animations
    public void EnableWeapon()
    {
        if (!weapon || endAnim)
        {
            return;
        }

        weapon.GetComponent<Collider2D>().enabled = true;
    }

    public void DisableWeapon()
    {
        if (!weapon || endAnim)
        {
            return;
        }

        weapon.GetComponent<Collider2D>().enabled = false;
    }
}
