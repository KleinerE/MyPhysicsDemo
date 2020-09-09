using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NativeObjectType
{
    Sphere,
    Cube
}

public enum PhysicsEngine
{
    NativeLibrary,
    BuiltIn
}


public class PhysicsManager : MonoBehaviour
{
    public NativeObjectType objType;
    public double baseLength = 0.5d;
    public double mass = 1.0d;
    public bool floorCollidable = false;
    public double floorHeight = -1000.0d;

    private IntPtr m_NativePhysicsObject = IntPtr.Zero;
    private PhysicsLibFacade m_Lib = new PhysicsLibFacade();
    private Transform m_Transform;

    private Vector3 m_Velocity;
    private Vector3 m_Acceleration;

    private PhysicsEngine m_CurrentPhysEngine;
    private Rigidbody m_RigidBody;
    private Collider m_BuiltInCollider;

    private bool applyingAcceleration = false;
    private float currentAccelerationEndTime = 0.0f;
    private bool floorCollided = false;
    private List<PhysicsManager> knownObjects = new List<PhysicsManager> { };

    private void Init(NativeObjectType objtype)
    {
        switch(objtype)
        {
            case NativeObjectType.Sphere:
                m_NativePhysicsObject = m_Lib.CreateNativePhysicsSphere(transform.position, baseLength, mass);
                break;
            case NativeObjectType.Cube:
                m_NativePhysicsObject = m_Lib.CreateNativePhysicsCube(transform.position, baseLength, mass);
                break;
            default:
                Debug.LogError("Unknown native object type.");
                m_NativePhysicsObject = IntPtr.Zero;
                break;
        }
    }
    
   
    private void Awake()
    {
        Init(objType);
        m_CurrentPhysEngine = PhysicsEngine.NativeLibrary;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_Transform = transform;
        EventManager.OnClicked += OnPhysicsChange;

        m_RigidBody = gameObject.GetComponent<Rigidbody>();
        if (!m_RigidBody)
            Debug.LogError("Error: No RigidBody found.", gameObject);

        m_BuiltInCollider = gameObject.GetComponent<Collider>();
        if (!m_BuiltInCollider)
            Debug.LogError("Error: No Built In Collider found.", gameObject);
    }

    private void OnDestroy()
    {
        EventManager.OnClicked -= OnPhysicsChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    } 

    private void FixedUpdate()
    {
        if(applyingAcceleration && Time.time <= currentAccelerationEndTime)
        {
            m_Lib.SetAcceleration(m_NativePhysicsObject, m_Acceleration);
        }
        else if (applyingAcceleration && Time.time > currentAccelerationEndTime)
        {
            applyingAcceleration = false;
            m_Acceleration.Set(0.0f, 0.0f, 0.0f);
            m_Lib.SetAcceleration(m_NativePhysicsObject, m_Acceleration);
        }


        if(floorCollidable && !floorCollided)
        {
            if (m_Lib.CheckCollisionWithFloor(m_NativePhysicsObject, floorHeight))
            {
                floorCollided = true;
                SetVelocityY(0.0f);
                SetAccelerationY(0.0f);
            }
        }

        /******** Update position & velocity based on current physics engine ********/
        switch(m_CurrentPhysEngine)
        {
            case PhysicsEngine.NativeLibrary:
                m_RigidBody.isKinematic = true;
                m_Lib.OnUpdate(m_NativePhysicsObject, Time.deltaTime);
                m_Transform.position = m_Lib.GetPosition(m_NativePhysicsObject);
                m_Velocity = m_Lib.GetVelocity(m_NativePhysicsObject);
                break;
            case PhysicsEngine.BuiltIn:
                m_RigidBody.isKinematic = false;
                m_Velocity = m_RigidBody.velocity;
                break;
        }
        
    }

    public IntPtr GetNativeObject()
    {
        return m_NativePhysicsObject;
    }

    public void SetVelocity(Vector3 velocity)
    {       
        m_Velocity = velocity;
        applyingAcceleration = false;
        m_Lib.SetVelocity(m_NativePhysicsObject, velocity);
    }

