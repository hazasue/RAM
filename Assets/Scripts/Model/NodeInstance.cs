using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeInstance : MonoBehaviour
{
    public static float MAGNIFICATION = 8f;
    
    public float time;
    public KeyCode key;

    void Update()
    {
        this.transform.position += new Vector3(0f, PatternManager.DEFAULT_NODE_SIZE * PatternManager.MAGNIFICATION * Time.deltaTime, 0f);
    }

    public void Init(float time, KeyCode key, float xPos, Transform parent)
    {
        this.time = time;
        this.key = key;
        this.transform.SetParent(parent);

        this.transform.localPosition = new Vector3(xPos, -time * PatternManager.DEFAULT_NODE_SIZE * PatternManager.MAGNIFICATION, 0f);
    }
    
    public void InitMaking(float time, KeyCode key, float xPos, Transform parent)
    {
        this.time = time;
        this.key = key;
        this.transform.SetParent(parent);

        this.transform.localPosition = new Vector3(xPos, 0f, 0f);
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
    }
}
