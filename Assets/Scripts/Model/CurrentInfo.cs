using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurrentInfo
{
    public int id;
    public bool createMode;

    public CurrentInfo(int id, bool createMode)
    {
        this.id = id;
        this.createMode = createMode;
    }
}
