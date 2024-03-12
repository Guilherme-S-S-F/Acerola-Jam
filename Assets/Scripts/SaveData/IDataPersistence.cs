using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence 
{
    void LoadData(SettingsData data);
    void SaveData(ref SettingsData data);
}
