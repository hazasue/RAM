using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurrentInfo
{
    public int id;
    public bool createMode;
    public bool autoPlay;

    public CurrentInfo(int id, bool createMode, bool autoPlay = false)
    {
        this.id = id;
        this.createMode = createMode;
        this.autoPlay = autoPlay;
    }
}
