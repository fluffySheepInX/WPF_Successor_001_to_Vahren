#pragma once
# include "ClassUnit.h" 

class ClassSkill
{
public:
	s3d::String nameTag;
	s3d::String name;
	s3d::String help;
	s3d::String image;
	s3d::Array<s3d::String> icon;
	std::pair<s3d::String, int32> fkey;
	int32 sortKey = 0;
	double mp = 0.0;
	double slowPer = 0.75;
	int32 slowTime = 1;
	s3d::Array<s3d::String> sound;
	s3d::String direct;
	int32 w = 0;
	int32 h = 0;
	int32 a = 0;
	s3d::String forceFire;
	s3d::String attr;
	std::pair<s3d::String, int32> str;
	int32 range = 0;
	int32 damageRangeAdjust = 0;
	int32 rangeMin = 0;
	int32 speed = 0;
	std::pair<s3d::String, int32> gunDelay;
	s3d::String pairNext;
	s3d::String next;
	int32 randomSpace = 0;
	s3d::Array<s3d::String> offset;
	s3d::Array<int32> ray;
	s3d::String forceRay;
	int32 rush = -1;
	int32 rushInterval = -1;
	int32 rushRandomDegree = -1;

	float radian = 0;
	float degree = 0;
};
