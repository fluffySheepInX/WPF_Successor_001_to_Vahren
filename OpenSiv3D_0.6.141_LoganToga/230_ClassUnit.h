﻿#pragma once
# include "152_EnumBattleFormation.h" 
# include "153_EnumMapTipObjectType.h" 
# include "154_EnumGender.h" 
# include "240_ClassSkill.h" 

class ClassUnit
{
public:
	ClassUnit() = default;
	ClassUnit& operator=(const ClassUnit& other) {
		if (this != &other) {
			Formation = other.Formation; // 前提：ClassFormationが適切なコピーコンストラクタを持つ
			ID = other.ID;
			houkou = other.houkou;
			initXY = other.initXY;
			IsLeader = other.IsLeader;
			IsSelect = other.IsSelect;
			IsDone = other.IsDone;
			IsBattleEnable = other.IsBattleEnable;
			NameTag = other.NameTag;
			Name = other.Name;
			Help = other.Help;
			Text = other.Text;
			Race = other.Race;
			Class = other.Class;
			Image = other.Image;
			Dead = other.Dead;
			Retreat = other.Retreat;
			Join = other.Join;
			Face = other.Face;
			Voice_type = other.Voice_type;
			gender = other.gender;
			Talent = other.Talent;
			Friend = other.Friend;
			Merce = other.Merce;
			Staff = other.Staff;
			InitMember = other.InitMember;
			Enemy = other.Enemy;
			Level = other.Level;
			Level_max = other.Level_max;
			Price = other.Price;
			Cost = other.Cost;
			Medical = other.Medical;
			HasExp = other.HasExp;
			Hp = other.Hp;
			Mp = other.Mp;
			Attack = other.Attack;
			Defense = other.Defense;
			Magic = other.Magic;
			MagDef = other.MagDef;
			Speed = other.Speed;
			Dext = other.Dext;
			Move = other.Move;
			Hprec = other.Hprec;
			Mprec = other.Mprec;
			Exp = other.Exp;
			Exp_mul = other.Exp_mul;
			Heal_max = other.Heal_max;
			Summon_max = other.Summon_max;
			No_knock = other.No_knock;
			Loyal = other.Loyal;
			Alive_per = other.Alive_per;
			Escape_range = other.Escape_range;
			SkillName = other.SkillName; // コピーされるStringの各要素は新しいメモリを確保します
			Skill = other.Skill; // 前提：ClassSkillが適切なコピーコンストラクタを持つ
			Finance = other.Finance;
			MoveType = other.MoveType;
			FlagMove = other.FlagMove;
			FlagMoving = other.FlagMoving;
			FlagMoveDispose = other.FlagMoveDispose;
			yokoUnit = other.yokoUnit;
			TakasaUnit = other.TakasaUnit;
			nowPosiLeft = other.nowPosiLeft;
			orderPosiLeft = other.orderPosiLeft;
			vecMove = other.vecMove;
			FlagMoveSkill = other.FlagMoveSkill;
			FlagMovingSkill = other.FlagMovingSkill;
		}
		return *this;
	}

	// Formation
	BattleFormation Formation;

	// ID
	long long ID = 0;

	Rect rectExecuteBtnStrategyMenu;
	Rect rectDetailStrategyMenu;
	bool pressedDetailStrategyMenu = false;

	// IsLeader
	bool IsBuilding = false;
	bool isValidBuilding() const {
		return IsBuilding;
	}
	bool IsBuildingEnable = false;
	int32 rowBuilding = 0;
	int32 colBuilding = 0;
	MapTipObjectType mapTipObjectType = MapTipObjectType::None;
	/// @brief wall2で攻撃側飛び道具がすり抜ける確率
	int32 NoWall2 = 0;
	int32 HPCastle = 0;
	int32 CastleDefense = 0;
	int32 CastleMagdef = 0;

	String houkou = U"";
	Point initXY = Point();

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
	Array<String> SkillName;

	// Skill
	Array<ClassSkill> Skill;

	// Finance
	int32 Finance = 0;

	// MoveType
	String MoveType;

	// FlagMove
	bool FlagMove = false;

	// FlagMoving
	bool FlagMoving = false;

	// FlagMoving
	bool FlagMovingEnd = true;

	// FlagMoveDispose
	bool FlagMoveDispose = false;

	int32 yokoUnit = 32;
	int32 TakasaUnit = 32;

	// NowPosiLeft
	Vec2 nowPosiLeft;

	// NowPosiCenter
	Vec2 GetNowPosiCenter()
	{
		return Vec2(nowPosiLeft.x + (yokoUnit / 2), nowPosiLeft.y + (TakasaUnit / 2));
	}
	// OrderPosiLeft
	Vec2 orderPosiLeft;
	Vec2 orderPosiLeftLast;

	// OrderPosiCenter
	Vec2 GetOrderPosiCenter()
	{
		return Vec2(orderPosiLeft.x + (yokoUnit / 2), orderPosiLeft.y + (TakasaUnit / 2));
	}

	// VecMove
	Vec2 vecMove;

	// FlagMoveSkill
	bool FlagMoveSkill = false;

	// FlagMovingSkill
	bool FlagMovingSkill = false;
};

