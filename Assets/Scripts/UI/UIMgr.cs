using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UIMgr : MonoBehaviour
{
    public EntityStats playerStats;
    public Slider playerHealthbar, skill1Indicator, skill2Indicator;
    public TMP_Text healthTxt, dialogueTxt;
    public Inventory inventory;

    [Tooltip("UI elements that need to swap color depending on background")]
    public GameObject[] uiElements;

    public GameObject dialogueUI;
    public Dialogue dialogueObj;

    [Header("For boss")]
    [SerializeField] private GameObject bossIndicator, bossArrow;
    [SerializeField] private Camera mainCamera;

    private Coroutine runningDialogueCoroutine = null;
    private bool skipDialogue, showBossPos, bossPosCoroutineRunning;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInputMgr.instance.inventoryInput.action.started += ToggleInventory;

        dialogueUI.SetActive(false);
        bossIndicator.SetActive(false);
    }

    private void OnDestroy()
    {
        PlayerInputMgr.instance.inventoryInput.action.started -= ToggleInventory;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealthbar)
        {   
            playerHealthbar.value = playerStats.currHealth/playerStats.maxHealth;
            healthTxt.text = playerStats.currHealth + "/" + playerStats.maxHealth;
        }

        if (Inventory.instance.equippedItems[4] && skill1Indicator)
        {
            Skill skill1 = Inventory.instance.equippedItems[4].GetComponent<Skill>();
            skill1Indicator.value = skill1.cooldownTimer / skill1.cooldown;
        }
        if (Inventory.instance.equippedItems[5] && skill2Indicator)
        {
            Skill skill2 = Inventory.instance.equippedItems[5].GetComponent<Skill>();
            skill2Indicator.value = skill2.cooldownTimer / skill2.cooldown;
        }

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
        if (!inventory || !PauseMgr.instance)
        {
            return;
        }

        inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
        PauseMgr.instance.gamePaused = inventory.gameObject.activeSelf;
        PauseMgr.instance.onTogglePause.Invoke();
    }

    public void PlayDialogue(string key)
    {
        if (!dialogueUI)
        {
            return;
        }

        if (runningDialogueCoroutine != null)
        {
            StopCoroutine(runningDialogueCoroutine);
        }

        dialogueUI.SetActive(true);
        runningDialogueCoroutine = StartCoroutine(AnimateDialogue(key));
    }

    public IEnumerator AnimateDialogue(string key)
    {
        if (!dialogueObj)
        {
            yield break;
        }

        //Show text
        dialogueTxt.text = "";
        foreach (char chara in dialogueObj.GetValue(key))
        {
            if (skipDialogue)
            {
                skipDialogue = false;
                dialogueTxt.text = dialogueObj.GetValue(key);
                break;
            }

            dialogueTxt.text += chara;
            yield return new WaitForSeconds(0.05f);
        }

        //Wait for 3 seconds or click before disabling UI
        float Timer = 0f;
        while (true)
        {
            Timer += Time.deltaTime;

            if ((Timer >= 20) || skipDialogue)
            {
                skipDialogue = false;
                break;
            }

            yield return null;
        }
        
        dialogueUI.SetActive(false);
    }

    public void SkipDialogue()
    {
        skipDialogue = true;
    }

    public void ShowBossPos(Vector2 pos)
    {
        if (!bossIndicator)
        {
            return;
        }

        if (!bossPosCoroutineRunning)
        {
            showBossPos = true;
            StartCoroutine(FlashBossIndicator());
        }

        Vector2 bossScreenPos = mainCamera.WorldToScreenPoint(pos);
        bossIndicator.transform.position = bossScreenPos + new Vector2(0, 150);

        if ((bossScreenPos.y >= 0) && (bossScreenPos.y <= Screen.height) && (bossScreenPos.x <= Screen.width) && (bossScreenPos.x >= 0))
        {
            bossArrow.SetActive(false);
            return;
        }

        bossArrow.SetActive(true);

        //Calculate the angle to rotate bossArrow
        Vector2 dir = (bossScreenPos - new Vector2(Screen.width/2, Screen.height/2)).normalized;
        float angle = Mathf.Atan2(dir.x, -dir.y) * Mathf.Rad2Deg;
        
        bossArrow.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        
        if ((bossScreenPos.x > 0) && (bossScreenPos.y <= Screen.height - 130) && (bossScreenPos.y >= 130))
        {
            bossIndicator.transform.position = new Vector3(Screen.width - 130, bossScreenPos.y, 0);
        }
        else if ((bossScreenPos.x < 0) && (bossScreenPos.y <= Screen.height - 130) && (bossScreenPos.y >= 130))
        {
            bossIndicator.transform.position = new Vector3(130, bossScreenPos.y, 0);
        }
        else if ((bossScreenPos.y < 0) && (bossScreenPos.x <= Screen.width) && (bossScreenPos.x >= 0))
        {
            bossIndicator.transform.position = new Vector3(bossScreenPos.x, 130, 0);
        }
        else if ((bossScreenPos.y > 0) && (bossScreenPos.x <= Screen.width) && (bossScreenPos.x >= 0))
        {
            bossIndicator.transform.position = new Vector3(bossScreenPos.x, Screen.height - 130, 0);
        }
    }

    public IEnumerator FlashBossIndicator()
    {
        bossPosCoroutineRunning = true;
        
        while (showBossPos && bossIndicator)
        {
            bossIndicator.SetActive(true);
            yield return new WaitForSeconds(0.2f);
            bossIndicator.SetActive(false);
            yield return new WaitForSeconds(0.2f);
        }

        bossPosCoroutineRunning = false;
    }

    public void HideBossPos()
    {
        if (!bossIndicator)
        {
            return;
        }

        showBossPos = false;
        bossIndicator.SetActive(false);
    }
}
