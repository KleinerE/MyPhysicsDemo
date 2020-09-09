#pragma once
#include "NativePhysicsObject.h"

#ifdef NATIVEPHYSICSCUBE_EXPORTS
//#ifdef PHYSICSLIBRARY_EXPORTS
#define MY_API extern "C" __declspec(dllexport)
#else
#define MY_API extern "C" __declspec(dllimport)
#endif


class NativePhysicsCube :
	public NativePhysicsObject
{
public:
	NativePhysicsCube(double position[3], double edge, double mass);
	~NativePhysicsCube();

	inline double GetEdge() override { return _edge; };
	inline void SetEdge(double edge) override { _edge = edge; };


	void AddOtherCube(NativePhysicsCube* obj);
	bool CheckCollisionWithObject(NativePhysicsObject* otherObj) override;
	//double3x3 ExecuteCollisionWithObject(NativePhysicsObject* otherObj) override;

	bool CheckCollisionWithFloor(double floor_y) override;

private:
	double _edge;
	std::vector<NativePhysicsCube*> _knownObjects;
};

MY_API NativePhysicsCube* ExtCreateNativePhysicsCube(double position[3], double edge, double mass);
MY_API void ExtDestroyNativePhysicsCube(NativePhysicsCube* obj);
