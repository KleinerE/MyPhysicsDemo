using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthController : MonoBehaviour
{
    
    public PhysicsManager spaceshipPM;
    public GameObject explosionPrefab;

    private PhysicsManager m_PhysicsManager;
    private List<PhysicsManager> m_CollidedObjects;

    // Start is called before the first frame update
    void Start()
    {
        m_PhysicsManager = gameObject.GetComponent<PhysicsManager>();
        if (m_PhysicsManager == null)
            Debug.LogWarning("No physics manager found!");

        m_PhysicsManager.AddOtherObject(spaceshipPM);
    }

    private void Awake()
    {

        m_CollidedObjects = new List<PhysicsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsManager pm = m_PhysicsManager.CheckForCollidedObject();
        if (pm != null && !m_CollidedObjects.Contains(pm))
        {
            m_CollidedObjects.Add(pm);
            CollisionInformation col_info = m_PhysicsManager.ExecuteCollisionWithObject(pm);
            Instantiate(explosionPrefab, col_info.position, Quaternion.LookRotation(col_info.position - transform.position));
        }

    }
}
