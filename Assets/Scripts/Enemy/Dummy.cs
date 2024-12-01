using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dummy : MonoBehaviour
{
    public Health health;
    public Slider healthbar;

    // Update is called once per frame
    protected virtual void Update()
    {
        healthbar.value = health.currHealth / health.maxHealth;
    }
}
