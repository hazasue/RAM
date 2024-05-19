using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameFile
{
    public int id;
    public string name;
    public string description;
    public string bgmPath;

    public GameFile(int id, string name, string description, string bgmPath)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.bgmPath = bgmPath;
    }
}
