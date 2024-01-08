#pragma once
# include "Enum.h" 
class ClassObjectMapTip
{
public:
	String nameTag;
	MapTipObjectType type = MapTipObjectType::None;
	/// @brief wall2で攻撃側飛び道具がすり抜ける確率
	int noWall2 = 0;
	int castle = 0;
	int castleDefense = 0;
	int castleMagdef = 0;
};
