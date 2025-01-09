using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class UIMgr : MonoBehaviour
{
    public EntityStats playerStats;
    public Slider playerHealthbar;
    public TMP_Text healthTxt;
    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInputMgr.instance.inventoryInput.action.started += ToggleInventory;
    }

    private void OnDestroy()
    {
        PlayerInputMgr.instance.inventoryInput.action.started -= ToggleInventory;
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthbar.value = playerStats.currHealth/playerStats.maxHealth;
        healthTxt.text = playerStats.currHealth + "/" + playerStats.maxHealth;

        //Check if background is same color as sprite and change sprite color if it is
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(playerHealthbar.transform.position),Vector2.zero, 20);
        
        //If there's something behind healthbar
        if (hit)
        {
            var spriteRenderer = hit.transform.GetComponent<SpriteRenderer>();

            if (!spriteRenderer || (hit.transform.GetComponent<SpriteRenderer>().color != playerHealthbar.GetComponentInChildren<Image>().color))
            {
                return;
            }

            var images = playerHealthbar.GetComponentsInChildren<Image>();

            foreach (Image image in images)
            {
                if (image.color == Color.black)
                {
                    image.color = Color.white;
                }
                else
                {
                    image.color = Color.black;
                }
            }
        }
        //If nothing
        else if (!hit)
        {
            var images = playerHealthbar.GetComponentsInChildren<Image>();

            foreach (Image image in images)
            {
                image.color = Color.black;
            }
        }
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
    }
}
