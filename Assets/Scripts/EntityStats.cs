using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EntityStats : ScriptableObject
{
    public float maxHealth = 100;
    public float currHealth = 100;
    public float speed = 5;
    public float maxSpeed = 10;
    public float jumpHeight = 5;
    public EntityStats defaultValues;

    public void ResetValues()
    {
        maxHealth = defaultValues.maxHealth;
        currHealth = defaultValues.currHealth;
        speed = defaultValues.speed;
        maxSpeed = defaultValues.maxSpeed;
        jumpHeight = defaultValues.jumpHeight;
    }
}
