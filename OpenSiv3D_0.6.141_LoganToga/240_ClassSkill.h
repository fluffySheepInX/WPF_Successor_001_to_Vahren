#pragma once
# include "155_EnumSkill.h" 

class ClassSkill
{
public:
	// == 演算子のオーバーロード
	bool operator==(const ClassSkill& other) const
	{
		return (nameTag == other.nameTag)
			&& (name == other.name)
			&& (help == other.help)
			&& (image == other.image)
			&& (icon == other.icon)
			&& (fkey == other.fkey)
			&& (sortKey == other.sortKey)
			&& (SkillType == other.SkillType)
			&& (MoveType == other.MoveType)
			&& (Easing == other.Easing)
			&& (EasingRatio == other.EasingRatio)
			&& (mp == other.mp)
			&& (slowPer == other.slowPer)
			&& (slowTime == other.slowTime)
			&& (SkillCenter == other.SkillCenter)
			&& (SkillBomb == other.SkillBomb)
			&& (SkillD360 == other.SkillD360)
			&& (sound == other.sound)
			&& (direct == other.direct)
			&& (w == other.w)
			&& (h == other.h)
			&& (a == other.a)
			&& (height == other.height)
			&& (radius == other.radius)
			&& (forceFire == other.forceFire)
			&& (attr == other.attr)
			&& (str == other.str)
			&& (SkillStrKind == other.SkillStrKind)
			&& (range == other.range)
			&& (damageRangeAdjust == other.damageRangeAdjust)
			&& (rangeMin == other.rangeMin)
			&& (speed == other.speed)
			&& (gunDelay == other.gunDelay)
			&& (pairNext == other.pairNext)
			&& (next == other.next)
			&& (randomSpace == other.randomSpace)
			&& (offset == other.offset)
			&& (ray == other.ray)
			&& (rush == other.rush)
			&& (rushInterval == other.rushInterval)
			&& (rushRandomDegree == other.rushRandomDegree)
			&& (SkillForceRay == other.SkillForceRay)
			&& (radian == other.radian)
			&& (degree == other.degree);
	}

	// < 演算子のオーバーロード
	bool operator<(const ClassSkill& other) const
	{
		return sortKey < other.sortKey;
	}

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
	SkillD360 SkillD360 = SkillD360::off;
	s3d::Array<s3d::String> sound;
	s3d::String direct;
	int32 w = 0;
	int32 h = 0;
	int32 a = 0;

	/// <summary>
	///	arc		放物線を高くする。負の値にすると低くなる
	///	throw	省略時は50。50にすると標的への直線距離の50% が高さ半径になる
	///	drop	落下タイプでは落下開始点の高さ。上昇タイプでは上昇する高さ
	///	circle	一周する毎に伸びる半径ﾄﾞｯﾄ数。400だと一周する度に4ﾄﾞｯﾄ半径が伸びる
	/// </summary>
	int32 height = 0;
	double radius = 0;
	s3d::String forceFire;
	s3d::String attr;
	int32 str;
	SkillStrKind SkillStrKind = SkillStrKind::none;
	int32 range = 0;
	int32 damageRangeAdjust = 0;
	int32 rangeMin = 0;
	double speed = 0;
	std::pair<s3d::String, int32> gunDelay;
	s3d::String pairNext;
	s3d::String next;
	int32 randomSpace = 0;
	s3d::Array<s3d::String> offset;
	Array<double> ray;
	int32 rayStrokeThickness;
	int32 rush = -1;
	int32 rushInterval = -1;
	int32 rushRandomDegree = -1;
	SkillForceRay SkillForceRay = SkillForceRay::off;
	float radian = 0;
	float degree = 0;
};
