using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour, IDamageable
{
    public GameObject weapon;

    [SerializeField] private EntityStats playerStats;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject weaponPivot;
    [SerializeField] private AnimatorOverrideController animOverrides;

    private bool endAnim = false;
    private int skillUsed = 0;

    private void Awake()
    {
        playerStats.ResetValues();
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerInputMgr.instance.attackInput.action.started += OnAttack;
        PlayerInputMgr.instance.skill1Input.action.started += OnSkill1;
        PlayerInputMgr.instance.skill2Input.action.started += OnSkill2;

        PauseMgr.instance.onTogglePause += TogglePause;
    }

    private void OnDestroy()
    {
        PlayerInputMgr.instance.attackInput.action.started -= OnAttack;
        PlayerInputMgr.instance.skill1Input.action.started -= OnSkill1;
        PlayerInputMgr.instance.skill2Input.action.started -= OnSkill2;

        PauseMgr.instance.onTogglePause -= TogglePause;
    }

    // Update is called once per frame
    void Update()
    {
        //Update skill cooldowns here because the object is disabled when in inventory
        if (Inventory.instance.equippedItems[4])
        {
            Inventory.instance.equippedItems[4].GetComponent<Skill>().cooldownTimer += Time.deltaTime;
        }
        if (Inventory.instance.equippedItems[5])
        {
            Inventory.instance.equippedItems[5].GetComponent<Skill>().cooldownTimer += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (animator.GetCurrentAnimatorStateInfo(2).IsName("Player_Idle") && !animator.IsInTransition(2))
        {
            animator.SetLayerWeight(2, 0);
        }
        else
        {
            animator.SetLayerWeight(2, 1);
        }
    }

    public void OnDamage(float damage)
    {
        playerStats.currHealth -= damage;

        if (playerStats.currHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    #region Input Detection

    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!weapon || !weapon.transform.parent || PauseMgr.instance.gamePaused)
        {
            return;
        }

        animator.SetTrigger("attack");
    }

    private void OnSkill1(InputAction.CallbackContext context)
    {
        if (PauseMgr.instance.gamePaused)
        {
            return;
        }

        InventoryItem[] equipment = Inventory.instance.equippedItems;

        //Check if there's a skill equipped
        if (!equipment[4])
        {
            return;
        }

        Skill skill = equipment[4].GetComponent<Skill>();

        //Check if requirements to use are met
        if (skill.requiredEquipment.Length != 0)
        {
            int matches = 0;
            foreach (IEquippable requirement in skill.requiredEquipment)
            {
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i] && equipment[i].GetComponent(requirement.GetType()))
                    {
                        matches++;
                    }
                    else if (matches == skill.requiredEquipment.Length)
                    {
                        break;
                    }
                }
            }

            if (matches < skill.requiredEquipment.Length)
            {
                return;
            }
        }

        //Check if skill on cooldown
        if (skill.cooldownTimer < skill.cooldown)
        {
            return;
        }

        skill.cooldownTimer = 0;

        AnimationClip skillAnim = skill.anim;

        //Save animation state
        AnimatorStateInfo[] layerInfo = new AnimatorStateInfo[animator.layerCount];
        for (int i = 0; i < animator.layerCount; i++)
        {
            layerInfo[i] = animator.GetCurrentAnimatorStateInfo(i);
        }

        //Change animation
        animOverrides["Skill_1"] = skillAnim;
        animator.runtimeAnimatorController = animOverrides;
        
        // Force an update
        animator.Update(0.0f);

        // Push back state
        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.Play(layerInfo[i].fullPathHash, i, layerInfo[i].normalizedTime);
        }

        animator.SetTrigger("skill1");
        skillUsed = 1;
    }

    private void OnSkill2(InputAction.CallbackContext context)
    {
        if (PauseMgr.instance.gamePaused)
        {
            return;
        }

        InventoryItem[] equipment = Inventory.instance.equippedItems;

        //Check if there's a skill equipped
        if (!equipment[5])
        {
            return;
        }

        Skill skill = equipment[5].GetComponent<Skill>();

        //Check if requirements to use are met
        if (skill.requiredEquipment.Length != 0)
        {
            int matches = 0;
            foreach (IEquippable requirement in skill.requiredEquipment)
            {
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i] && equipment[i].GetComponent(requirement.GetType()))
                    {
                        matches++;
                    }
                    else if (matches == skill.requiredEquipment.Length)
                    {
                        break;
                    }
                }
            }

            if (matches < skill.requiredEquipment.Length)
            {
                return;
            }
        }

        //Check if skill on cooldown
        if (skill.cooldownTimer < skill.cooldown)
        {
            return;
        }

        skill.cooldownTimer = 0;

        AnimationClip skillAnim = skill.anim;

        //Save animation state
        AnimatorStateInfo[] layerInfo = new AnimatorStateInfo[animator.layerCount];
        for (int i = 0; i < animator.layerCount; i++)
        {
            layerInfo[i] = animator.GetCurrentAnimatorStateInfo(i);
        }

        //Change animation
        animOverrides["Skill_2"] = skillAnim;
        animator.runtimeAnimatorController = animOverrides;

        // Force an update
        animator.Update(0.0f);

        // Push back state
        for (int i = 0; i < animator.layerCount; i++)
        {
            animator.Play(layerInfo[i].fullPathHash, i, layerInfo[i].normalizedTime);
        }

        animator.SetTrigger("skill2");
        skillUsed = 2;
    }

    #endregion

    public void AttachWeapon()
    {
        if (!weapon)
        {
            return;
        }

        endAnim = false;
        weapon.transform.SetParent(weaponPivot.transform, false);
    }

    public void DetachWeapon()
    {
        if (!weapon || !weapon.transform.parent)
        {
            return;
        }

        animator.SetTrigger("endAnim");
        endAnim = true;
        weaponPivot.transform.DetachChildren();
        weapon = null;
    }

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

    public void UseSkill()
    {
        switch (skillUsed)
        {
            case 1:
                Inventory.instance.equippedItems[4].GetComponent<Skill>().OnUse();
                break;
            case 2:
                Inventory.instance.equippedItems[5].GetComponent<Skill>().OnUse();
                break;
        }

        skillUsed = 0;
    }

    public void TogglePause()
    {

        enabled = !enabled;
    }
}
