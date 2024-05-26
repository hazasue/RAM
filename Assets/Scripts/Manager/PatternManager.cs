using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    private Queue<NodeInstance> makingNodes;
    private NodeInstance currentNode;
    private List<Character> characters;
    public Character leftCharacter;
    public Character rightCharacter;

    private int excellentCount;
    private int goodCount;
    private int badCount;
    private int missCount;

    private bool autoPlaying;
    private float timeGap;
    
    public TMP_InputField toggleTimeText;

    public NodeInstance nodePrefab;
    public Transform nodeTransform;
    public Transform nodeParentTransform;
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
        else if (Time.time - pausedTime >= 0f)
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

        if (autoPlaying)
        {
            foreach (KeyValuePair<KeyCode, Queue<NodeInstance>> node in inGameNodes)
            {
                if (node.Value.Count <= 0) return;
                timeGap = node.Value.Peek().time + pausedTime - Time.time;
                if (timeGap <= DEFAULT_EXCELLENT_STANDARD)
                {
                    excellentCount++;
                    combo++;
                    Destroy(node.Value.Dequeue().gameObject);
                    leftCharacter.PlayAnimation(node.Key);
                    rightCharacter.PlayAnimation(node.Key);
                    UIManager.GetInstance().UpdateScore(excellentCount, goodCount, badCount, missCount);
                    UIManager.GetInstance().UpdateCombo(combo, "Excellent!");
                }
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
        SoundManager.GetInstance().InitBgm(file.bgmName);
        Time.timeScale = 1f;

        combo = 0;
        excellentCount = 0;
        goodCount = 0;
        badCount = 0;
        missCount = 0;

        autoPlaying = false;
        timeGap = 0f;
        
        if (tempInfo.createMode) initMaking();
        else
        {
            initPlaying(tempInfo.id);
            if (tempInfo.autoPlay) autoPlaying = true;
        }
    }

    private void initMaking()
    {
        progress = Progress.MAKING;
        nodes = JsonManager.LoadJsonFile<List<Node>>(JsonManager.DEFAULT_NODE_DATA_NAME);
        makingNodes = new Queue<NodeInstance>();
        currentNodes = new List<Node>();
        startTime = -1f;
    }

    private void initPlaying(int id)
    {
        float duration = 0f;
        
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

            duration = node.time;
        }
        
        leftCharacter.Init(file.charNameLeft);
        rightCharacter.Init(file.charNameRight);
        nodeTransform.localPosition = new Vector3(0f, -DEFAULT_NODE_SIZE * MAGNIFICATION * 1.33f, 0f);
        pausedTime += 1.33f;
        Invoke("PlayBgm", 1.33f);
        paused = false;
        duration += 6.33f;

        UIManager.GetInstance().Init(duration, 100f, progress);

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

    public void PlayBgm()
    {
        SoundManager.GetInstance().PlayBgm(true);
    }

    public void StartMaking()
    {
        if (progress != Progress.MAKING) return;

        if (!paused) return;

        Time.timeScale = 1f;

        nodeParentTransform.localPosition = new Vector3(400f, 0f, 0f);

        paused = false;
        PlayBgm();
    }

    public void EndMaking()
    {
        if (progress != Progress.MAKING) return;

        if (paused) return;

        paused = true;
        Time.timeScale = 0f;
        SoundManager.GetInstance().PlayBgm(false);
    }

    public void SaveNodes()
    {
        if (progress != Progress.MAKING) return;

        foreach (NodeInstance node in makingNodes)
        {
            nodes.Add(new Node(file.id, node.time, node.key, 0, 0, ""));
        }
        
        //foreach (Node node in currentNodes)
        //{
        //    nodes.Add(node);
        //}

        JsonManager.CreateJsonFile(JsonManager.DEFAULT_NODE_DATA_NAME, nodes);
    }

    private void addPattern(KeyCode key)
    {
        if (paused) return;
        
        NodeInstance tempNode;
        float xPos = 0f;
        
        switch (key)
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
        tempNode.InitMaking(Time.time - pausedTime, key, xPos, nodeTransform);
        tempNode.transform.localScale = new Vector2(1f, 1f);
        makingNodes.Enqueue(tempNode);
        //inGameNodes[node.key].Enqueue(tempNode);

        currentNodes.Add(new Node(file.id, Time.time - pausedTime, key, 0, 0, ""));
    }

    public void UpdateMakingNodeInfo(NodeInstance node)
    {
        currentNode = node;
    }

    public void ToggleNodeTime()
    {
        if (currentNode == null) return;

        currentNode.ToggleTime(float.Parse(toggleTimeText.text));
    }
    

    private void inputKey(KeyCode key)
    {
        if (inGameNodes[key].Count <= 0) return;
        timeGap = inGameNodes[key].Peek().time + pausedTime - Time.time;
        string currentResult;
        if (timeGap <= DEFAULT_EXCELLENT_STANDARD
            && timeGap >= -DEFAULT_EXCELLENT_STANDARD)
        {
            excellentCount++;
            currentResult = "Excellent!";
            combo++;
        }
        else if (timeGap <= DEFAULT_GOOD_STANDARD
                 && timeGap >= -DEFAULT_GOOD_STANDARD)
        {
            goodCount++;
            currentResult = "Good!";
            combo++;
        }
        else if (timeGap <= DEFAULT_BAD_STANDARD
                 && timeGap >= -DEFAULT_BAD_STANDARD)
        {
            badCount++;
            currentResult = "Bad..";
            combo++;
        }
        else if (timeGap <= DEFAULT_MISS_STANDARD
                 && timeGap <= -DEFAULT_MISS_STANDARD)
        {
            missCount++;
            currentResult = "MISS";
            combo = 0;
        }
        else { return; }


        Destroy(inGameNodes[key].Dequeue().gameObject);
        leftCharacter.PlayAnimation(key);
        rightCharacter.PlayAnimation(key);
        UIManager.GetInstance().UpdateScore(excellentCount, goodCount, badCount, missCount);
        UIManager.GetInstance().UpdateCombo(combo, currentResult);
    }

    private void inactivateMissedNode()
    {
        foreach (Queue<NodeInstance> nodes in inGameNodes.Values)
        {
            if (nodes.Count <= 0) continue;
            if (nodes.Peek().time + pausedTime - Time.time <= -DEFAULT_GOOD_STANDARD)
            {
                Destroy(nodes.Dequeue().gameObject);
                missCount++;
                combo = 0;
                
                UIManager.GetInstance().UpdateCombo(combo, "MISS");
                UIManager.GetInstance().UpdateScore(excellentCount, goodCount, badCount, missCount);
            }
        }
    }
    
    public void BackToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
