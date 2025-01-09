using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMgr : MonoBehaviour
{
    public static PlayerInputMgr instance = null;
    public InputActionAsset inputActions;
    public InputActionReference moveInput, jumpInput, runInput, grapplingInput, attackInput, inventoryInput,
                                skill1Input, skill2Input;

    void Awake()
    {   
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        inputActions.Enable();
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }
}