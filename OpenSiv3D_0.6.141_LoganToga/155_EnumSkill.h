#pragma once
enum class SkillType
{
	missile,
	sword,
	heal,
	summon,
	charge,
	status,
};
enum class MoveType
{
	line,
	arc,
	thr,
	drop,
	circle,
	swing
};
enum class SkillEasing
{
	easeOutExpo
};
enum class SkillCenter
{
	on, off, end
};
enum class SkillBomb
{
	on, off
};
enum class SkillD360
{
	on, off
};
enum class SkillForceRay
{
	on, off
};
enum class SkillStrKind
{
	none, attack, magic, attack_magic, attack_dext, magic_dext, fix
};
