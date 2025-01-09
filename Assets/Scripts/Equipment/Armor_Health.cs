using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor_Health : Armor
{
    [SerializeField] private EntityStats playerStats;

    public override void OnEquip()
    {
        playerStats.maxHealth += 50;
        playerStats.currHealth += 50;
    }

    public override void OnUnequip()
    {
        playerStats.maxHealth -= 50;

        if (playerStats.currHealth > playerStats.maxHealth)
        {
            playerStats.currHealth = playerStats.maxHealth;
        }
    }
}
