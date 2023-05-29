#pragma once
class ClassSkill
{
public:
	s3d::String nameTag;
	s3d::String name;
	s3d::String help;
	s3d::String image;
	s3d::Array<s3d::String> icon;
	std::pair<s3d::String, int> fkey;
	int sortKey = 0;
	double mp = 0.0;
	double slowPer = 0.75;
	int slowTime = 1;
	s3d::Array<s3d::String> sound;
	s3d::String direct;
	int w = 0;
	int h = 0;
	int a = 0;
	s3d::String forceFire;
	s3d::String attr;
	std::pair<s3d::String, int> str;
	int range = 0;
	int damageRangeAdjust = 0;
	int rangeMin = 0;
	int speed = 0;
	std::pair<s3d::String, int> gunDelay;
	s3d::String pairNext;
	s3d::String next;
	int randomSpace = 0;
	s3d::Array<s3d::String> offset;
	s3d::Array<int> ray;
	s3d::String forceRay;
	int rush = -1;
	int rushInterval = -1;
	int rushRandomDegree = -1;
};
