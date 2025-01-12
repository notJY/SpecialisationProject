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

    [Tooltip("UI elements that need to swap color depending on background")]
    public GameObject[] uiElements;

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

        foreach (var ui in uiElements)
        {
            //Check if background is same color as sprite and change sprite color if it is
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(ui.transform.position), Vector2.zero, 20);

            //If there's something behind the ui
            if (hit)
            {
                var spriteRenderer = hit.transform.GetComponent<SpriteRenderer>();

                if (!spriteRenderer)
                {
                    continue;
                }

                var image = ui.GetComponent<Image>();

                if (image && (spriteRenderer.color == image.color))
                {
                    if (image.color == Color.black)
                    {
                        image.color = Color.white;
                    }
                    else
                    {
                        image.color = Color.black;
                    }

                    continue;
                }

                var text = ui.GetComponent<TMP_Text>();

                if (text && (spriteRenderer.color == text.color))
                {
                    if (text.color == Color.black)
                    {
                        text.color = Color.white;
                    }
                    else
                    {
                        text.color = Color.black;
                    }
                }
                
            }
            //If nothing
            else if (!hit)
            {
                var image = ui.GetComponent<Image>();

                if (image)
                {
                    image.color = Color.black;
                    continue;
                }

                var text = ui.GetComponent<TMP_Text>();

                if (text)
                {
                    text.color = Color.black;
                }
            }
        }
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
    }
}
