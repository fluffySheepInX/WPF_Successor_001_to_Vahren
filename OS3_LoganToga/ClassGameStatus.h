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
	Array<ClassHorizontalUnit> arrayPlayerUnit;
	Array<ClassSkill> arrayClassSkill;
	bool IsBattleMove = false;
	int32 DistanceBetweenUnit = 0;
	int32 DistanceBetweenUnitTate = 0;
	bool strategyMenu000 = true;
	bool strategyMenu001 = true;
	bool strategyMenu002 = true;
	bool strategyMenu003 = true;
	bool strategyMenu004 = true;
	bool strategyMenu005 = true;
	bool strategyMenu006 = true;
	bool strategyMenu007 = true;
	bool strategyMenu008 = true;
	bool strategyMenu009 = true;
private:
	long iDCount = 0;
};
