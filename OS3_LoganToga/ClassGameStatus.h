#pragma once
# include "ClassMap.h" 
# include "ClassTestBattle.h" 
# include "ClassUnit.h" 
# include "ClassPower.h" 
# include "ClassBattle.h" 

class ClassGameStatus
{
public:
	Array<ClassPower> arrayClassPower;
	Array<ClassMap> arrayClassMap;
	ClassTestBattle classTestBattle;
	ClassBattle classBattle;
	Array<ClassUnit> arrayClassUnit;
	Array<ClassSkill> arrayClassSkill;
	bool IsBattleMove = false;
	int32 DistanceBetweenUnit = 0;
	int32 DistanceBetweenUnitTate = 0;
};
