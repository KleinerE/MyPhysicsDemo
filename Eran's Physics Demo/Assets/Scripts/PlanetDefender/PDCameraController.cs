using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDCameraController : MonoBehaviour
{

    public Transform targetTransform;
    public float idleDistance;
    public float flyDistance;
    public float height;
    public float xSpeed, ySpeed;
    public float yMinLimit, yMaxLimit;
    public float dampingTime;

    private Transform m_Transform;
    private float x, y;
    private Vector3 localPosition;
    private float currentDistance;
    private float movementStartTime, movementStartDistance;
    private bool isCameraMovingAway, isCameraMovingToward;
    private Vector3 m_Velocity;

    // Start is called before the first frame update
    void Start()
    {
        if (targetTransform == null)
            Debug.LogWarning("No target specified for camera!");

        m_Transform = transform;
        x = 0.0f;
        y = 0.0f;
        currentDistance = idleDistance;
        isCameraMovingAway = false;
        isCameraMovingToward = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private void LateUpdate()
    {
        x += Input.GetAxis("Mouse X") * xSpeed * -0.02f;
        y += Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        y = ClampAngle(y, yMinLimit, yMaxLimit);

        float currentVelocity = targetTransform.parent.gameObject.GetComponent<PhysicsManager>().GetVelocity().magnitude;
        UpdateDistance();
        SphericalToCartesian(currentDistance, x, y, out localPosition);

        m_Transform.position = targetTransform.position + localPosition;
        m_Transform.LookAt(targetTransform);
    }

    public void StartMovementAway()
    {
        isCameraMovingAway = true;
        isCameraMovingToward = false;
        movementStartTime = Time.time;
        movementStartDistance = currentDistance;
    }

    public void StartMovementToward()
    {
        isCameraMovingToward = true;
        isCameraMovingAway = false;
        movementStartTime = Time.time;
        movementStartDistance = currentDistance;
    }

    private void UpdateDistance()
    {
        if(isCameraMovingAway && Time.time <= movementStartTime + dampingTime)
        {
            float t = (Time.time - movementStartTime) / dampingTime;
            currentDistance = Mathf.SmoothStep(movementStartDistance, flyDistance, t);
        }
        else if (isCameraMovingAway && Time.time > movementStartTime + dampingTime)
        {
            isCameraMovingAway = false;
        }

        if (isCameraMovingToward && Time.time <= movementStartTime + dampingTime)
        {
            float t = (Time.time - movementStartTime) / dampingTime;
            currentDistance = Mathf.SmoothStep(movementStartDistance, idleDistance, t);
        }
        else if (isCameraMovingToward && Time.time > movementStartTime + dampingTime)
        {
            isCameraMovingToward = false;
        }
    }

    private static void SphericalToCartesian(float radius, float polar, float elevation, out Vector3 outCart)
    {
        float a = radius * Mathf.Cos(elevation);
        outCart.x = a * Mathf.Cos(polar);
        outCart.y = radius * Mathf.Sin(elevation);
        outCart.z = a * Mathf.Sin(polar);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle <= 360.0f)
            angle += 360.0f;
        if (angle > 360.0f)
            angle -= 360.0f;
        return Mathf.Clamp(angle, min, max);
    }

}
