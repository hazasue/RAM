using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameFileManager : MonoBehaviour
{
    private Dictionary<int, GameFile> files;
    private GameFile currentGameFile;
    
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
        createGameFile();

        foreach (GameFile file in files.Values)
        {
            GameFileInstance tempFile =
                Instantiate(Resources.Load<GameFileInstance>("prefabs/GameFile"), gameFileTransform, true);
            tempFile.Init(file.id, file.name, file.description);
        }
    }

    private void createGameFile()
    {
        int tempKey = 0;
        foreach (int key in files.Keys)
        {
            if (tempKey < key) tempKey = key;
        }

        files.Add(++tempKey, new GameFile(tempKey, "", "", ""));
        JsonManager.CreateJsonFile(JsonManager.DEFAULT_GAMEFILE_DATA_NAME, files);
    }

    private void loadGameFile()
    {
        
    }
}
