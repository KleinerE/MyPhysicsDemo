using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float flightSpeed;
    public Transform cameraTransform;
    public bool isMoving;
    public GameObject explosionPrefab;
    public PDCameraController m_Camera;

    private PhysicsManager m_PhysicsManager;
    private Transform m_Transform;
    private bool m_Collided;
    private double[] collision_info;


    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        m_Transform = transform;
        m_PhysicsManager = gameObject.GetComponent<PhysicsManager>();
        if (m_PhysicsManager == null)
            Debug.LogWarning("No physics manager found!");
        m_Collided = false;
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("w"))
        {
            isMoving = true;
            m_Camera.StartMovementAway();
        }
        if (Input.GetKeyUp("w"))
        {
            isMoving = false;
            m_PhysicsManager.SetVelocity(new Vector3(0.0f, 0.0f, 0.0f));
            m_Camera.StartMovementToward();
        }
        
        
        
        PhysicsManager pm = m_PhysicsManager.CheckForCollidedObject();
        if (pm != null && !m_Collided)
        {
            m_Collided = true;
            CollisionInformation col_info = m_PhysicsManager.ExecuteCollisionWithObject(pm);
            pm.gameObject.GetComponent<AsteroidController>().OnShipCollision(col_info);
            m_PhysicsManager.SetVelocity(col_info.velocity_own);

            Instantiate(explosionPrefab, col_info.position, Quaternion.identity);           
        }
        else if(pm == null)
        {          
            m_Collided = false;
        }
        

        if (isMoving && !m_Collided)
        {
            m_PhysicsManager.SetVelocity(m_Transform.forward * flightSpeed);
        }

        var step = 120.0f * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_Camera.transform.rotation, step);
    }

}
