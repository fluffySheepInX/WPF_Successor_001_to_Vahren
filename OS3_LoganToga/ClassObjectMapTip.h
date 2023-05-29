#pragma once
# include "Enum.h" 
class ClassObjectMapTip
{
public:
	s3d::String nameTag;
	MapTipObjectType type = MapTipObjectType::None;
	int noWall2 = 0;
	int castle = 0;
	int castleDefense = 0;
	int castleMagdef = 0;
};
