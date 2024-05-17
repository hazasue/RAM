using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private const int MAX_KEY_COUNT = 4;
    
    private List<KeyCode> keys;
    
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
    
    private void applyKeyInput() {}
}
