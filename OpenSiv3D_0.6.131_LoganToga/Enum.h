#pragma once
enum class Language {
	English,
	Japan,
	C
};
enum class BattleWhichIsThePlayer {
	Sortie,
	Def,
	None
};
enum class Gender
{
	Neuter = 0
	,
	Male = 1
	,
	Female = 2
	,
	Androgynous = 3
	,
	infertile = 4
};
enum class MapTipObjectType
{
	None
	, WALL2
	, GATE
};

enum class SkillType
{
	missile,
	sword,
	heal,
	summon,
	charge,
	status,
};

/// <summary>
/// 部隊チップの種別
/// flag = 0 なら「ユニットの識別名」として扱う。
/// flag = 1 なら「@文字変数」として扱う。
/// flag = 2 なら「特殊な文字列」として扱う。
/// 
/// ユニットの識別名 同名のunit/class構造体ユニットが配置されます
/// 
/// @文字変数 @が接頭辞の文字列は文字変数と見なされます。代入スクリプトで防衛施設を変化できます
/// 
/// 特殊な文字列
/// 「@」 防衛部隊の位置。
/// 「@@」 侵攻部隊の位置。
/// 「@ESC@」 城兵の退却位置になります。
/// 
/// </summary>
///
enum class FlagBattleMapUnit
{
	Unit
	,
	Var
	,
	Spe
};
enum class BattleFormation
{
	F
	,
	M
	,
	B
};
enum class BattleStatus
{
	Battle
	,
	Message
	,
	Event
};
