#pragma once
# include "ClassUnit.h" 
# include "ClassSkill.h"

class ClassBullets
{
public:
	int32 No;
	Vec2 NowPosition;
	Vec2 StartPosition;
	Vec2 OrderPosition;
	Vec2 MoveVec;
	/// @brief 寿命
	double duration;
	/// @brief 現生存時間
	double lifeTime;

	float radian;
	float degree;
};

class ClassExecuteSkills
{
public:
	int32 No;
	ClassUnit* classUnit;
	ClassSkill classSkill;
	Array<ClassBullets> ArrayClassBullet;
};
