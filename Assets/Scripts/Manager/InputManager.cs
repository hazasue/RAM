using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputManager : MonoBehaviour
{
    private const int MAX_KEY_COUNT = 4;
    private Dictionary<KeyCode, bool> keys;

    // Start is called before the first frame update
    void Awake()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        applyKeyInput();
    }

    private void init()
    {
        keys = new Dictionary<KeyCode, bool>();
    }

    private void applyKeyInput()
    {
        keys.Clear();
        
        if (Input.GetKeyDown(KeyCode.D)) keys.Add(KeyCode.D, true);
        if (Input.GetKeyDown(KeyCode.F)) keys.Add(KeyCode.F, true);
        if (Input.GetKeyDown(KeyCode.J)) keys.Add(KeyCode.J, true);
        if (Input.GetKeyDown(KeyCode.K)) keys.Add(KeyCode.K, true);
        
        if (Input.GetKeyUp(KeyCode.D)) keys.Add(KeyCode.D, false);
        if (Input.GetKeyUp(KeyCode.F)) keys.Add(KeyCode.F, false);
        if (Input.GetKeyUp(KeyCode.J)) keys.Add(KeyCode.J, false);
        if (Input.GetKeyUp(KeyCode.K)) keys.Add(KeyCode.K, false);

        PatternManager.GetInstance().HandleKeyInput(keys);
    }
}
