using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName = "settings.game";


    private SettingsData settingsData;

    private List<IDataPersistence> dataPersistencesObjects;

    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Founded More than one persistant data");
        }
        instance = this;
    }
    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistencesObjects = FindAllDataPersistanceObject();
        loadGame();
    }

    public void newGame()
    {
        this.settingsData = new SettingsData();
    } 

    public void loadGame()
    {
        if (this.dataHandler == null)
            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

        this.settingsData = this.dataHandler.Load();

        if (this.settingsData == null)
        {
            Debug.Log("No data founded, getting default settings");
            newGame();
        }
        this.dataPersistencesObjects = FindAllDataPersistanceObject();
        // push loaded Data to all other scripts
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.LoadData(settingsData);
        }

    }

    public void saveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
        {
            dataPersistenceObj.SaveData(ref settingsData);
        }

        dataHandler.Save(settingsData);
    }

    private void OnApplicationQuit()
    {
        saveGame();
    }

    private List<IDataPersistence> FindAllDataPersistanceObject()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
     }
}
