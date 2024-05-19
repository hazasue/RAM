using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public int file;
    public float time;
    public KeyCode key;
    public int targetCharacter;
    public int animationIdx;
    public string sfxPath;

    public Node(int file, float time, KeyCode key, int targetCharacter, int animationIdx, string sfxPath)
    {
        this.file = file;
        this.time = time;
        this.key = key;
        this.targetCharacter = targetCharacter;
        this.animationIdx = animationIdx;
        this.sfxPath = sfxPath;
    }
}
