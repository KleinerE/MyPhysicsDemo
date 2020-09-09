#include "pch.h"
#include "NativePhysicsCube.h"

NativePhysicsCube::NativePhysicsCube(double position[3], double edge, double mass)
	: NativePhysicsObject{ position,	mass },
	_edge(edge)
{
}

NativePhysicsCube::~NativePhysicsCube()
{
}

bool NativePhysicsCube::CheckCollisionWithFloor(double floor_y)
{
	return (_position.y <= floor_y + _edge * 0.5) && _velocity.y <= 0;
}

bool NativePhysicsCube::CheckCollisionWithObject(NativePhysicsObject* otherObj)
{
	// This assumes the other object is also a cube.
	double3 otherCenter = otherObj->GetPosition();
	double otherEdge = otherObj->GetEdge();
	bool x_overlap = abs(otherCenter.x - _position.x) <= (otherEdge + _edge) * 0.5;
	bool y_overlap = abs(otherCenter.y - _position.y) <= (otherEdge + _edge) * 0.5;
	bool z_overlap = abs(otherCenter.z - _position.z) <= (otherEdge + _edge) * 0.5;
	return x_overlap && y_overlap && z_overlap;
}

void NativePhysicsCube::AddOtherCube(NativePhysicsCube* obj)
{
	PTR_ASSERT(obj);
	_knownObjects.push_back(obj);
}

/**********************************************************************************/

NativePhysicsCube* ExtCreateNativePhysicsCube(double position[3], double edge, double mass)
{
	return new NativePhysicsCube(position, edge, mass);
}

void ExtDestroyNativePhysicsCube(NativePhysicsCube* obj)
{
	delete obj;
}