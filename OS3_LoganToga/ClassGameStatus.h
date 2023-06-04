#pragma once
# include "ClassMap.h" 
# include "ClassTestBattle.h" 
# include "ClassUnit.h" 
# include "ClassBattle.h" 

class ClassGameStatus
{
public:
	Array<ClassMap> arrayClassMap;
	ClassTestBattle classTestBattle;
	ClassBattle classBattle;
	Array<ClassUnit> arrayClassUnit;
	Array<ClassSkill> arrayClassSkill;

};
