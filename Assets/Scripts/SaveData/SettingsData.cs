using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public int qualityLevel;
    public float volume;
    public float sensibility;

    public SettingsData()
    {
        qualityLevel = 1;
        volume = 1f;
        sensibility = 1f;
    }
}
