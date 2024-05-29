using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public int file;
    public float time;
    public float duration;
    public KeyCode key;
    public int targetCharacter;
    public int animationIdx;
    public string sfxPath;

    public Node(int file, float time, float duration, KeyCode key, int targetCharacter, int animationIdx, string sfxPath)
    {
        this.file = file;
        this.time = time;
        this.duration = duration;
        this.key = key;
        this.targetCharacter = targetCharacter;
        this.animationIdx = animationIdx;
        this.sfxPath = sfxPath;
    }
}
