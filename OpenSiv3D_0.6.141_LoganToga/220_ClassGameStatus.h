#pragma once
# include "235_ClassMap.h" 
# include "250_ClassTestBattle.h" 
# include "230_ClassUnit.h" 
# include "215_ClassPower.h" 
# include "255_ClassBattle.h" 
# include "245_ClassObjectMapTip.h" 
# include "225_ClassCard.h"
# include "231_ClassEnemy.h" 
#include <bitset>

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
	Array<ClassEnemy> arrayClassEnemy;
	Array<ClassHorizontalUnit> arrayPlayerUnit;
	Array<ClassSkill> arrayClassSkill;
	Array<ClassCard> arrayClassCard;
	bool IsBattleMove = false;
	int32 DistanceBetweenUnit = 0;
	int32 DistanceBetweenUnitTate = 0;

	Array<String> arrayInfoProcessSelectCharaMap;
	Array<String> arrayInfoProcessSelectCharaEnemyUnit;

	inline static constexpr uint8 NumMenus = 10;
	std::bitset<NumMenus> strategyMenus = std::bitset<NumMenus>("1111111111");;
private:
	long iDCount = 0;
	long battleIDCount = 0;
	long deleteCESIDCount = 0;
};
