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
        applyKeyInput();
    }

    private void init()
    {
        keys = new List<KeyCode>();
    }

    private void applyKeyInput()
    {
        keys.Clear();

        if (Input.GetKeyDown(KeyCode.D)) keys.Add(KeyCode.D);
        if (Input.GetKeyDown(KeyCode.F)) keys.Add(KeyCode.F);
        if (Input.GetKeyDown(KeyCode.J)) keys.Add(KeyCode.J);
        if (Input.GetKeyDown(KeyCode.K)) keys.Add(KeyCode.K);

        PatternManager.GetInstance().HandleKeyInput(keys);
    }
}
