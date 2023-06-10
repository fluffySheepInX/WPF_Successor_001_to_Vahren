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

class ISkill
{
public:
	virtual ~ISkill() = default;
	virtual int getDamage()const = 0;
	virtual void sumDamage(int num) = 0;
	virtual void setDamage(int num) = 0;
	virtual int getRadius()const = 0;
	virtual void sumRadius(int num) = 0;
	virtual void setRadius(int num) = 0;
	virtual double getSpeed()const = 0;
	virtual void sumSpeed(double num) = 0;
	virtual void setSpeed(double num) = 0;
	virtual bool getStatusAlive()const = 0;
	virtual void setStatusAlive(bool value) = 0;
	virtual Vec2 getDirection()const = 0;
	virtual void setDirection(Vec2 num) = 0;
	virtual Vec2 getPos()const = 0;
	virtual void setPos(Vec2 num) = 0;
	virtual void sumPos(Vec2 num) = 0;
	virtual void draw() const = 0;
	virtual void drawRotated(int32 num) const = 0;
	virtual SkillType getSkillType()const = 0;
	virtual Circle getCircle()const = 0;
};

class SkillParent :public ISkill
{
public:
	SkillParent() = default;
	/// @brief 弾生存フラグ
	bool isAlive = true;
	/// @brief 中心座標
	Vec2 pos = { 0,0 };
	/// @brief 早さ（ピクセル毎秒）
	double speed = 0;
	/// @brief ダメージ量
	int32 damage = 0;
	/// @brief 向き（正規化済）
	Vec2 direction = { 0,0 };
	/// @brief 半径
	double radius = 10;
	Texture tex;
	/// @brief 幅
	double width = 0;
	/// @brief 高さ
	double height = 0;
	int getDamage()const override
	{
		return damage;
	}
	void sumDamage(int num) override
	{
		damage += num;
	}
	void setDamage(int num) override
	{
		damage = num;
	}
	int getRadius()const override
	{
		return radius;
	}
	void sumRadius(int num) override
	{
		radius += num;
	}
	void setRadius(int num) override
	{
		radius = num;
	}
	double getSpeed()const override
	{
		return speed;
	}
	void sumSpeed(double num) override
	{
		speed += num;
	}
	void setSpeed(double num) override
	{
		speed = num;
	}
	bool getStatusAlive()const override
	{
		return isAlive;
	}
	void setStatusAlive(bool value) override
	{
		isAlive = value;
	}
	Vec2 getDirection()const override
	{
		return direction;
	}
	void setDirection(Vec2 num) override
	{
		direction = num;
	}
	Vec2 getPos()const override
	{
		return pos;
	}
	void setPos(Vec2 num) override
	{
		pos = num;
	}
	void sumPos(Vec2 num) override
	{
		pos += num;
	}
	void draw() const override
	{
		tex.resized(radius).drawAt(pos);
	}
	void drawRotated(int32 num) const override
	{
		tex.resized(radius).rotated(num).drawAt(pos);
	}
	SkillType getSkillType()const override
	{
		return SkillType::missile;
	}
	/// @brief 弾の Circle を返す関数
	/// @return Circle
	Circle getCircle() const override
	{
		return Circle{ pos, radius * 2 };
	}
};
class SkillMissile :public SkillParent
{
	SkillType getSkillType() const override
	{
		return SkillType::missile;
	}
};
