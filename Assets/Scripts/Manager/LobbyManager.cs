using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private Stack<GameObject> screens;
    public GameObject createScreen;

    public TMP_InputField name;
    public TMP_Dropdown bgm;
    public TMP_Dropdown characterLeft;
    public TMP_Dropdown characterRight;
    public TMP_Dropdown backGround;


    
    // Start is called before the first frame update
    void Awake()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) PopScreen();
    }

    private void init()
    {
        screens = new Stack<GameObject>();

        foreach (AudioClip bgmFile in Resources.LoadAll<AudioClip>("bgms"))
        {
            bgm.options.Add(new TMP_Dropdown.OptionData(bgmFile.name));
        }

        foreach (Texture texture in Resources.LoadAll<Texture>("chars/basic"))
        {
            characterLeft.options.Add(new TMP_Dropdown.OptionData(texture.name));
            characterRight.options.Add(new TMP_Dropdown.OptionData(texture.name));
        }

        foreach (Sprite texture in Resources.LoadAll<Sprite>("bg"))
        {
            backGround.options.Add(new TMP_Dropdown.OptionData(texture.name, texture));
        }

        ChangeBG();
    }

    public void ChangeBG()
    {
        backGround.captionImage.sprite = backGround.options[backGround.value].image;
    } 

    public void PushScreen(string name)
    {
        switch (name)
        {
            case "create":
                screens.Push(createScreen);
                createScreen.SetActive(true);
                break;
            
            default:
                break;
        }
    }

    public void LoadScene(string name)
    {
        switch (name)
        {
            case "Create":
                SceneManager.LoadScene("Create");
                break;
            
            case "Play":
                SceneManager.LoadScene("Play");
                break;
            
            default:
                break;
        }
    }

    public void CreateFile()
    {
        GameFileManager.GetInstance().CreateGameFile(name.text, bgm.captionText.text, characterLeft.captionText.text,
            characterRight.captionText.text, backGround.captionText.text);
        name.text = "";
    }

    public void PopScreen()
    {
        if (screens.Count <= 0) return;

        screens.Pop().SetActive(false);
    }
    
}
