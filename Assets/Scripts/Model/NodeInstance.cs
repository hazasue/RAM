using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeInstance : MonoBehaviour
{
    public static float MAGNIFICATION = 8f;
    
    public float time;
    public float duration;
    public KeyCode key;
    private bool isMaking;
    private bool isPressing;
    private float pressedTime;

    void Update()
    {
        this.transform.position += new Vector3(0f, PatternManager.DEFAULT_NODE_SIZE * PatternManager.MAGNIFICATION * Time.deltaTime, 0f);
        
        if (isPressing)
        {
            this.transform.position -= new Vector3(0f, PatternManager.DEFAULT_NODE_SIZE * PatternManager.MAGNIFICATION * Time.deltaTime, 0f);
            this.GetComponent<RectTransform>().sizeDelta -= new Vector2(0f,
                PatternManager.DEFAULT_NODE_SIZE * Time.deltaTime * PatternManager.MAGNIFICATION);
        }
        
        if (isMaking)
        {
            pressedTime += Time.deltaTime;
            this.transform.position -= new Vector3(0f, PatternManager.DEFAULT_NODE_SIZE * PatternManager.MAGNIFICATION * Time.deltaTime, 0f);
            if (pressedTime >= 1f / PatternManager.MAGNIFICATION)
            {
                this.GetComponent<RectTransform>().sizeDelta = new Vector2(PatternManager.DEFAULT_NODE_SIZE,
                    PatternManager.DEFAULT_NODE_SIZE * pressedTime * PatternManager.MAGNIFICATION);
            }
        }
    }

    public void Init(float time, KeyCode key, float xPos, Transform parent)
    {
        this.time = time;
        this.key = key;
        this.transform.SetParent(parent);
        isMaking = false;

        this.transform.localPosition = new Vector3(xPos, -time * PatternManager.DEFAULT_NODE_SIZE * PatternManager.MAGNIFICATION, 0f);
    }

    public void InitDuration(float duration)
    {
        this.duration = duration;
        if(duration <= 1f / PatternManager.MAGNIFICATION && isMaking) this.transform.position += new Vector3(0f, PatternManager.DEFAULT_NODE_SIZE * PatternManager.MAGNIFICATION * duration, 0f);
        isMaking = false;
        if (duration <= 1f / PatternManager.MAGNIFICATION)
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(PatternManager.DEFAULT_NODE_SIZE,
                PatternManager.DEFAULT_NODE_SIZE);
        else
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(PatternManager.DEFAULT_NODE_SIZE,
                PatternManager.DEFAULT_NODE_SIZE * duration * PatternManager.MAGNIFICATION);
        }
    }
    
    public void InitMaking(float time, KeyCode key, float xPos, Transform parent)
    {
        this.time = time;
        this.key = key;
        this.transform.SetParent(parent);
        
        isMaking = true;
        pressedTime = 0f;

        this.transform.localPosition = new Vector3(xPos, 0f, 0f);
    }

    public void PressingNode()
    {
        isPressing = true;
    }

    public void UpdateCurrentSelectedNode()
    {
        PatternManager.GetInstance().UpdateMakingNodeInfo(this);
    }

    public void ToggleTime(float scale)
    {
        time += scale;

        this.transform.position -=
            new Vector3(0f, PatternManager.DEFAULT_NODE_SIZE * PatternManager.MAGNIFICATION * scale, 0f);
        UpdateCurrentSelectedNode();
    }
}
