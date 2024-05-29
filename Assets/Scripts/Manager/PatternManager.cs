using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public RawImage bg;

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

    public TMP_Text nodeTimeText;
    public TMP_Text nodeKeyCodeText;

    private Dictionary<KeyCode, float> pressedDuration;
    private Dictionary<KeyCode, NodeInstance> tempNodeInstance;

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
                if (node.Value.Count <= 0) continue;
                
                timeGap = node.Value.Peek().time + pausedTime - Time.time;

                if (timeGap <= DEFAULT_EXCELLENT_STANDARD)
                {
                    excellentCount++;
                    combo++;
                    
                    if (node.Value.Peek().duration >= 1f / MAGNIFICATION)
                    {
                        node.Value.Peek().time += node.Value.Peek().duration;
                        node.Value.Peek().duration = 0f;
                        node.Value.Peek().PressingNode();
                    }
                    else
                    {
                        Destroy(node.Value.Dequeue().gameObject);
                    }

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
        pressedDuration = new Dictionary<KeyCode, float>();
        pressedDuration.Add(KeyCode.D, 0f);
        pressedDuration.Add(KeyCode.F, 0f);
        pressedDuration.Add(KeyCode.J, 0f);
        pressedDuration.Add(KeyCode.K, 0f);
        tempNodeInstance = new Dictionary<KeyCode, NodeInstance>();
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
            tempNode.InitDuration(node.duration);
            tempNode.transform.localScale = new Vector2(1f, 1f);
            inGameNodes[node.key].Enqueue(tempNode);

            duration = node.time + node.duration;
        }
        
        leftCharacter.Init(file.charNameLeft);
        rightCharacter.Init(file.charNameRight);
        bg.texture = Resources.Load<Texture>($"bg/{file.bg}");
        nodeTransform.localPosition = new Vector3(0f, -DEFAULT_NODE_SIZE * MAGNIFICATION * 1.33f, 0f);
        pausedTime += 1.33f;
        Invoke("PlayBgm", 1.33f);
        paused = false;
        duration += 4.33f;

        UIManager.GetInstance().Init(duration, 100f, progress);

        startTimer.text = "3";
        startTime = 4f;
    }

    public void HandleKeyInput(Dictionary<KeyCode, bool> keys)
    {
        switch (progress)
        {
            case Progress.MAKING:
                foreach (KeyValuePair<KeyCode, bool> key in keys)
                {
                    addPattern(key.Key, key.Value);
                }
                break;
            
            case Progress.PLAYING:
                foreach (KeyValuePair<KeyCode, bool> key in keys)
                {
                    inputKey(key.Key, key.Value);
                }
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
            nodes.Add(new Node(file.id, node.time, node.duration, node.key, 0, 0, ""));
        }
        
        //foreach (Node node in currentNodes)
        //{
        //    nodes.Add(node);
        //}

        JsonManager.CreateJsonFile(JsonManager.DEFAULT_NODE_DATA_NAME, nodes);
    }

    private void addPattern(KeyCode key, bool pressing)
    {
        if (paused) return;
        if (!pressing && pressedDuration[key] > 0f)
        {
            pressedDuration[key] = Time.time - pausedTime - pressedDuration[key];
            currentNodes.Add(new Node(file.id, pressedDuration[key], Time.time - pausedTime - pressedDuration[key], key, 0, 0, ""));
            tempNodeInstance[key].InitDuration(pressedDuration[key]);
            pressedDuration[key] = 0f;
            return;
        }
        else if (!pressing) return;
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

        if (pressedDuration[key] <= 0f)
        {
            tempNodeInstance[key] = Instantiate(nodePrefab, this.transform, true);
            tempNodeInstance[key].InitMaking(Time.time - pausedTime, key, xPos, nodeTransform);
            tempNodeInstance[key].transform.localScale = new Vector2(1f, 1f);
            makingNodes.Enqueue(tempNodeInstance[key]);
            pressedDuration[key] = Time.time - pausedTime;
            return;
        }
    }

    public void UpdateMakingNodeInfo(NodeInstance node)
    {
        currentNode = node;

        nodeTimeText.text = $"Time: {node.time.ToString("F3")}";
        nodeKeyCodeText.text = $"KeyCode: {node.key.ToString()}";
    }

    public void ToggleNodeTime()
    {
        if (currentNode == null) return;

        currentNode.ToggleTime(float.Parse(toggleTimeText.text));
    }
    

    private void inputKey(KeyCode key, bool pressing)
    {
        if (inGameNodes[key].Count <= 0) return;
        
        if (pressing && pressedDuration[key] <= 0f) timeGap = inGameNodes[key].Peek().time + pausedTime - Time.time;
        else if (!pressing && pressedDuration[key] > 0f)
        {
            timeGap = inGameNodes[key].Peek().time + inGameNodes[key].Peek().duration + pausedTime - Time.time;
            if (timeGap >= DEFAULT_GOOD_STANDARD)
            {
                missCount++;
                UIManager.GetInstance().GiveDamage(10f);
                combo = 0;
                UIManager.GetInstance().UpdateScore(excellentCount, goodCount, badCount, missCount);
                UIManager.GetInstance().UpdateCombo(combo, "MISS");
                leftCharacter.PlayAnimation(key);
                rightCharacter.PlayAnimation(key);
                Destroy(inGameNodes[key].Dequeue().gameObject);
                return;
            }
        }
        else if (!pressing) return;

        string currentResult;
        if ((pressing && pressedDuration[key] <= 0f)
            || !pressing && pressedDuration[key] > 0f)
        {
            if (timeGap <= DEFAULT_EXCELLENT_STANDARD
                && timeGap >= -DEFAULT_EXCELLENT_STANDARD)
            {
                excellentCount++;
                currentResult = "Excellent!";
                UIManager.GetInstance().GiveDamage(-3f);
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
                UIManager.GetInstance().GiveDamage(10f);
                combo = 0;
            }
            else
            {
                return;
            }

            bool keepGoing = true;
            
            if (inGameNodes[key].Peek().duration <= 1f / MAGNIFICATION && pressing)
            {
                Destroy(inGameNodes[key].Dequeue().gameObject);
                pressedDuration[key] = 0f;
                keepGoing = false;
            }
            else if (inGameNodes[key].Peek().duration > 1f / MAGNIFICATION && !pressing)
            {
                Destroy(inGameNodes[key].Dequeue().gameObject);
                pressedDuration[key] = 0f;
                keepGoing = false;
            }
            else if (inGameNodes[key].Peek().duration > 1f / MAGNIFICATION && pressing)
            {
                inGameNodes[key].Peek().PressingNode();
            }

            UIManager.GetInstance().UpdateScore(excellentCount, goodCount, badCount, missCount);
            UIManager.GetInstance().UpdateCombo(combo, currentResult);
            leftCharacter.PlayAnimation(key);
            rightCharacter.PlayAnimation(key);
            if (!keepGoing) return;
            
            
            pressedDuration[key] = 0.1f;
        }
    }

    private void inactivateMissedNode()
    {
        float tempTime = 0f;
        foreach (Queue<NodeInstance> nodes in inGameNodes.Values)
        {
            if (nodes.Count <= 0) continue;
            
            if (nodes.Peek().duration <= 1f / MAGNIFICATION) tempTime = nodes.Peek().time + pausedTime - Time.time;
            else if (nodes.Peek().duration > 1f / MAGNIFICATION)
            {
                tempTime = nodes.Peek().time + nodes.Peek().duration + pausedTime - Time.time;
            }
                
            if (tempTime <= -DEFAULT_GOOD_STANDARD)
            {
                Destroy(nodes.Dequeue().gameObject);
                missCount++;
                combo = 0;
                UIManager.GetInstance().GiveDamage(10f);
                
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
