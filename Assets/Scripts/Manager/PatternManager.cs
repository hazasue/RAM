using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PatternManager : MonoBehaviour
{
    public enum Progress
    {
        MAKING,
        PLAYING,
    }

    private static PatternManager instance;
    public static float time;
    public static float DEFAULT_NODE_SIZE = 150f;
    public static float MAGNIFICATION = 8f;
    public static float DEFAULT_NODE_X_POS_D = -300f;
    public static float DEFAULT_NODE_X_POS_F = -100f;
    public static float DEFAULT_NODE_X_POS_J = 100f;
    public static float DEFAULT_NODE_X_POS_K = 300f;

    public const float DEFAULT_MISS_STANDARD = 0.2f;
    public const float DEFAULT_BAD_STANDARD = 0.15f;
    public const float DEFAULT_GOOD_STANDARD = 0.1f;
    public const float DEFAULT_EXCELLENT_STANDARD = 0.075f;

    
    
    private GameFile file;
    private int combo;
    private float maxHp;
    private float hp;
    private float pausedTime;
    private bool paused;
    private List<Node> nodes;
    private List<Node> currentNodes;
    private Dictionary<KeyCode, Queue<NodeInstance>> inGameNodes;
    private List<Character> characters;

    public NodeInstance nodePrefab;
    public Transform nodeTransform;
    public TMP_Text startTimer;
    private float startTime;

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
        if (paused) pausedTime += Time.deltaTime;
        else
        {
            UIManager.GetInstance().UpdateTimer(Time.time - pausedTime);
        }

        if (startTime > 1f)
        {
            startTime -= Time.deltaTime * 3f;
            
            startTimer.text = $"{((int)startTime).ToString("D1")}";
            if (startTime <= 1f)
            {
                startTimer.text = $"GO!";
                Destroy(startTimer.gameObject, 0.33f);
            }
        }

        if (progress == Progress.PLAYING) inactivateMissedNode();
    }

    public static PatternManager GetInstance()
    {
        if (instance != null) return instance;
        instance = FindObjectOfType<PatternManager>();
        if (instance == null) Debug.Log("There's no active PatternManager object");
        return instance;
    }
    
    private void init()
    {
        CurrentInfo tempInfo = JsonManager.LoadJsonFile<CurrentInfo>(JsonManager.DEFAULT_CURRENT_INFO_NAME);
        file = JsonManager.LoadJsonFile<Dictionary<int, GameFile>>(JsonManager.DEFAULT_GAMEFILE_DATA_NAME)[tempInfo.id];
        pausedTime = Time.time;
        paused = true;
        instance = this;
        
        if (tempInfo.createMode) initMaking();
        else
        {
            initPlaying(tempInfo.id);
        }
    }

    private void initMaking()
    {
        progress = Progress.MAKING;
        nodes = JsonManager.LoadJsonFile<List<Node>>(JsonManager.DEFAULT_NODE_DATA_NAME);
        currentNodes = new List<Node>();
        startTime = -1f;
    }

    private void initPlaying(int id)
    {
        progress = Progress.PLAYING;
        nodes = JsonManager.LoadJsonFile<List<Node>>(JsonManager.DEFAULT_NODE_DATA_NAME);

        inGameNodes = new Dictionary<KeyCode, Queue<NodeInstance>>();
        inGameNodes.Add(KeyCode.D, new Queue<NodeInstance>());
        inGameNodes.Add(KeyCode.F, new Queue<NodeInstance>());
        inGameNodes.Add(KeyCode.J, new Queue<NodeInstance>());
        inGameNodes.Add(KeyCode.K, new Queue<NodeInstance>());

        currentNodes = new List<Node>();
        foreach (Node node in nodes)
        {
            if (node.file == id) currentNodes.Add(node);
        }

        float xPos = 0f;
        NodeInstance tempNode;

        foreach (Node node in currentNodes)
        {
            switch (node.key)
            {
                case KeyCode.D:
                    xPos = DEFAULT_NODE_X_POS_D;
                    break;

                case KeyCode.F:
                    xPos = DEFAULT_NODE_X_POS_F;
                    break;

                case KeyCode.J:
                    xPos = DEFAULT_NODE_X_POS_J;
                    break;

                case KeyCode.K:
                    xPos = DEFAULT_NODE_X_POS_K;
                    break;

                default:
                    break;
            }

            tempNode = Instantiate(nodePrefab, this.transform, true);
            tempNode.Init(node.time, node.key, xPos, nodeTransform);
            tempNode.transform.localScale = new Vector2(1f, 1f);
            inGameNodes[node.key].Enqueue(tempNode);
        }

        nodeTransform.localPosition = new Vector3(0f, -DEFAULT_NODE_SIZE * MAGNIFICATION * 1.33f, 0f);
        pausedTime += 1.33f;
        paused = false;

        startTimer.text = "3";
        startTime = 4f;
    }

    public void HandleKeyInput(List<KeyCode> keys)
    {
        switch (progress)
        {
            case Progress.MAKING:
                foreach (KeyCode key in keys) { addPattern(key); }
                break;
            
            case Progress.PLAYING:
                foreach (KeyCode key in keys) { inputKey(key); }
                break;
            
            default:
                break;
        }
    }

    public void StartMaking()
    {
        if (progress != Progress.MAKING) return;
        
        if (paused) paused = false;
        else
        {
            paused = true;
        }
    }

    public void SaveNodes()
    {
        if (progress != Progress.MAKING) return;
        
        foreach (Node node in currentNodes)
        {
            nodes.Add(node);
        }

        JsonManager.CreateJsonFile(JsonManager.DEFAULT_NODE_DATA_NAME, nodes);
    }

    private void addPattern(KeyCode key)
    {
        if (paused) return;

        currentNodes.Add(new Node(file.id, Time.time - pausedTime, key, 0, 0, ""));
    }

    private void inputKey(KeyCode key)
    {
        if (inGameNodes[key].Count <= 0) return;
        float timeGap = inGameNodes[key].Peek().time + pausedTime - Time.time;
        if (timeGap <= DEFAULT_EXCELLENT_STANDARD
            && timeGap >= -DEFAULT_EXCELLENT_STANDARD)
        {
            
        }
        else if (timeGap <= DEFAULT_GOOD_STANDARD
                 && timeGap >= -DEFAULT_GOOD_STANDARD)
        {}
        else if (timeGap <= DEFAULT_BAD_STANDARD
                 && timeGap >= -DEFAULT_BAD_STANDARD)
        {}
        else if (timeGap <= DEFAULT_MISS_STANDARD
                 && timeGap <= -DEFAULT_MISS_STANDARD)
        {}
        else { return; }

        Debug.Log(timeGap);
        Destroy(inGameNodes[key].Dequeue().gameObject);
    }

    private void inactivateMissedNode()
    {
        foreach (Queue<NodeInstance> nodes in inGameNodes.Values)
        {
            if (nodes.Count <= 0) continue;
            if (nodes.Peek().time + pausedTime - Time.time <= -DEFAULT_GOOD_STANDARD)
                Destroy(nodes.Dequeue().gameObject);
        }
    }
}
