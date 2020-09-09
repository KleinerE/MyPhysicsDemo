using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    private static EventManager _instance;
    public static EventManager Instance { get { return _instance; } }

    public float gameStartDelay;
    public float timeBetweenLevels;

    public delegate void ClickAction();
    public static event ClickAction OnClicked;

    public delegate void GameStart();
    public static event GameStart OnGameStarted;

    public delegate void LevelIncrease();
    public static event LevelIncrease OnLevelIncreased;

    public delegate void GameOver();
    public static event GameOver OnGameOver;

    private bool m_GameStarted;
    private int m_CurrentLevel;
    private float m_AwakeTime;
    private float m_CurrentLevelStartTime;
    private bool m_IsGameOver;

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 150, 30), "Change Physics"))
        {
            if (OnClicked != null)
                OnClicked();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(m_IsGameOver);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }


        
        m_AwakeTime = Time.time;
        m_CurrentLevelStartTime = Time.time;
        m_GameStarted = false;
        m_CurrentLevel = 1;
        m_IsGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync("Menu 3D");

        }

        if (Time.time - m_AwakeTime > gameStartDelay && !m_GameStarted)
        {
            m_GameStarted = true;
            if (OnGameStarted != null)
                OnGameStarted();
        }

        if(Time.time - m_AwakeTime > m_CurrentLevel * timeBetweenLevels)
        {
            m_CurrentLevel++;
            if (OnLevelIncreased != null)
                OnLevelIncreased();
        }
    }

    public void PublishGameOver()
    {      
        if (!m_IsGameOver && OnGameOver != null)
        {
            m_IsGameOver = true;
            OnGameOver();
        }            
    }
}
