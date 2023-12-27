#pragma once
# include "ClassMapBattle.h" 
# include "ClassHorizontalUnit.h" 
# include "Enum.h" 

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
