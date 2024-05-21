using System.Collections;
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
    public TMP_InputField bgmPath;
    public TMP_InputField cnl;
    public TMP_InputField cnr;

    
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
        GameFileManager.GetInstance().CreateGameFile(name.text, bgmPath.text, cnl.text, cnr.text);
        name.text = "";
        bgmPath.text = "";
        cnl.text = "";
        cnr.text = "";
    }

    public void PopScreen()
    {
        if (screens.Count <= 0) return;

        screens.Pop().SetActive(false);
    }
    
}
