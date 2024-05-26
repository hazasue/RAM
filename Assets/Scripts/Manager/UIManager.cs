using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    
    public Slider progressSlider;
    public Slider hp;
    public TMP_Text timer;
    public TMP_Text combo;
    public TMP_Text score;
    public TMP_Text currentResult;

    public RawImage leftChar;
    public RawImage rightChar;

    private PatternManager.Progress progress;

    void Update()
    {
        if (progress != PatternManager.Progress.PLAYING) return;
        progressSlider.value += Time.deltaTime;
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
        score.text =
            $"{excellentCount.ToString("D3")} : {goodCount.ToString("D3")} : {badCount.ToString("D3")} : {missCount.ToString("D3")}";
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
}
