using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using static DialogPopUp;
using static Water;
public class GameController : MonoBehaviour
{
    // Always Accessible as an Instance
    public static GameController Instance { get; private set; }

    [Header("Frame rate Control (Changes require restarting the game)")]
    [Tooltip("How many physics calculations per second?\nWarning: Will change A LOT stuff")]
    [SerializeField] private int updatePerSecond = 20;
    [Tooltip("Render Framerate, does not affect gameplay.\nThe conventional 'FPS'")]
    [SerializeField] private int frameRate = 60;

    [Header("Debugger in Application")]
    [SerializeField] private KeyCode debugActivate = KeyCode.O;
    private bool debugMode = false;

    [SerializeField] private GameObject debugCanvas;
    private TMP_Text debugLogs;

    [Header("Lab Report Object for Unlocking")]
    [SerializeField] public bool gotLabReport = false;
    [SerializeField] private GameObject labReport;

    [Header("Knife Object for Unlocking")]
    [SerializeField] public bool gotKnife = false;
    [SerializeField] private GameObject knife;

    [Header("Rubber Duck Object for Unlocking")]
    [SerializeField] public bool gotRubberDuck = false;
    [SerializeField] private int rubberDuckCount;
    [SerializeField] private int unlockDuckCount;
    [SerializeField] private GameObject[] rubberDucks;

    [Header("Flower Pot Object for Unlocking")]
    [SerializeField] public bool pushedFlowerPot = false;
    [SerializeField] private int pushedPots;
    [SerializeField] private int unlockPotCount;
    [SerializeField] private GameObject[] flowerPots;

    [Header("Cat go OOB for Unlocking")]
    [SerializeField] public bool catOOB = false;

    [Header("Lore Object for Unlocking")]
    [SerializeField] public bool gotLore1 = false;
    [SerializeField] private GameObject lore1fromPrinter;

    [SerializeField] public bool gotLore2 = false;
    [SerializeField] private GameObject lore2fromPrinter;

    [SerializeField] public bool gotLore3 = false;
    [SerializeField] private GameObject lore3fromPrinter;


    [Header("Central Components Control")]
    public InputActionAsset inputActions;

    // Runtime Vars
    public bool isPaused = false;
    private float timeScale;

    private void Awake()
    {
        // If there's already one instance, destroy the new one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Assign and make persistent throughout scenes
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Limit UpdateRate [Runtime]
        Time.fixedDeltaTime = 1f / updatePerSecond;
        timeScale = Time.timeScale;

        // Limit Framerate [Render Pipeline]
        PlayerPrefs.SetInt("FPS", frameRate); // temp, to be deleted afterwards.
        try { frameRate = PlayerPrefs.GetInt("FPS"); } catch { }
        ;
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = frameRate; // Default fps is set to 60, so that your GPU won't scream eve
    }

    public void Respawn()
    {
        // Reload the Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(debugActivate))
        {
            debugMode = !debugMode;
        }
        if (debugMode)
        {
            handleDebug();
        }
        if (debugLogs == null)
        {
            try
            {
                debugLogs = GameObject.Find("Debug Logs").GetComponent<TMP_Text>();
            }
            catch
            {
                Debug.Log("Cannot Find Debug Log TMP Text!");
            }
        }
        debugCanvas.SetActive(debugMode);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = timeScale;
        Debug.Log("Game Resumed");
    }

    private void handleDebug()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        else if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        else if (Input.GetKeyDown(KeyCode.C))
            myLogQueue.Clear();
    }

    string myLog;
    Queue myLogQueue = new Queue();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "[" + type + "]: " + myLog + "\n";
        myLogQueue.Enqueue(newString);

        if (type == LogType.Exception)
        {
            newString = "" + stackTrace;
            myLogQueue.Enqueue(newString);
        }
        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
        if (myLogQueue.Count > 10)
        {
            myLogQueue.Dequeue();
        }
        if (debugLogs != null) debugLogs.text = myLog;
    }

    public void DelayedSpawnObject(Vector3 pos, Quaternion rot, GameObject obj, float delay)
    {
        StartCoroutine(SpawnObj(pos, rot, obj, delay));
    }

    IEnumerator SpawnObj(Vector3 pos, Quaternion rot, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject newObj = Instantiate(obj, pos, rot);
        newObj.SetActive(true);
        RespawnableObject script = newObj.GetComponent<RespawnableObject>();
        script.TriggerRespawnSFX();
        Debug.Log("New Clone of " + obj.name + " created at " + pos);
        Destroy(obj);
    }

    // Only called once in tutorial room
    public void ObtainLabReport()
    {
        gotLabReport = true;

        if (labReport != null)
        {
            labReport.SetActive(true);
        }
    }
    public void ObtainKnife()
    {
        gotKnife = true;
    }
    public void ObtainRubberDuck()
    {
        if (gotRubberDuck) return;

        foreach (GameObject duck in rubberDucks)
        {
            if (duck != null && duck.activeInHierarchy)
                return; 
        }

        gotRubberDuck = true;
    }
    public void PushFlowerPot()
    {
        if (pushedFlowerPot) return;

        foreach (GameObject pot in flowerPots)
        {
            if (pot != null && pot.activeInHierarchy)
                return;
        }

        pushedFlowerPot = true;
    }
    public void oob()
    {
        catOOB = true;
        Debug.Log("TRUE");
    }
    public void ObtainLore1()
    {
        gotLore1 = true;
    }
    public void ObtainLore2()
    {
        gotLore2 = true;
    }
    public void ObtainLore3()
    {
        gotLore3 = true;
    }
}