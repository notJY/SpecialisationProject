using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelEnd : MonoBehaviour
{
    public UnityEvent onLevelEnd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onLevelEnd.Invoke();
    }
}
