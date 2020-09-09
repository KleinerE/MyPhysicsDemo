using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UserInput
{
    StartGoingRight,
    SteadyGoingRight,
    StartGoingLeft,
    SteadyGoingLeft,
    Stop,
    Bounce
}


public class CollectorController : MonoBehaviour
{
    public float sidewaysSpeed;
    public float sidewaysAccForce;
    public float bounceSpeed;
    public float minX, maxX;

    private PhysicsManager m_PhysicsManager;
    private Transform m_Transform;
    private bool m_IsMovingRight, m_IsMovingLeft;
    private Vector3 m_RightMovementVel, m_LeftMovementVel;
    private bool m_IsMovingVertical;
    private UserInput m_CurrentInput;
    private AudioSource m_AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        m_Transform = transform;

        m_PhysicsManager = gameObject.GetComponent<PhysicsManager>();
        if (m_PhysicsManager == null)
            Debug.LogWarning("No physics manager found!");

        m_AudioSource = gameObject.GetComponent<AudioSource>();
        if (m_AudioSource == null)
            Debug.LogWarning("No audio source found!");

        m_RightMovementVel = new Vector3(sidewaysSpeed, 0.0f, 0.0f);
        m_LeftMovementVel = new Vector3(-sidewaysSpeed, 0.0f, 0.0f);
        m_IsMovingVertical = false;

        m_CurrentInput = UserInput.Stop;
        EventManager.OnGameOver += OnGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrentInput = ReadInput(m_CurrentInput);
        HandleInput();
    }

    public IEnumerator Dissolve()
    {
        for (float a = 0.0f; a <= 1.0f; a += 0.005f)
        {
            gameObject.GetComponent<Renderer>().material.SetFloat("_Amount", a);
            yield return null;
        }
    }

    private void OnGameOver()
    {
        m_AudioSource.Play();
        StartCoroutine("Dissolve");
    }

    private void HandleInput()
    {
        switch (m_CurrentInput)
        {
            case UserInput.StartGoingRight:
                m_PhysicsManager.AccelerateToVelocity(m_RightMovementVel, accelerationMagnitude: sidewaysAccForce);
                break;
            case UserInput.StartGoingLeft:
                m_PhysicsManager.AccelerateToVelocity(m_LeftMovementVel, accelerationMagnitude: sidewaysAccForce);
                break;
            case UserInput.Stop:
                m_PhysicsManager.AccelerateToVelocity(new Vector3(), accelerationMagnitude: sidewaysAccForce);
                break;

            case UserInput.SteadyGoingRight:
                if (transform.position.x > maxX)
                {
                    m_PhysicsManager.SetAcceleration(new Vector3(0.0f, 0.0f, 0.0f));
                    m_PhysicsManager.SetVelocity(new Vector3(0.0f, 0.0f, 0.0f));
                }
                break;
            case UserInput.SteadyGoingLeft:
                if (transform.position.x < minX)
                {
                    m_PhysicsManager.SetAcceleration(new Vector3(0.0f, 0.0f, 0.0f));
                    m_PhysicsManager.SetVelocity(new Vector3(0.0f, 0.0f, 0.0f));
                }
                break;
            case UserInput.Bounce:
                if (!m_IsMovingVertical)
                {
                    m_IsMovingVertical = true;
                    m_PhysicsManager.SetVelocityY(bounceSpeed);
                    m_PhysicsManager.SetAccelerationY(-9.81f);
                }

                break;


            default:
                Debug.LogWarning("Warning: Unrecognized input");
                Debug.LogWarning(m_CurrentInput);
                m_CurrentInput = UserInput.Stop;
                break;

        }
    }

    private UserInput ReadInput(UserInput currentInput)
    {
        /*
        if (Input.GetKey("space"))
        {
            if (currentInput != UserInput.Bounce)
                return UserInput.Bounce;
        }
        */
        if (Input.GetKey("d") || Input.GetKey("right"))
        {
            switch (currentInput)
            {
                case UserInput.StartGoingRight:
                case UserInput.SteadyGoingRight:
                    return UserInput.SteadyGoingRight;
                    //break;
                case UserInput.StartGoingLeft:
                case UserInput.SteadyGoingLeft:
                    return UserInput.Stop;
                //break;
                default:
                    return UserInput.StartGoingRight;
            }
        }
        if (Input.GetKey("a") || Input.GetKey("left"))
        {
            switch (currentInput)
            {
                case UserInput.StartGoingLeft:
                case UserInput.SteadyGoingLeft:
                    return UserInput.SteadyGoingLeft;
                //break;
                case UserInput.StartGoingRight:
                case UserInput.SteadyGoingRight:
                    return UserInput.Stop;
                //break;
                default:
                    return UserInput.StartGoingLeft;
            }
        }
        

        return UserInput.Stop;
    }

}
