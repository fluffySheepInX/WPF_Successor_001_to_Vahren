#pragma once
# include "156_EnumBattleWhichIsThePlayer.h" 
# include "157_EnumFlagBattleMapUnit.h" 

class MapDetail
{
public:
	String tip;
	/// @brief 種別、ClassUnitID、陣営
	Array<std::tuple<String, long, BattleWhichIsThePlayer>> building;
	FlagBattleMapUnit flagBattleMapUnit;
	String unit;
	String houkou;
	String zinkei;
	String posSpecial;
	bool kougekiButaiNoIti = false;
	bool boueiButaiNoIti = false;
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
