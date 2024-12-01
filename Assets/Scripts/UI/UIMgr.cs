using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour
{
    public Health playerHealth;
    public Slider playerHealthbar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthbar.value = playerHealth.currHealth/playerHealth.maxHealth;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(playerHealthbar.transform.position),Vector2.zero, 20);
        
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
        else if (!hit)
        {
            var images = playerHealthbar.GetComponentsInChildren<Image>();

            foreach (Image image in images)
            {
                image.color = Color.black;
            }
        }
    }


}
