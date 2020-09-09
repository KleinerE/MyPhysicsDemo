using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoodBlockController : MonoBehaviour
{
    public TextMeshProUGUI tmpgui;

    private PhysicsManager m_PhysicsManager;
    private Transform m_Transform;
    private bool m_Collided;
    private AudioSource m_AudioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Collided = false;
        m_Transform = transform;

        m_PhysicsManager = gameObject.GetComponent<PhysicsManager>();
        if (m_PhysicsManager == null)
            Debug.LogWarning("No physics manager found!");

        m_AudioSource = gameObject.GetComponent<AudioSource>();
        if (m_AudioSource == null)
            Debug.LogWarning("No audio source found!");

    }

    // Update is called once per frame
    void Update()
    {
        PhysicsManager pm = m_PhysicsManager.CheckForCollidedObject();
        if (pm != null && !m_Collided)
        {
            m_AudioSource.Play();
            tmpgui.enabled = true;
            StartCoroutine("Collapse");
        }
        else if (pm == null)
        {
            m_Collided = false;
        }


        if(transform.position.z <= -5)
        {
            Destroy(gameObject);
        }
    }


    private IEnumerator Collapse()
    {
        for (float s = 0.6f; s >= 0; s -= 0.01f)
        {
            //Collapse in size.
            Vector3 scale = new Vector3(s, s, s);
            m_Transform.localScale = scale;
            
            //Enlarge Text.
            tmpgui.fontSize = 1 - s / 0.6f;

            //Dissolve effect.
            gameObject.GetComponent<Renderer>().material.SetFloat("_Amount", 1-s/0.6f);

            yield return null;
        }
    }
}
