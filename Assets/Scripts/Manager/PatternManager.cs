using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager : MonoBehaviour
{
    public enum Progress
    {
        MAKING,
        PLAYING,
    }
    
    public static float time;


    private GameFile file;
    private int combo;
    private float maxHp;
    private float hp;
    private List<Node> patterns;
    private List<Node> currentPatterns;
    private List<Character> characters;

    private Progress progress;
    private SoundManager soundManager;
    private InputManager inputManager;
    private UIManager uIManager;

    // Start is called before the first frame update
    void Awake()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void init() {}
    
    public void HandleKeyInput(List<KeyCode> keys) {}
    
    private void addPattern(KeyCode key) {}
    
    private void inputKey(KeyCode key) {}
}
