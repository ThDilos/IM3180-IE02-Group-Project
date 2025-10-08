using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;
public class GameController : MonoBehaviour
{
    // Always Accessible as an Instance
    public static GameController Instance { get; private set; }

    [Header("Frame rate Control")]
    [Tooltip("How many physics calculations per second?\nWarning: Will change A LOT stuff")]
    [SerializeField] private int updatePerSecond = 20;
    [Tooltip("Render Framerate, does not affect gameplay.\nThe conventional 'FPS'")]
    [SerializeField] private int frameRate = 60;

    [SerializeField] private KeyCode debugActivate = KeyCode.O;
    private bool debugMode = false;

    [SerializeField] private GameObject debugCanvas;
    private TMP_Text debugLogs;

    [SerializeField] public bool gotLabReport = false;
    [SerializeField] private GameObject labReport;

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
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Limit UpdateRate [Runtime]
        Time.fixedDeltaTime = 1 / updatePerSecond;
        timeScale = Time.timeScale;

        // Limit Framerate [Render Pipeline]
        try { frameRate = PlayerPrefs.GetInt("FPS"); } catch { };
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = frameRate; // Default fps is set to 60, so that your GPU won't scream eve

        // Try Find Debug Canvas
        if (debugCanvas == null)
            debugCanvas = GameObject.Find("DebugModeCanvas");
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

    // Only called once in tutorial room
    public void ObtainLabReport()
    {
        gotLabReport = true;
        
        if (labReport != null)
        {
            labReport.SetActive(true);
        }
    }
}
