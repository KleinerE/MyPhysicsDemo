using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public struct CollisionInformation
{
    public Vector3 position;
    public Vector3 velocity_own;
    public Vector3 velocity_other;
}

public class PhysicsLibFacade
{
    /********************************  DLL Imports  *****************************************/

    [DllImport("PhysicsLibrary")]
    private static extern IntPtr ExtCreateNativePhysicsObject(double[] position, double mass);
    [DllImport("PhysicsLibrary")]
    private static extern IntPtr ExtCreateNativePhysicsSphere(double[] position, double radius, double mass);
    [DllImport("PhysicsLibrary")]
    private static extern IntPtr ExtCreateNativePhysicsCube(double[] position, double edge, double mass);
    [DllImport("PhysicsLibrary")]
    private static extern void ExtDestroyNativePhysicsObject(IntPtr obj);
    [DllImport("PhysicsLibrary")]
    private static extern void ExtOnUpdate(IntPtr obj, double dt);
    [DllImport("PhysicsLibrary")]
    private static extern void ExtGetPosition(IntPtr obj, double[] position);
    [DllImport("PhysicsLibrary")]
    private static extern void ExtSetPosition(IntPtr obj, double[] position);
    [DllImport("PhysicsLibrary")]
    private static extern void ExtGetVelocity(IntPtr obj, double[] velocity);
    [DllImport("PhysicsLibrary")]
    private static extern void ExtSetVelocity(IntPtr obj, double[] velocity);
    [DllImport("PhysicsLibrary")]
    private static extern void ExtSetAcceleration(IntPtr obj, double[] acceleration);
    [DllImport("PhysicsLibrary")]
    private static extern void ExtAddOtherObject(IntPtr obj, IntPtr otherObj);
    [DllImport("PhysicsLibrary")]
    private static extern IntPtr ExtCheckAllObjectCollisions(IntPtr obj);
    [DllImport("PhysicsLibrary")]
    private static extern void ExtExecuteCollisionWithObject(IntPtr obj, IntPtr otherObj, double[] arr);
    [DllImport("PhysicsLibrary")]
    private static extern bool ExtCheckCollisionWithFloor(IntPtr obj, double floor_y);


    /********************************  Class Methods *****************************************/

    public PhysicsLibFacade()
    {

    }

    private void OnInvalidPtr()
    {
        Debug.LogError("Error calling native method: Invalid pointer provided. Call to native library stopped.");
    }

    /********************************  Method Wrappers  *****************************************/

    public IntPtr CreateNativePhysicsObject(Vector3 position, double mass)
    {
        double[] pos = new double[3] { (double)position.x, (double)position.y, (double)position.z };
        IntPtr p = ExtCreateNativePhysicsObject(pos, mass);
        if (p == IntPtr.Zero)
        {
            Debug.LogError("Error creating native physics sphere!");
        }
        return p;
    }

    public IntPtr CreateNativePhysicsSphere(Vector3 position, double radius, double mass)
    {
        double[] pos = new double[3] { (double)position.x, (double)position.y, (double)position.z };
        IntPtr p = ExtCreateNativePhysicsSphere(pos, radius, mass);
        if(p == IntPtr.Zero)
        {
            Debug.LogError("Error creating native physics sphere!");
        }
        return p;
    }

    public IntPtr CreateNativePhysicsCube(Vector3 position, double edge, double mass)
    {
        double[] pos = new double[3] { (double)position.x, (double)position.y, (double)position.z };
        IntPtr p = ExtCreateNativePhysicsCube(pos, edge, mass);
        if (p == IntPtr.Zero)
        {
            Debug.LogError("Error creating native physics cube!");
        }
        return p;
    }

    public void DestroyNativePhysicsObject(IntPtr obj)
    {
        if(obj == IntPtr.Zero)
        {
            OnInvalidPtr();
        }
        else
        {
            ExtDestroyNativePhysicsObject(obj);
        }
    }

    public void OnUpdate(IntPtr obj, double dt)
    {
        if (obj == IntPtr.Zero)
        {
            OnInvalidPtr();
        }
        else
        {
            ExtOnUpdate(obj, dt);
        }
    }

    public Vector3 GetPosition(IntPtr obj)
    {
        if (obj == IntPtr.Zero)
        {
            OnInvalidPtr();    
        }
        else
        {
            double[] pos = new double[3];
            ExtGetPosition(obj, pos);
            return new Vector3((float)pos[0], (float)pos[1], (float)pos[2]);
        }
        return new Vector3 { };
    }
    public void SetPosition(IntPtr obj, Vector3 position)
    {
        if (obj == IntPtr.Zero)
        {
            OnInvalidPtr();
        }
        else
        {
            double[] pos = new double[3] { (double)position.x, (double)position.y, (double)position.z };
            ExtSetPosition(obj, pos);
        }
    }

    public Vector3 GetVelocity(IntPtr obj)
    {
        if (obj == IntPtr.Zero)
        {
            OnInvalidPtr();
        }
        else
        {
            double[] vel = new double[3];
            ExtGetVelocity(obj, vel);
            return new Vector3((float)vel[0], (float)vel[1], (float)vel[2]);
        }
        return new Vector3 { };
    }

    public void SetVelocity(IntPtr obj, Vector3 velocity)
    {
        if (obj == IntPtr.Zero)
        {
            OnInvalidPtr();
        }
        else
        {
            double[] vel = new double[3] { (double)velocity.x, (double)velocity.y, (double)velocity.z };
            ExtSetVelocity(obj, vel);
        }       
    }

    public void SetAcceleration(IntPtr obj, Vector3 acceleration)
    {
        if (obj == IntPtr.Zero)
        {
            OnInvalidPtr();
        }
        else
        {
            double[] acc = new double[3] { (double)acceleration.x, (double)acceleration.y, (double)acceleration.z };
            ExtSetAcceleration(obj, acc);
        }
    }

    public IntPtr CheckAllObjectCollisions(IntPtr obj)
    {
        return ExtCheckAllObjectCollisions(obj);
    }

    public void AddOtherObject(IntPtr obj, IntPtr otherObj)
    {
        if (obj == IntPtr.Zero || otherObj == IntPtr.Zero)
        {
            OnInvalidPtr();
        }
        
        else
        {
            ExtAddOtherObject(obj, otherObj);
        }
    }

    public CollisionInformation ExecuteCollisionWithObject(IntPtr obj, IntPtr otherObj)
    {
        if (obj == IntPtr.Zero || otherObj == IntPtr.Zero)
        {
            OnInvalidPtr();
            return new CollisionInformation();
        }
        else
        {
            double[] arr = new double[9];
            ExtExecuteCollisionWithObject(obj, otherObj, arr);
            Vector3 col_pos = new Vector3((float)arr[0], (float)arr[1], (float)arr[2]);
            Vector3 own_vel = new Vector3((float)arr[3], (float)arr[4], (float)arr[5]);
            Vector3 other_vel = new Vector3((float)arr[6], (float)arr[7], (float)arr[8]);
            return new CollisionInformation
            {
                position = col_pos,
                velocity_own = own_vel,
                velocity_other = other_vel
            };
        }
    }

    public bool CheckCollisionWithFloor(IntPtr obj, double floor_y)
    {
        if (obj == IntPtr.Zero)
        {
            OnInvalidPtr();
        }
        else
        {
            return ExtCheckCollisionWithFloor(obj, floor_y);
        }
        return false;
    }
    
}
