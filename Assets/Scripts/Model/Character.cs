using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public int id;
    public string imagePath;

    public RawImage image;
    public Texture idleImage;

    public Dictionary<KeyCode, Texture> animations;

    private float time;

    void Update()
    {
        if (time <= 0f) return;

        time -= Time.deltaTime;
        if (time <= 0f) image.texture = idleImage;
    }

    public void Init(string name)
    {
        image = this.GetComponent<RawImage>();
        imagePath = $"chars/{name}_";
        animations = new Dictionary<KeyCode, Texture>();
        idleImage = Resources.Load<Texture>($"{imagePath}idle");
        image.texture = idleImage;
        time = 0f;

        animations.Add(KeyCode.D, Resources.Load<Texture>($"{imagePath}D"));
        animations.Add(KeyCode.F, Resources.Load<Texture>($"{imagePath}F"));
        animations.Add(KeyCode.J, Resources.Load<Texture>($"{imagePath}J"));
        animations.Add(KeyCode.K, Resources.Load<Texture>($"{imagePath}K"));
    }

    public void PlayAnimation(KeyCode key)
    {
        image.texture = animations[key];
        time = 0.5f;
    }
}
