#pragma once
# include "Enum.h" 
struct ClassTestBattle
{
	String name = U"";
	String map = U"";
	Array<String> memberKougeki;
	Array<String> memberBouei;
	BattleWhichIsThePlayer player = BattleWhichIsThePlayer::None;
};
