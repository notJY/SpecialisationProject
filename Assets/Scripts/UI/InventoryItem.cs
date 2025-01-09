using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Marks an object as an inventory item. Always put on item root.
public class InventoryItem : MonoBehaviour
{
    public Sprite icon;
    public int inventoryIndex = -1;
    public enum EquipmentType
    {
        Armor,
        Weapon,
        Skill,
        None
    }
    public EquipmentType equipmentType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.tag != "Player")
        {
            return;
        }

        Inventory.instance.PickUpItem(this);
    }

    private void Start()
    {
        //Check if background is same color as sprite and change sprite color if it is
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero, 20);
        
        //Get the item's renderer
        var spriteRenderer = GetComponent<SpriteRenderer>();

        //If there's something behind object
        if (hits.Length > 1)
        {
            if (hits[1].transform.GetComponent<SpriteRenderer>().color != spriteRenderer.color)
            {
                return;
            }

            var otherRenderer = hits[1].transform.GetComponent<SpriteRenderer>();

            if (otherRenderer.color == Color.black)
            {
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = Color.black;
            }
        }
        //If nothing
        else
        {
            spriteRenderer.color = Color.black;
        }
    }
}
