using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameFileInstance : MonoBehaviour
{
    private int key;
    
    public TMP_Text id;
    public TMP_Text name;

    public void Init(int id, string name)
    {
        this.key = id;
        this.id.text = $"{id}";
        this.name.text = $"{name}";
    }

    public void SelectFile() { GameFileManager.currentFileKey = key; }
}
