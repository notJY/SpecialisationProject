using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMgr : MonoBehaviour
{
    public static PauseMgr instance = null;
    public bool gamePaused = false;
    public Action onTogglePause;

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
    }
}
