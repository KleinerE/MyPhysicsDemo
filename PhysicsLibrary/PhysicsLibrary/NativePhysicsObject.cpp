#include "pch.h"
#include "NativePhysicsObject.h"
#include <iterator>
#include <algorithm>
#include <exception>


#define G -9.81
#define VELOCITY_TRESHOLD 0.1



NativePhysicsObject::NativePhysicsObject(double position[3], double mass)
	: _position{ position[0], position[1], position[2] },
	_velocity{ 0.0, 0.0, 0.0 },
	_elasticity(0.7),
	_mass(mass)
{
	//For Debugging
#ifdef _DEBUG
	FILE* pConsole;
	AllocConsole();
	freopen_s(&pConsole, "CONOUT$", "wb", stdout);
	DPRINT("Created Native Physics Object at: " << position[0] << ", " << position[1] << ", " << position[2]);
#endif // 

}

NativePhysicsObject::~NativePhysicsObject()
{
}

void NativePhysicsObject::OnUpdate(double dt)
{
	UpdateVelocity(dt);
	UpdatePosition(dt);
}

void NativePhysicsObject::AddOtherObject(NativePhysicsObject* obj)
{
	PTR_ASSERT(obj);
	_knownObjects.push_back(obj);
}

NativePhysicsObject* NativePhysicsObject::CheckAllObjectCollisions()
{
	for (auto i = _knownObjects.begin(); i != _knownObjects.end(); i++)
	{
		if (CheckCollisionWithObject(*i))
		{
			return *i;
		}	
	}
	return nullptr;
}

void NativePhysicsObject::UpdatePosition(double dt)
{
	_position += _velocity * dt;
}

void NativePhysicsObject::UpdateVelocity(double dt)
{
	_velocity += _acceleration * dt;
}


/*
void NativePhysicsObject::StartMovementVertical(double v0y)
{
	_velocity.y = v0y;
	_isMovingVertical = true;	
}

void NativePhysicsObject::StopMovementVertical()
{
	_velocity.y = 0;
	_isMovingVertical = false;
}

void NativePhysicsObject::StartMovementHorizontal(double v0x, double v0z)
{
	_velocity.x = v0x;
	_velocity.z = v0z;
	_isMovingHorizontal = true;
}

void NativePhysicsObject::StopMovementHorizontal()
{
	_velocity.x = 0;
	_velocity.z = 0;
	_isMovingHorizontal = false;
}

//assuming starting above floor and moving downwards.
//bool NativePhysicsObject::CheckCollisionVertical(double floor_y)
//{
//	return (_position.y <= floor_y + _radius) && _velocity.y <= 0;
//}
//
//bool NativePhysicsObject::CheckCollisionHorizontal(NativePhysicsObject* otherObj)
//{
//	double3 otherCenter = otherObj->GetObjectPosition();
//	double otherRadius = otherObj->GetObjectRadius();
//	double distance = linalg::distance(_position, otherCenter);
//	return distance <= _radius + otherRadius;
//}
*/

/*
void NativePhysicsObject::CalcVelocityVertical(double dt)
{
	_velocity.y += _gravity_constant * dt;
}

void NativePhysicsObject::CalcVelocityHorizontal(double dt)
{
}
*/
/*
void NativePhysicsObject::CalcCollisionVelocities(NativePhysicsObject* otherObj)
{
	double3 otherCenter = otherObj->GetObjectPosition();
	double3 otherVelocity = otherObj->GetObjectVelocity();
	double otherRadius = otherObj->GetObjectRadius();
	double otherMass = 1.0;
	double ce = 1.0;

	double3 vab = _velocity - otherVelocity;
	double3 normal = linalg::normalize(otherCenter - _position);
	double impulse = -((1.0+ce) * linalg::dot(vab, normal)) / ((1.0/_mass)+(1.0/otherMass));

	double3 vaf = _velocity + impulse / _mass * normal;
	double3 vbf = otherVelocity - impulse / otherMass * normal;
	StartMovementHorizontal(vaf.x, vaf.z);
	otherObj->StartMovementHorizontal(vbf.x, vbf.z);

}
*/
/*
void NativePhysicsObject::GetObjectPosition(double position[])
{
	std::copy(linalg::begin(_position), linalg::end(_position), position);
}
*/
/*
void NativePhysicsObject::SetVelocity(double vx, double vy, double vz)
{
	std::cout << "Setting Velocity to " << vx << " " << vx << " " << vx << std::endl;
	_mass = vx;// = double3(vx, vy, vz);
}
*/
/***********************************************************/

NativePhysicsObject* ExtCreateNativePhysicsObject(double position[3], double mass)
{
	return new NativePhysicsObject(position, mass);
}

void ExtDestroyNativePhysicsObject(NativePhysicsObject* obj)
{
	PTR_ASSERT(obj);
	delete obj;
}

void ExtOnUpdate(NativePhysicsObject* obj, double dt)
{
	PTR_ASSERT(obj);
	obj->OnUpdate(dt);
}

void ExtGetPosition(NativePhysicsObject* obj, double out_arr[])
{
	PTR_ASSERT(obj);
	double3 pos = obj->GetPosition();
	std::copy(linalg::begin(pos), linalg::end(pos), out_arr);
}

void ExtSetPosition(NativePhysicsObject* obj, double in_arr[3])
{
	PTR_ASSERT(obj);
	obj->SetPosition(double3(in_arr[0], in_arr[1], in_arr[2]));
}

void ExtGetVelocity(NativePhysicsObject* obj, double out_arr[])
{
	PTR_ASSERT(obj);
	double3 vel = obj->GetVelocity();
	std::copy(linalg::begin(vel), linalg::end(vel), out_arr);
}

void ExtSetVelocity(NativePhysicsObject* obj, double in_arr[3])
{
	PTR_ASSERT(obj);
	obj->SetVelocity(double3(in_arr[0], in_arr[1], in_arr[2]));
}

void ExtGetAcceleration(NativePhysicsObject* obj, double out_arr[])
{
	PTR_ASSERT(obj);
	double3 acc = obj->GetAcceleration();
	std::copy(linalg::begin(acc), linalg::end(acc), out_arr);
}

void ExtSetAcceleration(NativePhysicsObject* obj, double in_arr[3])
{
	PTR_ASSERT(obj);
	obj->SetAcceleration(double3(in_arr[0], in_arr[1], in_arr[2]));
}

NativePhysicsObject* ExtCheckAllObjectCollisions(NativePhysicsObject* obj)
{
	PTR_ASSERT(obj);
	return obj->CheckAllObjectCollisions();
}

void ExtAddOtherObject(NativePhysicsObject* obj, NativePhysicsObject* otherObj)
{
	PTR_ASSERT(obj);
	PTR_ASSERT(otherObj);
	obj->AddOtherObject(otherObj);
}

void ExtExecuteCollisionWithObject(NativePhysicsObject* obj, NativePhysicsObject* otherObj, double out_arr[])
{
	PTR_ASSERT(obj);
	PTR_ASSERT(otherObj);
	double3x3 out = obj->ExecuteCollisionWithObject(otherObj);
	std::copy(linalg::begin(out[0]), linalg::end(out[2]), out_arr);
}
bool ExtCheckCollisionWithFloor(NativePhysicsObject* obj, double floor_y)
{
	PTR_ASSERT(obj);
	return obj->CheckCollisionWithFloor(floor_y);
}