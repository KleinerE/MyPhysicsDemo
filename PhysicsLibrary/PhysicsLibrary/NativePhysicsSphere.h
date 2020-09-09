#pragma once
#include "NativePhysicsObject.h"


#ifdef NATIVEPHYSICSSPHERE_EXPORTS
//#ifdef PHYSICSLIBRARY_EXPORTS
#define MY_API extern "C" __declspec(dllexport)
#else
#define MY_API extern "C" __declspec(dllimport)
#endif

class NativePhysicsSphere :
	public NativePhysicsObject
{
public:
	NativePhysicsSphere(double position[3], double radius, double mass);
	~NativePhysicsSphere();

	inline double GetRadius() override { return _radius; };
	inline void SetRadius(double radius) override { _radius = radius;  };

	
	void AddOtherSphere(NativePhysicsSphere* obj);
	bool CheckCollisionWithObject(NativePhysicsObject* otherObj) override;
	double3x3 ExecuteCollisionWithObject(NativePhysicsObject* otherObj) override;

	bool CheckCollisionWithFloor(double floor_y) override;
	//NativePhysicsSphere* CheckAllObjectCollisions();

private:
	double _radius;
	std::vector<NativePhysicsSphere*> _knownObjects;
};

MY_API NativePhysicsSphere* ExtCreateNativePhysicsSphere(double position[3], double radius, double mass);
MY_API void ExtDestroyNativePhysicsSphere(NativePhysicsSphere* obj);