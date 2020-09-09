using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    public float flightSpeed;

    private Transform m_TargetTransform;

    private Transform m_Transform;
    private Vector3 m_Velocity;
    private bool m_HasCollided;
    private PhysicsManager m_PhysicsManager;
    private ParticleSystem m_ParticleSystem;


    // Start is called before the first frame update
    void Start()
    {
       
        m_PhysicsManager = gameObject.GetComponent<PhysicsManager>();
        if (!m_PhysicsManager.isActiveAndEnabled)
            Debug.LogWarning("No physics manager found!");

        m_ParticleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
        if (m_ParticleSystem == null)
            Debug.LogWarning("No particle system found!");

    }

    private void Awake()
    {
        m_Transform = transform;
        m_HasCollided = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (!m_HasCollided)
        {

        }
        if(CheckOutOfBounds(1000.0f))
        {
            Destroy(gameObject);
        }

        
    }

    public void SetTarget(Transform target)
    {

        if (target != null && m_Transform != null)
        {
            m_PhysicsManager = gameObject.GetComponent<PhysicsManager>();
            m_TargetTransform = target;
            m_Transform.LookAt(target);
            m_PhysicsManager.SetVelocity(m_Transform.forward * flightSpeed);
        }
            
        else
            Debug.LogWarning("No target for asteroid!");

    }

    public void OnShipCollision(CollisionInformation col_info)
    {
        m_PhysicsManager.SetVelocity(col_info.velocity_other);
        m_ParticleSystem.transform.LookAt(col_info.position);
       
    }

    private bool CheckOutOfBounds(float limit)
    {
        return Mathf.Abs(m_Transform.position.x) > limit
                || Mathf.Abs(m_Transform.position.y) > limit
                || Mathf.Abs(m_Transform.position.z) > limit;
    }
}
