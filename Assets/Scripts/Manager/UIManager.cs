using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    
    public Slider progressSlider;
    public Slider hp;
    public TMP_Text timer;
    public TMP_Text combo;
    public TMP_Text score;
    public TMP_Text currentResult;

    public TMP_Text[] excellentText = new TMP_Text[2];
    public TMP_Text[] goodText = new TMP_Text[2];
    public TMP_Text[] badText = new TMP_Text[2];
    public TMP_Text[] missText = new TMP_Text[2];

    public RawImage leftChar;
    public RawImage rightChar;

    public GameObject clearScreen;
    public GameObject failScreen;
    public GameObject pauseScreen;

    private PatternManager.Progress progress;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
        
        if (progress != PatternManager.Progress.PLAYING) return;
        progressSlider.value += Time.deltaTime;
        if (progressSlider.value >= progressSlider.maxValue) ClearGame();
    }

    public static UIManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<UIManager>();
        if (instance == null) Debug.Log("There's no active UIManager object");
        return instance;
    }

    public void Init(float duration, float maxHp, PatternManager.Progress progress)
    {
        progressSlider.maxValue = duration;
        progressSlider.value = 0f;
        hp.maxValue = maxHp;
        hp.value = maxHp;

        this.progress = progress;
    }
    
    public void UpdateSlider(float pregress, float currentHp) {}

    public void UpdateTimer(float time)
    {
        timer.text = $"{((int)(time / 60)).ToString("D2")} : {((int)(time % 60)).ToString("D2")} : {((int)(time % 1f * 100f)).ToString("D2")}";
    }

    public void UpdateScore(int excellentCount, int goodCount, int badCount, int missCount)
    {
        //score.text = $"{excellentCount.ToString("D3")} : {goodCount.ToString("D3")} : {badCount.ToString("D3")} : {missCount.ToString("D3")}";
        excellentText[0].text = $"Excellent: {excellentCount.ToString("D3")}";
        goodText[0].text = $"Good: {goodCount.ToString("D3")}";
        badText[0].text = $"Bad: {badCount.ToString("D3")}";
        missText[0].text = $"Miss: {missCount.ToString("D3")}";
        excellentText[1].text = $"Excellent: {excellentCount.ToString("D3")}";
        goodText[1].text = $"Good: {goodCount.ToString("D3")}";
        badText[1].text = $"Bad: {badCount.ToString("D3")}";
        missText[1].text = $"Miss: {missCount.ToString("D3")}";
    }

    public void UpdateCombo(int count, string current)
    {
        if (count <= 0) combo.text = "";
        else
        {
            combo.text = $"{count.ToString()} Combo!";
        }
        currentResult.text = current;
    }

    public void ClearGame()
    {
        Time.timeScale = 0f;
        SoundManager.GetInstance().PlayBgm(false);
        clearScreen.SetActive(true);
    }

    public void FailGame()
    {
        Time.timeScale = 0f;
        SoundManager.GetInstance().PlayBgm(false);
        failScreen.SetActive(true);
    }

    public void PauseGame()
    {
        if (pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            SoundManager.GetInstance().PlayBgm(false);
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
            SoundManager.GetInstance().PlayBgm(true);
        }
    }

    public void SelectActionButton(string str)
    {
        Time.timeScale = 1f;
        
        switch (str)
        {
            case "retry":
                SceneManager.LoadScene("Play");
                break;
            
            case "back":
                SceneManager.LoadScene("Lobby");
                break;
            
            default:
                break;
        }
    }

    public void GiveDamage(float value)
    {
        hp.value -= value;
        if (hp.value <= 0f) FailGame();
    }
}
