using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadBlockController : MonoBehaviour
{
    PhysicsManager m_PhysicsManager;
    Transform m_Transform;
    bool m_Collided;

    // Start is called before the first frame update
    void Start()
    {
        m_Collided = false;
        m_Transform = transform;

        m_PhysicsManager = gameObject.GetComponent<PhysicsManager>();
        if (m_PhysicsManager == null)
            Debug.LogWarning("No physics manager found!");
    }

    // Update is called once per frame
    void Update()
    {
        PhysicsManager pm = m_PhysicsManager.CheckForCollidedObject();
        if (pm != null && !m_Collided)
        {
            CollectorController cc = pm.gameObject.GetComponent<CollectorController>();
            GCCameraController gccam = Camera.main.gameObject.GetComponent<GCCameraController>();
            if(cc.isActiveAndEnabled)
            {
                /*  Game Over! */
                //EventManager em = new EventManager();
                EventManager.Instance.PublishGameOver();
                //EventManager em = gameObject.AddComponent<EventManager>();
                //em.PublishGameOver();
            }
        }
        else if (pm == null)
        {
            m_Collided = false;
        }


        if (transform.position.z <= -5)
        {
            Destroy(gameObject);
        }
    }
}
