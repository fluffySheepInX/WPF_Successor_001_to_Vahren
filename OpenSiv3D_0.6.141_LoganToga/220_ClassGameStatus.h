#pragma once
# include "235_ClassMap.h" 
# include "250_ClassTestBattle.h" 
# include "230_ClassUnit.h" 
# include "215_ClassPower.h" 
# include "255_ClassBattle.h" 
# include "245_ClassObjectMapTip.h" 
# include "225_ClassCard.h" 

class ClassGameStatus
{
public:
	long getIDCount() {
		long re = iDCount;
		iDCount++;
		return re;
	}
	long getBattleIDCount() {
		long re = battleIDCount;
		battleIDCount++;
		return re;
	}
	long getDeleteCESIDCount() {
		long re = deleteCESIDCount;
		deleteCESIDCount++;
		return re;
	}

	HashTable<int64, Array<Point>> aiRoot;
	Array<ClassPower> arrayClassPower;
	String nowPowerTag;
	Array<ClassMap> arrayClassMap;
	Array<ClassObjectMapTip> arrayClassObjectMapTip;
	ClassTestBattle classTestBattle;
	ClassBattle classBattle;
	Array<ClassUnit> arrayClassUnit;
	Array<ClassHorizontalUnit> arrayPlayerUnit;
	Array<ClassSkill> arrayClassSkill;
	Array<ClassCard> arrayClassCard;
	bool IsBattleMove = false;
	int32 DistanceBetweenUnit = 0;
	int32 DistanceBetweenUnitTate = 0;

	Array<String> arrayInfoProcessSelectCharaMap;
	Array<String> arrayInfoProcessSelectCharaEnemyUnit;

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
	long battleIDCount = 0;
	long deleteCESIDCount = 0;
};
