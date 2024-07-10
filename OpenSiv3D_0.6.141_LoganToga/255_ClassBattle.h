#pragma once
# include "260_ClassMapBattle.h" 
# include "265_ClassHorizontalUnit.h" 
# include "156_EnumBattleWhichIsThePlayer.h" 

class ClassBattle
{
public:
	String battleSpot;
	String sortieSpot;
	String attackPower;
	String defensePower;
	Array<ClassHorizontalUnit> sortieUnitGroup;
	Array<ClassHorizontalUnit> defUnitGroup;
	Array<ClassHorizontalUnit> neutralUnitGroup;
	std::optional<ClassMapBattle> classMapBattle;
	//std::vector<Rect> listBuildingAlive;
	BattleWhichIsThePlayer battleWhichIsThePlayer;
};
