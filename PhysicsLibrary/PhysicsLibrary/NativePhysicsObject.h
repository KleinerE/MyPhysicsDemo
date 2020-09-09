#pragma once
#include <iostream>
#include <vector>
#include "Linalg.h"
using namespace linalg::aliases;

//#define MYAPI __declspec(dllexport)

#ifdef NATIVEPHYSICSOBJECT_EXPORTS
//#ifdef PHYSICSLIBRARY_EXPORTS
#define MY_API extern "C" __declspec(dllexport)
#else
#define MY_API extern "C" __declspec(dllimport)
#endif

#ifdef _DEBUG
#define DPRINT(x) std::cout << x << std::endl
#else
#define DPRINT(x)
#endif

#define PTR_ASSERT(x)	if ((x) == NULL) { DPRINT("Invalid Pointer! at: " << __FILE__ << "(" << __LINE__ << "): " << #x);}

class NativePhysicsObject
{
public:
	NativePhysicsObject(double position[3], double mass); //TODO: Pass world parameters - global acceleration, boundaries.
	~NativePhysicsObject();
		
	void OnUpdate(double dt);

	inline double3 GetPosition() { return _position; };
	inline void SetPosition(double3 position) { _position = position; };

	inline double3 GetVelocity() { return _velocity; };
	inline void SetVelocity(double3 velocity) { _velocity = velocity; };

	inline double3 GetAcceleration() { return _acceleration; };
	inline void SetAcceleration(double3 acceleration) { _acceleration = acceleration; };

	inline double GetMass() { return _mass; };
	inline void SetMass(double mass) { _mass = mass; };

	inline virtual double GetRadius() { return 0.0; };
	inline virtual void SetRadius(double radius) { };

	inline virtual double GetEdge() { return 0.0; };
	inline virtual void SetEdge(double edge) { };

	void AddOtherObject(NativePhysicsObject* obj);
	NativePhysicsObject* CheckAllObjectCollisions();
	virtual double3x3 ExecuteCollisionWithObject(NativePhysicsObject* otherObj) { return double3x3{}; };

	virtual bool CheckCollisionWithFloor(double floor_y) { return false; };

protected:
	void UpdatePosition(double dt);
	void UpdateVelocity(double dt);

	virtual bool CheckCollisionWithObject(NativePhysicsObject* otherObj) { return false; };
	

	double _elasticity;
	double _mass;
	
	double3 _position;
	double3 _velocity;
	double3 _acceleration;

	std::vector<NativePhysicsObject*> _knownObjects;

};

MY_API NativePhysicsObject* ExtCreateNativePhysicsObject(double position[3], double mass);
MY_API void ExtDestroyNativePhysicsObject(NativePhysicsObject* obj);
MY_API void ExtOnUpdate(NativePhysicsObject* obj, double dt);

MY_API void ExtGetPosition(NativePhysicsObject* obj, double out_arr[]);
MY_API void ExtSetPosition(NativePhysicsObject* obj, double in_arr[3]);
MY_API void ExtGetVelocity(NativePhysicsObject* obj, double out_arr[]);
MY_API void ExtSetVelocity(NativePhysicsObject* obj, double in_arr[3]);
MY_API void ExtGetAcceleration(NativePhysicsObject* obj, double out_arr[]);
MY_API void ExtSetAcceleration(NativePhysicsObject* obj, double in_arr[3]);

MY_API NativePhysicsObject* ExtCheckAllObjectCollisions(NativePhysicsObject* obj);
MY_API void ExtAddOtherObject(NativePhysicsObject* obj, NativePhysicsObject* otherObj);
MY_API void ExtExecuteCollisionWithObject(NativePhysicsObject* obj, NativePhysicsObject* otherObj, double out_arr[]);
MY_API bool ExtCheckCollisionWithFloor(NativePhysicsObject* obj, double floor_y);
