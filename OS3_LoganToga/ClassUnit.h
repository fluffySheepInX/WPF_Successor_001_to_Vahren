#pragma once
# include "Enum.h" 
# include "ClassSkill.h" 

struct ClassFormation
{
	// ClassFormation のメンバ変数
};

class ClassUnit
{
public:
	//ClassUnit ShallowCopy() const
	//{
	//	return *this;
	//}

	//ClassUnit DeepCopy() const
	//{
	//	ClassUnit cu = *this;

	//	if (cu.Formation)
	//	{
	//		ClassFormation fo;
	//		fo.Formation = cu.Formation->Formation;
	//		fo.Id = cu.Formation->Id;
	//		cu.Formation = std::make_unique<ClassFormation>(std::move(fo));
	//	}

	//	// 配列など参照型のデータを新規作成して元の値をコピーする
	//	cu.SkillName = std::vector<String>(cu.SkillName);
	//	cu.Skill = std::vector<ClassSkill>(cu.Skill);

	//	if (cu.OrderPosiSkill)
	//	{
	//		cu.OrderPosiSkill = std::make_unique<HashTable<int32, Point>>(*cu.OrderPosiSkill);
	//	}

	//	if (cu.VecMoveSkill)
	//	{
	//		cu.VecMoveSkill = std::make_unique<HashTable<int32, Point>>(*cu.VecMoveSkill);
	//	}

	//	if (cu.NowPosiSkill)
	//	{
	//		cu.NowPosiSkill = std::make_unique<HashTable<int32, Point>>(*cu.NowPosiSkill);
	//	}

	//	return cu;
	//}

	// Formation
	ClassFormation Formation;

	// ID
	long long ID = 0;

	// IsLeader
	bool IsLeader = false;

	// IsSelect
	bool IsSelect = false;

	// IsDone
	bool IsDone = false;

	// IsBattleEnable
	bool IsBattleEnable = true;

	// NameTag
	String NameTag;

	// Name
	String Name;

	// Help
	String Help;

	// Text
	String Text;

	// Race
	String Race;

	// Class
	String Class;

	// Image
	String Image;

	// Dead
	String Dead;

	// Retreat
	String Retreat;

	// Join
	String Join;

	// Face
	String Face;

	// Voice_type
	String Voice_type;

	// Gender
	Gender gender = Gender::Neuter;

	// Talent
	String Talent;

	// Friend
	String Friend;

	// Merce
	String Merce;

	// Staff
	String Staff;

	// InitMember
	String InitMember;

	// Enemy
	String Enemy;

	// Level
	int32 Level = 0;

	// Level_max
	int32 Level_max = 0;

	// Price
	int32 Price = 0;

	// Cost
	int32 Cost = 0;

	// Medical
	int32 Medical = 0;

	// HasExp
	int32 HasExp = 0;

	// Hp
	int32 Hp = 0;

	// Mp
	int32 Mp = 0;

	// Attack
	int32 Attack = 0;

	// Defense
	int32 Defense = 0;

	// Magic
	int32 Magic = 0;

	//MagDef
	int32 MagDef = 0;

	// Speed
	double Speed = 0.0;

	// Dext
	int32 Dext = 0;

	// Move
	int32 Move = 0;

	// Hprec
	int32 Hprec = 0;

	// Mprec
	int32 Mprec = 0;

	// Exp
	int32 Exp = 0;

	// Exp_mul
	int32 Exp_mul = 0;

	// Heal_max
	int32 Heal_max = 0;

	// Summon_max
	int32 Summon_max = 0;

	// No_knock
	int32 No_knock = 0;

	// Loyal
	int32 Loyal = 0;

	// Alive_per
	int32 Alive_per = 0;

	// Escape_range
	int32 Escape_range = 0;

	// SkillName
	std::vector<String> SkillName;

	// Skill
	std::vector<ClassSkill> Skill;

	// Finance
	int32 Finance = 0;

	// MoveType
	String MoveType;

	// FlagMove
	bool FlagMove = false;

	// FlagMoving
	bool FlagMoving = false;

	// FlagMoveDispose
	bool FlagMoveDispose = false;

	int32 yokoUnit = 32;
	int32 TakasaUnit = 32;

	// NowPosiLeft
	Point nowPosiLeft;

	// NowPosiCenter
	Point GetNowPosiCenter()
	{
		return Point(nowPosiLeft.x + (yokoUnit / 2), nowPosiLeft.y + (TakasaUnit / 2));
	}
	// OrderPosiLeft
	Point orderPosiLeft;

	// OrderPosiCenter
	Point GetOrderPosiCenter()
	{
		return Point(orderPosiLeft.x + (yokoUnit / 2), orderPosiLeft.y + (TakasaUnit / 2));
	}

	// VecMove
	Point vecMove;

	// FlagMoveSkill
	bool FlagMoveSkill = false;

	// FlagMovingSkill
	bool FlagMovingSkill = false;

	// NowPosiSkill
	HashTable<int32, Point> NowPosiSkill;

	// OrderPosiSkill
	HashTable<int32, Point> OrderPosiSkill;

	// VecMoveSkill
	HashTable<int32, Point> VecMoveSkill;
};
