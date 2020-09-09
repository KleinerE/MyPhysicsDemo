#include "pch.h"
#include "NativePhysicsSphere.h"


NativePhysicsSphere::NativePhysicsSphere(double position[3], double radius, double mass)
	: NativePhysicsObject{ position,	mass},
		_radius(radius)
{
}

NativePhysicsSphere::~NativePhysicsSphere()
{
}

bool NativePhysicsSphere::CheckCollisionWithFloor(double floor_y)
{
	return (_position.y <= floor_y + _radius) && _velocity.y <= 0;
}

bool NativePhysicsSphere::CheckCollisionWithObject(NativePhysicsObject* otherObj)
{
	// This assumes the other object is also a sphere.
	double3 otherCenter = otherObj->GetPosition();
	double otherRadius = otherObj->GetRadius();
	double distance = linalg::distance(_position, otherCenter);
	return distance <= _radius + otherRadius;
}

double3x3 NativePhysicsSphere::ExecuteCollisionWithObject(NativePhysicsObject* otherObj)
{
	// This assumes the other object is also a sphere.
	double3 otherCenter = otherObj->GetPosition();
	double3 otherVelocity = otherObj->GetVelocity();
	double otherRadius = otherObj->GetRadius();
	double otherMass = otherObj->GetMass();
	double ce = 1.0;	// collision elasticity

	double3 vab = _velocity - otherVelocity;
	double3 normal = linalg::normalize(otherCenter - _position);
	double impulse = -((1.0 + ce) * linalg::dot(vab, normal)) / ((1.0 / _mass) + (1.0 / otherMass));

	double3 col_position = _position + normal * _radius;
	double3 vaf = _velocity + impulse / _mass * normal;
	double3 vbf = otherVelocity - impulse / otherMass * normal;
	return double3x3{ col_position, vaf, vbf };
}


void NativePhysicsSphere::AddOtherSphere(NativePhysicsSphere* obj)
{
	PTR_ASSERT(obj);
	_knownObjects.push_back(obj);
}

/**********************************************************************************/

NativePhysicsSphere* ExtCreateNativePhysicsSphere(double position[3], double radius, double mass)
{
	return new NativePhysicsSphere(position, radius, mass);
}

void ExtDestroyNativePhysicsSphere(NativePhysicsSphere* obj)
{
	delete obj;
}