#pragma once
struct IFade
{
	virtual ~IFade() = default;
	virtual void fade(double t) = 0;
};
struct Fade4 : public IFade
{
	Array<int> rectPos;
	const int size = 50;
	const int w = Scene::Width() / size;
	const int h = Scene::Height() / size;

	Fade4()
	{
		rectPos = Iota(w * h);
		Shuffle(rectPos);
	}

	void fade(double t) override
	{
		for (auto [index, pos] : Indexed(rectPos))
		{
			if (index > t * w * h) break;

			const auto x = pos % w;
			const auto y = pos / w;
			Rect{ x * size, y * size, size }.draw(Color{ 0,0,0,255 });
		}
	}
};
struct PerlinEffect : IEffect
{
	Vec2 m_pos;
	ColorF color;
	PerlinNoise noise{ RandomUint32() };

	explicit PerlinEffect(const Vec2& pos)
		:m_pos{ pos + RandomVec2(5) },
		color{ ColorF(1) } {}

	bool update(double t)
	{
		m_pos -= Vec2{ 0, noise.noise1D0_1(t) * 15 };
		color.a = 1 - t;
		Circle{ m_pos + Vec2{noise.noise1D(t) * 100, 0}, 5 - t * 5 }.draw(color);

		return t < 1.0;
	}
};
