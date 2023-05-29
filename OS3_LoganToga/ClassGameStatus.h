#pragma once
# include "ClassMap.h" 
# include "ClassTestBattle.h" 
# include "ClassUnit.h" 

class ClassGameStatus
{
public:
	Array<ClassMap> arrayClassMap;
	ClassTestBattle classTestBattle;
	Array<ClassUnit> arrayClassUnit;
	Array<ClassSkill> arrayClassSkill;
};
