using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu]
public class Dialogue : ScriptableObject
{
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();

    public string GetValue(string key)
    {
        int index = keys.IndexOf(key);
        return values[index];
    }
}
