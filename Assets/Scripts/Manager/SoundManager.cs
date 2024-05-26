using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    
    public AudioSource audioSource;

    private AudioClip bgm;
    private List<AudioClip> sfxs;
    
    // Start is called before the first frame update
    void Start()
    {
        init();
    }
    
    public static SoundManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<SoundManager>();
        if (instance == null) Debug.Log("There's no active SoundManager object");
        return instance;
    }

    private void init()
    {
        instance = this;
    }

    public void InitBgm(string path)
    {
        bgm = Resources.Load<AudioClip>($"bgms/{path}");
        audioSource.clip = bgm;
    }

    public void PlayBgm(bool _bool)
    {
        if(_bool) audioSource.Play();
        else
        {
            audioSource.Pause();
        }
    }
    
    public void PlaySfx(AudioClip audioClip) {}
}
