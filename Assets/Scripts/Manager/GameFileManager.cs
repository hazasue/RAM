using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameFileManager : MonoBehaviour
{
    private static GameFileManager instance;
    
    private Dictionary<int, GameFile> files;
    public static int currentFileKey;
    
    public Transform gameFileTransform;
    
    // Start is called before the first frame update
    void Awake()
    {
        init();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static GameFileManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<GameFileManager>();
        if (instance == null) Debug.Log("There's no active GameFileManager object");
        return instance;
    }

    private void init()
    {
        if(!File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_GAMEFILE_DATA_NAME + ".json"))
        {
            files = new Dictionary<int, GameFile>();
            JsonManager.CreateJsonFile(JsonManager.DEFAULT_GAMEFILE_DATA_NAME, files);
        }
        else
        {
            files = JsonManager.LoadJsonFile<Dictionary<int, GameFile>>(JsonManager.DEFAULT_GAMEFILE_DATA_NAME);
        }

        instantiateGameFileObjects();

        currentFileKey = -1;
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void CreateGameFile(string name, string description, string path)
    {
        int tempKey = 0;
        foreach (int key in files.Keys)
        {
            if (tempKey < key) tempKey = key;
        }

        files.Add(++tempKey, new GameFile(tempKey, name, description, path));
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_GAMEFILE_DATA_NAME, files);

        currentFileKey = tempKey;

        instantiateGameFileObjects();
    }

    private void instantiateGameFileObjects()
    {
        for (int i = gameFileTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(gameFileTransform.GetChild(i).gameObject);
        }
        
        foreach (GameFile file in files.Values)
        {
            GameFileInstance tempFile =
                Instantiate(Resources.Load<GameFileInstance>("prefabs/GameFile"), gameFileTransform, true);
            tempFile.Init(file.id, file.name, file.description);
        }
    }

    public void LoadGameFile()
    {
        Debug.Log(currentFileKey);
    }
}
