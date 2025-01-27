using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string dialogueKey;

    [SerializeField] private UIMgr uiMgr;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        uiMgr.PlayDialogue(dialogueKey);
        Destroy(gameObject);
    }
}
