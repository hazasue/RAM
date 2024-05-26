using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameFileInstance : MonoBehaviour
{
    private int key;
    public TMP_Text name;
    public TMP_Text bgmName;

    public void Init(int id, string name, string bgmName)
    {
        this.key = id;
        this.name.text = $"Name: {name}";
        this.bgmName.text = $"BGM: {bgmName}";
    }

    public void SelectFile() { GameFileManager.currentFileKey = key; }
}
