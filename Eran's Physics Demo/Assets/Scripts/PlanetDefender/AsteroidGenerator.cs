using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public Transform targetTransform;
    public PhysicsManager spaceshipPM, earthPM;
    public float m_TimeDelayBetweenGenerations;
    private float m_TimeOfLastGeneration;
    private Vector3 m_GenLocation;

    // Start is called before the first frame update
    void Start()
    {
        m_TimeOfLastGeneration = 4.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > m_TimeOfLastGeneration + m_TimeDelayBetweenGenerations)
        {
            m_TimeOfLastGeneration = Time.time;
            m_GenLocation.x = Random.Range(-500, 500);
            m_GenLocation.y = Random.Range(100, 150);
            m_GenLocation.z = Random.Range(-500, 500);
            GenerateAsteroid();
        }
    }

    private void GenerateAsteroid()
    {
        GameObject instance = Instantiate(asteroidPrefab, m_GenLocation, Quaternion.identity, transform);
        instance.GetComponent<AsteroidController>().SetTarget(targetTransform);
        earthPM.AddOtherObject(instance.GetComponent<PhysicsManager>());
        spaceshipPM.AddOtherObject(instance.GetComponent<PhysicsManager>());
    }
}
