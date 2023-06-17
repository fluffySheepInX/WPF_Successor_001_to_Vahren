#pragma once
# include "ClassMap.h" 
# include "ClassTestBattle.h" 
# include "ClassUnit.h" 
# include "ClassPower.h" 
# include "ClassBattle.h" 
# include "ClassObjectMapTip.h" 

class ClassGameStatus
{
public:
	long getIDCount() {
		long re = iDCount;
		iDCount++;
		return re;
	}

	Array<ClassPower> arrayClassPower;
	Array<ClassMap> arrayClassMap;
	Array<ClassObjectMapTip> arrayClassObjectMapTip;
	ClassTestBattle classTestBattle;
	ClassBattle classBattle;
	Array<ClassUnit> arrayClassUnit;
	Array<ClassSkill> arrayClassSkill;
	bool IsBattleMove = false;
	int32 DistanceBetweenUnit = 0;
	int32 DistanceBetweenUnitTate = 0;
private:
	long iDCount = 0;
};
