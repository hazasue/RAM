using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    private AudioClip bgm;
    private List<AudioClip> sfxs;
    
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void init() {}
    
    public void PlaySfx(AudioClip audioClip) {}
}
