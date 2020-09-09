using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTextController : MonoBehaviour
{
    //public float beginAnimationTime;
    private TextMeshProUGUI tmpro;
    private string[] texts_begin, texts_over;
    public float stageTime;
    private int prevStage;
    private AudioSource m_AudioSource;

    private bool m_IsGameOver;
    private float m_GameOverStartTime;
    private float m_AwakeTime;

    private void Awake()
    {
        texts_begin = new string[] {
        "_",
        " ",
        "_",
        " ",
        "_",
        "B_",
        "Be_",
        "Beg_",
        "Begi_",
        "Begin_",
        "Begin!_",
        "Begin! ",
        "Begin!_",
        "Begin! "};

        texts_over = new string[] {
        "_",
        " ",
        "_",
        " ",
        "_",
        "G_",
        "Ga_",
        "Gam_",
        "Game_",
        "Game _",
        "Game O_",
        "Game Ov_",
        "Game Ove_",
        "Game Over_",
        "Game Over!_",
        "Game Over! ",
        "Game Over!_",
        "Game Over! ",};

        
        prevStage = -1;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_AwakeTime = Time.time;
        tmpro = gameObject.GetComponent<TextMeshProUGUI>();
        m_AudioSource = gameObject.GetComponent<AudioSource>();

        EventManager.OnGameOver += OnGameOver;
    }


    // Update is called once per frame
    void Update()
    {
        if(Time.time - m_AwakeTime < stageTime * texts_begin.Length)
        {
            int stage = (int)((Time.time - m_AwakeTime) / stageTime);
            tmpro.text = texts_begin[stage];
            if(stage > prevStage)
            {
                prevStage = stage;
                if (stage >= 5 && stage <= 10)
                    m_AudioSource.Play();
            }
        }
        else if(!m_IsGameOver)
            tmpro.text = "";

        if(m_IsGameOver)
        {
            if (Time.time - m_AwakeTime - m_GameOverStartTime < stageTime * texts_over.Length)
            {
                int stage = (int)((Time.time - m_AwakeTime - m_GameOverStartTime) / stageTime);
                tmpro.text = texts_over[stage];
                if (stage > prevStage)
                {
                    prevStage = stage;
                    if (stage >= 5 && stage <= 14)
                        m_AudioSource.Play();
                }
            }
        }
    }

    private void OnGameOver()
    {
        m_GameOverStartTime = Time.time;
        m_IsGameOver = true;
        prevStage = -1;
    }
}
