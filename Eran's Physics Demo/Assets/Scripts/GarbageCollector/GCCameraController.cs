using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GCCameraController : MonoBehaviour
{
    public Transform targetTransform;
    public float hDistance, vDistance;
    public TextMeshProUGUI tmpgui;
    public float shakeStrength;

    private Transform m_Transform;
    private Vector3 m_PosShift, m_PosShiftShake;
    private AudioSource m_AudioSource;
    private bool m_MusicStarted;
    private int m_CurrentLevel;
  

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = transform;
        m_PosShift = new Vector3(0.0f, vDistance, -hDistance);

        m_AudioSource = gameObject.GetComponent<AudioSource>();
        if (!m_AudioSource.isActiveAndEnabled)
            Debug.LogError("Error: No Audio Source found!");

        EventManager.OnGameStarted += StartMusic;
        EventManager.OnLevelIncreased += OnLevelIncreased;
        EventManager.OnGameOver += OnGameOver;
    }

    private void Awake()
    {
        m_CurrentLevel = 1;
        m_PosShiftShake = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        m_Transform.position = targetTransform.position + m_PosShift + m_PosShiftShake;
    }

    private void StartMusic()
    {
        
        m_AudioSource.Play();
    }

    private void OnLevelIncreased()
    {
        m_CurrentLevel++;
        tmpgui.text = "Level: " + m_CurrentLevel;
    }

    public IEnumerator CameraShake()
    {
        for (float s = 10.0f; s >= 0; s -= 0.05f)
        {
            Vector3 v = Random.insideUnitSphere * shakeStrength;
            m_PosShiftShake += v;
        }            
        yield return null;
    }

    private void OnGameOver()
    {
        StartCoroutine("CameraShake");
    }

    
}
