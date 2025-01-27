using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance = null;
    [HideInInspector] public InventoryItem[] inventory = new InventoryItem[24];
    [HideInInspector] public InventoryItem[] equippedItems = new InventoryItem[6];

    [SerializeField] private GameObject[] inventorySlots;
    //Used for storing instantiated items
    [SerializeField] private GameObject storedItemsParent;

    [SerializeField] private GraphicRaycaster uiRaycaster;
    [SerializeField] private GameObject[] equipmentSlots; //0-2: armor slots, 3: weapon slot, 4-5: skill slots
    [SerializeField] private Sprite[] equipmentIcons; //0: armor, 1: weapon, 2: skill
    [SerializeField] private Image skill1HUDIcon, skill2HUDIcon;

    private bool draggingItem = false;
    private PointerEventData clickData = new PointerEventData(EventSystem.current);
    private List<RaycastResult> raycastResults = new List<RaycastResult>();
    private Image draggedImg = null;
    private int selectedInventoryIndex = -1;
    private int selectedEquipmentIndex = -1; //If one of these > 0, that means it is being selected

    // Start is called before the first frame update
    void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameObject.SetActive(false);
        UpdateInventoryDisplay();
    }

    private void Update()
    {
        if (draggingItem && draggedImg)
        {
            draggedImg.transform.position = Mouse.current.position.ReadValue();
        }
    }

    public void UpdateInventoryDisplay()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            Image img = inventorySlots[i].GetComponentsInChildren<Image>()[1];

            if (inventory[i] != null)
            {
                img.sprite = inventory[i].icon;
                img.color = Color.black;
            }
            else
            {
                img.color= Color.white;
            }
        }

        for (int i = 0; i < equippedItems.Length; i++)
        {
            Image img = equipmentSlots[i].GetComponentsInChildren<Image>()[1];

            if (equippedItems[i] != null)
            {
                img.sprite = equippedItems[i].icon;
                img.color = Color.black;

                switch (i)
                {
                    case 4:
                        skill1HUDIcon.sprite = equippedItems[i].icon;
                        skill1HUDIcon.color = Color.black;
                        break;
                    case 5:
                        skill2HUDIcon.sprite= equippedItems[i].icon;
                        skill2HUDIcon.color = Color.black;
                        break;
                }
            }
            else
            {
                switch (i)
                {
                    case 0:
                    case 1:
                    case 2:
                        img.sprite = equipmentIcons[0];
                        break;
                    case 3:
                        img.sprite = equipmentIcons[1];
                        break;
                    case 4:
                    case 5:
                        img.sprite = equipmentIcons[2];
                        break;
                }

                img.color= Color.white;
            }
        }
    }

    public void PickUpItem(InventoryItem item)
    {
        //Add item to inventory and update UI
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventory[i] == null)
            {
                item.inventoryIndex = i;
                inventory[i] = item;
                Image img = inventorySlots[i].GetComponentsInChildren<Image>()[1];
                img.sprite = inventory[i].icon;
                img.color = Color.black;

                //Disable the original object and add to stored objects
                item.transform.parent = storedItemsParent.transform;
                item.gameObject.SetActive(false);

                break;
            }
        }
    }

    public void DeleteItem(InventoryItem item)
    {
        //Delete an item from inventory
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventory[i] == item)
            {
                if (i != inventorySlots.Length - 1)
                {
                    //Move other inventory items up one slot
                    for (int k = i; k < inventorySlots.Length; k++)
                    {
                        Image currImg = inventorySlots[k].GetComponentsInChildren<Image>()[1];

                        if (k != inventorySlots.Length - 1)
                        {
                            Image nextImg = inventorySlots[k + 1].GetComponentsInChildren<Image>()[1];
                            inventory[k] = inventory[k + 1];
                            currImg.sprite = nextImg.sprite;
                            currImg.color = Color.black;
                        }
                        else
                        {
                            inventory[k] = null;
                            currImg.sprite = null;
                            currImg.color = Color.white;
                        }
                    }
                }
                else
                {
                    inventory[i] = null;

                    Image img = inventorySlots[i].GetComponentsInChildren<Image>()[1];
                    img.sprite = null;
                    img.color = Color.white;
                }

                Destroy(item);

                break;
            }
        }
    }

    public void DragItem()
    {
        clickData.position = Mouse.current.position.ReadValue();
        raycastResults.Clear();
        uiRaycaster.Raycast(clickData, raycastResults);

        //Drag image
        if (!draggedImg)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                //Check first 2 indices because the raycast may hit an unwanted object
                if ((inventorySlots[i] == raycastResults[0].gameObject) || (inventorySlots[i] == raycastResults[1].gameObject))
                {
                    selectedInventoryIndex = i;
                    break;
                }
            }

            //If not dragging from inventory, check if dragging from equipment
            if (selectedInventoryIndex < 0)
            {
                for (int i = 0; i < equipmentSlots.Length; i++)
                {
                    if ((equipmentSlots[i] == raycastResults[0].gameObject) || (equipmentSlots[i] == raycastResults[1].gameObject))
                    {
                        selectedEquipmentIndex = i;
                        break;
                    }
                }
            }
        }

        //Check if first selected slot is empty
        if (!draggedImg && (((selectedInventoryIndex >= 0) && !inventory[selectedInventoryIndex]) ||
                           ((selectedEquipmentIndex >= 0) && !equippedItems[selectedEquipmentIndex])))
        {
            selectedInventoryIndex = -1;
            selectedEquipmentIndex = -1;
            return;
        }

        draggingItem = !draggingItem;

        //When start
        if (draggingItem && !draggedImg)
        {
            draggedImg = raycastResults[0].gameObject.GetComponent<Image>();
            draggedImg.raycastTarget = false;
            draggedImg.transform.SetParent(gameObject.transform, false);
        }
        //When stop
        else if (draggedImg)
        {
            int newIndex = -1;

            //Check if dragged to inventory
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if ((inventorySlots[i] == raycastResults[0].gameObject) || (inventorySlots[i] == raycastResults[1].gameObject))
                {
                    newIndex = i;
                    break;
                }
            }

            //Update inventory if dragged to inventory
            if (newIndex >= 0)
            {
                //Swap dragged item
                if ((selectedInventoryIndex != newIndex) && inventory[newIndex])
                {
                    Debug.Log("Switch item");
                    InventoryItem tempItem;
                    tempItem = inventory[newIndex];

                    //If equipped
                    if ((selectedEquipmentIndex >= 0) && (tempItem.equipmentType != InventoryItem.EquipmentType.None))
                    {
                        equippedItems[newIndex].GetComponent<IEquippable>().OnUnequip();
                        inventory[newIndex] = equippedItems[selectedEquipmentIndex];
                        equippedItems[selectedEquipmentIndex] = tempItem;
                        
                    }
                    else if (selectedInventoryIndex >= 0)
                    {
                        inventory[newIndex] = inventory[selectedInventoryIndex];
                        inventory[selectedInventoryIndex] = tempItem;
                    }

                    draggingItem = true;
                }
                //Item dragged to new slot
                else if (selectedInventoryIndex != newIndex)
                {
                    Debug.Log("Switch slot");

                    if ((selectedEquipmentIndex >= 0) && equippedItems[selectedEquipmentIndex])
                    {
                        equippedItems[selectedEquipmentIndex].GetComponent<IEquippable>().OnUnequip();
                        inventory[newIndex] = equippedItems[selectedEquipmentIndex];
                        equippedItems[selectedEquipmentIndex] = null;
                    }
                    else if (selectedInventoryIndex >= 0)
                    {
                        inventory[newIndex] = inventory[selectedInventoryIndex];
                        inventory[selectedInventoryIndex] = null;
                    }

                    draggedImg.sprite = null;
                    draggedImg.transform.localPosition = Vector3.zero;
                }
                //Item dragged to same slot
                else
                {
                    Debug.Log("Same slot");
                    draggedImg.transform.localPosition = Vector3.zero;
                }
            }
            //If not dragged to inventory and is equipment
            else if (((selectedInventoryIndex >= 0) && (inventory[selectedInventoryIndex].equipmentType != InventoryItem.EquipmentType.None)) 
                     || (selectedEquipmentIndex >= 0))
            {
                //Check if dragged to equipment
                for (int i = 0; i < equipmentSlots.Length; i++)
                {
                    if ((equipmentSlots[i] == raycastResults[0].gameObject) || (equipmentSlots[i] == raycastResults[1].gameObject))
                    {
                        newIndex = i;
                        break;
                    }
                }

                //Update equipment if dragged to equipment
                switch (newIndex)
                {
                    case 0:
                    case 1:
                    case 2: //Armor Slots
                        //Check if dragged item cannot be equipped in this slot
                        if (((selectedInventoryIndex >= 0) && (inventory[selectedInventoryIndex].equipmentType != InventoryItem.EquipmentType.Armor)) ||
                            ((selectedEquipmentIndex >= 0) && (equippedItems[selectedEquipmentIndex].equipmentType != InventoryItem.EquipmentType.Armor)))
                            
                        {
                            break;
                        }

                        //Swap equipped items
                        if ((equippedItems[newIndex] != null) && (selectedEquipmentIndex != newIndex))
                        {
                            Debug.Log("Switch weapon");
                            InventoryItem tempItem;
                            tempItem = equippedItems[newIndex];

                            if (selectedInventoryIndex >= 0)
                            {
                                equippedItems[newIndex] = inventory[selectedInventoryIndex];
                                inventory[selectedInventoryIndex] = tempItem;

                                equippedItems[newIndex].GetComponent<IEquippable>().OnEquip();
                                inventory[selectedInventoryIndex].GetComponent<IEquippable>().OnUnequip();
                            }
                            else if (selectedEquipmentIndex >= 0)
                            {
                                equippedItems[newIndex] = equippedItems[selectedEquipmentIndex];
                                equippedItems[selectedEquipmentIndex] = tempItem;
                            }
                            
                            draggingItem = true;
                        }
                        //Equipped in empty slot
                        if (equippedItems[newIndex] == null)
                        {
                            Debug.Log("Switch armor slot");

                            if (selectedInventoryIndex >= 0)
                            {
                                equippedItems[newIndex] = inventory[selectedInventoryIndex];
                                inventory[selectedInventoryIndex] = null;
                                equippedItems[newIndex].GetComponent<IEquippable>().OnEquip();
                            }
                            else if (selectedEquipmentIndex >= 0)
                            {
                                equippedItems[newIndex] = equippedItems[selectedEquipmentIndex];
                                equippedItems[selectedEquipmentIndex] = null;
                            }

                            draggedImg.sprite = null;
                            draggedImg.transform.localPosition = Vector3.zero;
                        }
                        //Same slot
                        else if (selectedEquipmentIndex == newIndex)
                        {
                            Debug.Log("Same armor slot");
                            draggedImg.transform.localPosition = Vector3.zero;
                        }

                        break;
                    case 3: //Weapon Slot
                        //Check if dragged item cannot be equipped in this slot
                        if (((selectedInventoryIndex >= 0) && (inventory[selectedInventoryIndex].equipmentType != InventoryItem.EquipmentType.Weapon)) ||
                            ((selectedEquipmentIndex >= 0) && (equippedItems[selectedEquipmentIndex].equipmentType != InventoryItem.EquipmentType.Weapon)))

                        {
                            break;
                        }

                        //Swap equipped items
                        if ((equippedItems[newIndex] != null) && (selectedEquipmentIndex != newIndex))
                        {
                            Debug.Log("Switch weapon");
                            InventoryItem tempItem;
                            tempItem = equippedItems[newIndex];
                            equippedItems[newIndex] = inventory[selectedInventoryIndex];
                            inventory[selectedInventoryIndex] = tempItem;

                            draggingItem = true;

                            equippedItems[newIndex].GetComponent<IEquippable>().OnEquip();
                            inventory[selectedInventoryIndex].GetComponent<IEquippable>().OnUnequip();
                        }
                        //Equipped in empty slot
                        else if (equippedItems[newIndex] == null)
                        {
                            Debug.Log("Switch weapon slot");
                            equippedItems[newIndex] = inventory[selectedInventoryIndex];
                            inventory[selectedInventoryIndex] = null;

                            draggedImg.sprite = null;
                            draggedImg.transform.localPosition = Vector3.zero;

                            equippedItems[newIndex].GetComponent<IEquippable>().OnEquip();
                        }
                        //Same slot
                        else if (selectedEquipmentIndex == newIndex)
                        {
                            Debug.Log("Same weapon slot");
                            draggedImg.transform.localPosition = Vector3.zero;
                        }

                        break;
                    case 4:
                    case 5: //Skill Slots
                        //Check if dragged item cannot be equipped in this slot
                        if (((selectedInventoryIndex >= 0) && (inventory[selectedInventoryIndex].equipmentType != InventoryItem.EquipmentType.Skill)) ||
                            ((selectedEquipmentIndex >= 0) && (equippedItems[selectedEquipmentIndex].equipmentType != InventoryItem.EquipmentType.Skill)))

                        {
                            break;
                        }

                        //Swap equipped items
                        if ((equippedItems[newIndex] != null) && (selectedEquipmentIndex != newIndex))
                        {
                            Debug.Log("Switch skill");
                            InventoryItem tempItem;
                            tempItem = equippedItems[newIndex];

                            if (selectedInventoryIndex >= 0)
                            {
                                equippedItems[newIndex] = inventory[selectedInventoryIndex];
                                inventory[selectedInventoryIndex] = tempItem;

                                equippedItems[newIndex].GetComponent<IEquippable>().OnEquip();
                                inventory[selectedInventoryIndex].GetComponent<IEquippable>().OnUnequip();
                            }
                            else if (selectedEquipmentIndex >= 0)
                            {
                                equippedItems[newIndex] = equippedItems[selectedEquipmentIndex];
                                equippedItems[selectedEquipmentIndex] = tempItem;
                            }

                            draggingItem = true;
                        }
                        //Equipped in empty slot
                        if (equippedItems[newIndex] == null)
                        {
                            Debug.Log("Switch skill slot");
                            
                            if (selectedInventoryIndex >= 0)
                            {
                                equippedItems[newIndex] = inventory[selectedInventoryIndex];
                                inventory[selectedInventoryIndex] = null;
                                equippedItems[newIndex].GetComponent<IEquippable>().OnEquip();
                            }
                            else if (selectedEquipmentIndex >= 0)
                            {
                                equippedItems[newIndex] = equippedItems[selectedEquipmentIndex];
                                equippedItems[selectedEquipmentIndex] = null;
                            }

                            draggedImg.sprite = null;
                            draggedImg.transform.localPosition = Vector3.zero;
                        }
                        //Same slot
                        else if (selectedEquipmentIndex == newIndex)
                        {
                            Debug.Log("Same skill slot");
                            draggedImg.transform.localPosition = Vector3.zero;
                        }

                        break;
                }
            }

            //Reparent image
            if (selectedEquipmentIndex >= 0)
            {
                draggedImg.transform.SetParent(equipmentSlots[selectedEquipmentIndex].transform, false);
                draggedImg.transform.localPosition = Vector3.zero;
            }
            else if (selectedInventoryIndex >= 0)
            {
                draggedImg.transform.SetParent(inventorySlots[selectedInventoryIndex].transform, false);
                draggedImg.transform.localPosition = Vector3.zero;
            }

            UpdateInventoryDisplay();

            //Check if still dragging an item
            if (!draggingItem)
            {
                draggedImg.raycastTarget = true;
                draggedImg = null;
                selectedInventoryIndex = -1;
                selectedEquipmentIndex = -1;
            }
            else
            {
                draggedImg.transform.SetParent(gameObject.transform, false);
            }
        }
    }
}