    public void SetAcceleration(Vector3 acceleration)
    {
        m_Acceleration = acceleration;
        applyingAcceleration = false;
        m_Lib.SetAcceleration(m_NativePhysicsObject, acceleration);
    }

    public Vector3 GetVelocity()
    {
        return m_Velocity;
    }

    

    public void AddOtherObject(PhysicsManager pm)
    {
        m_Lib.AddOtherObject(m_NativePhysicsObject, pm.GetNativeObject());
        knownObjects.Add(pm);
    }

    public PhysicsManager CheckForCollidedObject()
    {
        IntPtr collidedNativeObj = m_Lib.CheckAllObjectCollisions(m_NativePhysicsObject);
        if (collidedNativeObj != IntPtr.Zero)
        {
            foreach (PhysicsManager pm in knownObjects)
            {
                if (pm.GetNativeObject() == collidedNativeObj)
                {
                    return pm;
                }    
            }
        }
        return null;
    }
    
    public CollisionInformation ExecuteCollisionWithObject(PhysicsManager otherPM)
    {
        return m_Lib.ExecuteCollisionWithObject(m_NativePhysicsObject, otherPM.GetNativeObject());
    }
    
    public void SetVelocityY(float vy)
    {
        Vector3 vel = new Vector3(m_Velocity.x, vy, m_Velocity.z);
        SetVelocity(vel);
    }

    public void SetAccelerationY(float ay)
    {
        Vector3 acc = new Vector3(m_Acceleration.x, ay, m_Acceleration.z);
        SetAcceleration(acc);
    }

    public void ApplyAccelerationOverTime(Vector3 acceleration, float time)
    {
        applyingAcceleration = true;
        currentAccelerationEndTime = Time.time + time;
        m_Acceleration = acceleration;
    }

    public void DecelerateOverTime(float time)
    {
        Vector3 acc = -m_Velocity / time;
        ApplyAccelerationOverTime(acc, time);
    }

    public void AccelerateToVelocity(Vector3 targetVelocity, float timeToTargetVelocity = -1.0f, float accelerationMagnitude = -1.0f)
    {
        Vector3 deltaV = targetVelocity - m_Velocity;
        if(timeToTargetVelocity < 0.0f && accelerationMagnitude > 0.0f)
        {
            // acceleration magnitude specified and time not specified -> calculate time.
            float time = deltaV.magnitude / accelerationMagnitude;
            Vector3 acc = deltaV.normalized * accelerationMagnitude;
            ApplyAccelerationOverTime(acc, time);
            return;
        }
        if (timeToTargetVelocity > 0.0f && accelerationMagnitude < 0.0f)
        {
            // acceleration magnitude not specified and time specified -> calculate acceleration.
            float accM = deltaV.magnitude / timeToTargetVelocity;
            Vector3 acc = deltaV.normalized * accM;
            ApplyAccelerationOverTime(acc, timeToTargetVelocity);
            return;
        }
        else
        {
            Debug.LogWarning("Accelerate to velocity: Not enough parameters specified. Must specify either Time to Target Velocity or Acceleration Magnitude.");
            return;
        }
    }


    private void OnPhysicsChange()
    {
        switch(m_CurrentPhysEngine)
        {
            case PhysicsEngine.NativeLibrary:
                //Change native to builtin.
                m_CurrentPhysEngine = PhysicsEngine.BuiltIn;
                m_RigidBody.velocity = m_Velocity;
                break;
            case PhysicsEngine.BuiltIn:
                //Change builtin to native.
                m_CurrentPhysEngine = PhysicsEngine.NativeLibrary;
                m_Velocity = m_RigidBody.velocity;
                Vector3 newpos = m_Transform.position + new Vector3(0.0f, (float)floorHeight, 0.0f);
                m_Lib.SetPosition(m_NativePhysicsObject, newpos);
                break;
        }
    }

   public void SetPhysicsEngine(PhysicsEngine eng)
    {
        m_CurrentPhysEngine = eng;
    }
}
