#pragma once
# include "230_ClassUnit.h" 
# include "240_ClassSkill.h"

class ClassBullets
{
public:
	int32 No;
	int32 RushNo;
	Vec2 NowPosition;
	Vec2 StartPosition;
	Vec2 OrderPosition;
	Vec2 MoveVec;
	/// @brief 寿命
	double duration;
	/// @brief 現生存時間
	double lifeTime;
	Stopwatch stopwatch{ StartImmediately::Yes };

	float radian;
	float degree;
	float initDegree;
	///// @brief 累積時間
	//double AccumulationTime = 0;
	Optional<RectF> rectF;
};

class ClassExecuteSkills
{
public:
	int32 No;
	int32 UnitID;
	ClassUnit* classUnit;
	ClassSkill classSkill;
	Array<ClassBullets> ArrayClassBullet;
};
