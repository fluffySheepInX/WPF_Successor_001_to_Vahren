#pragma once
# include "Enum.h" 

class MapDetail
{
public:
	String tip;
	Array<HashTable<String, long>> building;
	FlagBattleMapUnit flagBattleMapUnit;
	String unit;
	String houkou;
	String zinkei;
	bool kougekiButaiNoIti;
	bool boueiButaiNoIti;
	//s3d::Optional<s3d::Path> mapPath;
};

class ClassMapBattle
{
public:
	String name;
	//std::string tagName;
	Array<Array<MapDetail>> mapData;
	s3d::Optional<s3d::Point> frontPosi;
	s3d::Optional<s3d::Point> defensePosi;
	s3d::Optional<s3d::Point> neutralPosi;
};
