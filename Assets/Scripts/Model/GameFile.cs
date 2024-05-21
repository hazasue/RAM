using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameFile
{
    public int id;
    public string name;
    public string bgmName;
    public string charNameLeft;
    public string charNameRight;

    public GameFile(int id, string name, string bgmName, string charNameLeft, string charNameRight)
    {
        this.id = id;
        this.name = name;
        this.bgmName = bgmName;
        this.charNameLeft = charNameLeft;
        this.charNameRight = charNameRight;
    }
}
