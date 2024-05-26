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
        initBasicFiles();
        
        instantiateGameFileObjects();

        currentFileKey = -1;
        instance = this;
    }

    private void initBasicFiles()
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

        if (!File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_CURRENT_INFO_NAME + ".json"))
        {
            JsonManager.CreateJsonFile(JsonManager.DEFAULT_CURRENT_INFO_NAME, new CurrentInfo(-1, false, false));
        }

        if (!File.Exists(Application.dataPath + "/Data/" + JsonManager.DEFAULT_NODE_DATA_NAME + ".json"))
        {
            JsonManager.CreateJsonFile(JsonManager.DEFAULT_NODE_DATA_NAME, new List<Node>());
        }
        
        if (!File.Exists(Application.dataPath + "/Resources/bgms/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/bgms/");
        }
        
        if (!File.Exists(Application.dataPath + "/Resources/sfxs/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/sfxs/");
        }
        
        if (!File.Exists(Application.dataPath + "/Resources/chars/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/chars/");
        }
    }

    public void CreateGameFile(string name, string path, string cnl, string cnr, string bg)
    {
        int tempKey = 0;
        foreach (int key in files.Keys)
        {
            if (tempKey < key) tempKey = key;
        }

        files.Add(++tempKey, new GameFile(tempKey, name, path, cnl, cnr, bg));
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_GAMEFILE_DATA_NAME, files);

        currentFileKey = tempKey;
        
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CURRENT_INFO_NAME, new CurrentInfo(currentFileKey, true, false));
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
            tempFile.Init(file.id, file.name, file.bgmName);
        }
    }

    public void LoadGameFile()
    {
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CURRENT_INFO_NAME, new CurrentInfo(currentFileKey, false, false));
    }

    public void DeleteGameFile()
    {
        files.Remove(currentFileKey);

        List<Node> nodes = JsonManager.LoadJsonFile<List<Node>>(JsonManager.DEFAULT_NODE_DATA_NAME);
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            if (nodes[i].file == currentFileKey) nodes.RemoveAt(i);
        }
        
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_GAMEFILE_DATA_NAME, files);
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_NODE_DATA_NAME, nodes);
        
        currentFileKey = -1;
        
        instantiateGameFileObjects();
    }

    public void PlayAutoGameFile()
    {
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_CURRENT_INFO_NAME, new CurrentInfo(currentFileKey, false, true));
    }
}
