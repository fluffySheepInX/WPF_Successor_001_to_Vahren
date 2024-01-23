#pragma once
# include "ClassUnit.h" 
# include "Enum.h" 

class ClassSkill
{
public:
	s3d::String nameTag;
	s3d::String name;
	s3d::String help;
	s3d::String image;
	s3d::Array<s3d::String> icon;
	std::pair<s3d::String, int32> fkey;
	int32 sortKey = 0;
	SkillType SkillType = SkillType::sword;
	/// <summary>
	///		movetype = line、speed≠0	直進タイプ	敵に向かって直進します
	///		movetype = line、speed = 0	静止タイプ	静止したまま
	///		movetype = arc	円弧タイプ	円弧軌道で飛びます
	///		movetype = throw	放物線タイプ	放物線軌道で飛びます
	///		movetype = drop、speed = 正の値	落下タイプ	スキルが落下します
	///		movetype = drop、speed = 負の値	上昇タイプ	スキルが上昇します
	///		movetype = circle	回転タイプ	術者を中心に回転します
	///		movetype = swing	振り回しタイプ	術者を中心に旋回します（剣を振る等）
	/// </summary>
	///
	MoveType MoveType = MoveType::line;
	Optional<SkillEasing> Easing = none;
	int32 EasingRatio = 0;
	double mp = 0.0;
	/// @brief 省略時は75%。発射した時の「移動」速度変化率。
	Optional<double> slowPer = 0.75;
	/// @brief 発射後に数値分の攻撃間隔だけ「ユニットが」減速され続けます。
	Optional<int32> slowTime = 1;
	SkillCenter SkillCenter = SkillCenter::on;
	SkillBomb SkillBomb = SkillBomb::on;
	s3d::Array<s3d::String> sound;
	s3d::String direct;
	int32 w = 0;
	int32 h = 0;
	int32 a = 0;
	s3d::String forceFire;
	s3d::String attr;
	std::pair<s3d::String, int32> str;
	int32 range = 0;
	int32 damageRangeAdjust = 0;
	int32 rangeMin = 0;
	int32 speed = 0;
	std::pair<s3d::String, int32> gunDelay;
	s3d::String pairNext;
	s3d::String next;
	int32 randomSpace = 0;
	s3d::Array<s3d::String> offset;
	s3d::Array<int32> ray;
	s3d::String forceRay;
	int32 rush = -1;
	int32 rushInterval = -1;
	int32 rushRandomDegree = -1;

	float radian = 0;
	float degree = 0;
};
