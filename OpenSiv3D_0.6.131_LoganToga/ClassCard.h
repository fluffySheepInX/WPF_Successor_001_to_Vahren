#pragma once
class ClassCard
{
public:
	String nameTag;
	int32 sortKey;
	String func;
	Array<String> icon;
	String name;
	String help;

	int32 attackMyUnit;
	int32 defMyUnit;
	int32 moveMyUnit;
	int32 costMyUnit;
	int32 hpMyUnit;

	RectF rectF;
};
