using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    
    public Slider progress;
    public Slider hp;
    public TMP_Text timer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static UIManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<UIManager>();
        if (instance == null) Debug.Log("There's no active UIManager object");
        return instance;
    }
    
    public void Init(float maxHp) {}
    
    public void UpdateSlider(float pregress, float currentHp) {}

    public void UpdateTimer(float time)
    {
        timer.text = $"{((int)(time / 60)).ToString("D2")} : {((int)(time % 60)).ToString("D2")} : {((int)(time % 1f * 100f)).ToString("D2")}";
    }
}
