using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameFileInstance : MonoBehaviour
{
    public TMP_Text id;
    public TMP_Text name;
    public TMP_Text description;

    public void Init(int id, string name, string description)
    {
        this.id.text = $"{id}";
        this.name.text = $"{name}";
        this.description.text = $"{description}";
    }
}
