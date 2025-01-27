using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dummy : MonoBehaviour
{
    public Health health;
    public Slider healthbar;

    [SerializeField] private List<GameObject> drops = new List<GameObject>();

    protected virtual void Start()
    {
        PauseMgr.instance.onTogglePause += TogglePause;
    }

    protected virtual void OnDestroy()
    {
        PauseMgr.instance.onTogglePause -= TogglePause;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        healthbar.value = health.currHealth / health.maxHealth;

        if (health.currHealth <= 0)
        {
            foreach (GameObject item in drops)
            {
                Instantiate(item, transform.position, item.transform.rotation);
                drops.Remove(item);
            }
        }
    }

    public virtual void TogglePause()
    {
        enabled = !enabled;
    }
}
