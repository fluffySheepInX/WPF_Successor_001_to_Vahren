#pragma once

#ifndef GAMEUITOOLKIT_H_
#define GAMEUITOOLKIT_H_
class ProgressBar
{
private:

	RectF m_rect = Rect(0);

	ColorF m_backgroundColor = ColorF(0.25);

	Array<std::pair<double, ColorF>> m_barColors = {
		{ 1.0, ColorF(0.1, 0.8, 0.2) }
	};

	double m_round = 0.0;

	ColorF getBarColor(double progress) const
	{
		ColorF result = m_barColors.front().second;

		for (auto& c : m_barColors)
		{
			if (progress < c.first)
			{
				result = c.second;
			}
			else
			{
				break;
			}
		}

		return result;
	}

public:

	ProgressBar() = default;

	explicit ProgressBar(const RectF& rect, double round = 0.0)
		: ProgressBar(rect, ColorF(0.25), { { 1.0, ColorF(0.1, 0.8, 0.2) } }, round) {}

	ProgressBar(const RectF& rect, const ColorF& backgroundColor, const ColorF& barColor, double round = 0.0)
		: ProgressBar(rect, backgroundColor, { { 1.0, barColor } }, round) {}

	ProgressBar(const RectF& rect, const ColorF& backgroundColor, const Array<std::pair<double, ColorF>>& barColors, double round = 0.0)
		: m_rect(rect)
		, m_backgroundColor(backgroundColor)
		, m_barColors(barColors)
		, m_round(round)
	{
		m_barColors.sort_by([](const auto& a, const auto& b) { return a.first > b.first; });
	}

	// バーを描く
	const ProgressBar& draw(double value, double maxValue) const
	{
		const double progress = maxValue ? Math::Saturate(value / maxValue) : 1.0;
		const RectF innnerRect = m_rect.stretched(-1);
		const RectF innnerRectBar(innnerRect.pos, innnerRect.w * progress, innnerRect.h);

		if (m_round == 0.0)
		{
			m_rect.draw(m_backgroundColor);
			innnerRectBar.draw(getBarColor(progress));
		}
		else
		{
			m_rect.rounded(m_round).draw(m_backgroundColor);

			if (innnerRectBar.w)
			{
				const Polygon bar = innnerRectBar.asPolygon();
				const RoundRect innnerRoundRect = innnerRect.rounded(m_round - 1);
				const Polygon clip = innnerRoundRect.asPolygon();

				if (const auto g = Geometry2D::And(bar, clip))
				{
					g.front().draw(getBarColor(progress));
				}
			}
		}

		return *this;
	}

	//// 中央揃えでテキストを描く
	//const ProgressBar& withText(const Font& drawableText, double fontSize, const ColorF& textColor, const Vec2& textPosOffset = Vec2(0, 0)) const
	//{
	//	const Size textSize = drawableText.region(fontSize).size;
	//	const auto& font = drawableText.font;
	//	const Point textPos = (m_rect.pos + (m_rect.size - textSize) / 2 + textPosOffset).asPoint();
	//	Graphics2D::SetSDFParameters(font.pixelRange(), 0.2);
	//	drawableText.draw(fontSize, textPos, ColorF(0.1));
	//	Graphics2D::SetSDFParameters(font.pixelRange(), 0.0);
	//	drawableText.draw(fontSize, textPos, textColor);
	//	return *this;
	//}

	// 左揃えでテキストを描く
	const ProgressBar& withLabel(const DrawableText drawableText, double fontSize, const ColorF& textColor, double offsetX, const Vec2& textPosOffset = Vec2(0, 0)) const
	{
		const RectF textSize = drawableText.region();
		const Point textPos = (Vec2(m_rect.x + offsetX, m_rect.y + (m_rect.h - textSize.y) / 2) + textPosOffset).asPoint();
		drawableText.draw(fontSize, textPos, ColorF(0.1));
		drawableText.draw(fontSize, textPos, textColor);
		return *this;
	}
};

class Slice9
{
public:

	struct Style
	{
		Rect backgroundRect = Rect{ 0, 0, 64, 64 };

		Rect frameRect = Rect{ 64, 0, 64, 64 };

		int32 cornerSize = 8;

		bool backgroundRepeat = false;

		[[nodiscard]]
		static Style Default() noexcept
		{
			return{};
		}
	};

	Slice9() = default;

	explicit Slice9(FilePathView path, const Style& style = Style::Default())
		: m_style{ style }
	{
		Image image{ path };
		m_background = Texture{ image.clipped(m_style.backgroundRect) };
		m_frame = Texture{ image.clipped(m_style.frameRect) };
	}

	void draw(const Rect& rect) const
	{
		if (m_style.backgroundRepeat)
		{
			const ScopedRenderStates2D repeat{ SamplerState::RepeatLinear };
			m_background.mapped(rect.size).draw(rect.pos);
		}
		else
		{
			rect(m_background).draw();
		}

		{
			const Rect top{ m_style.cornerSize, 0, (m_style.frameRect.w - (m_style.cornerSize * 2)), m_style.cornerSize };
			const Rect bottom{ m_style.cornerSize, (m_style.frameRect.h - m_style.cornerSize), (m_style.frameRect.w - (m_style.cornerSize * 2)), m_style.cornerSize };
			const Rect left{ 0, m_style.cornerSize, m_style.cornerSize, (m_style.frameRect.h - (m_style.cornerSize * 2)) };
			const Rect right{ (m_style.frameRect.w - m_style.cornerSize), m_style.cornerSize, m_style.cornerSize, (m_style.frameRect.h - (m_style.cornerSize * 2)) };

			m_frame(top).resized(rect.w - (m_style.cornerSize * 2), m_style.cornerSize).draw(rect.pos.movedBy(m_style.cornerSize, 0));
			m_frame(bottom).resized(rect.w - (m_style.cornerSize * 2), m_style.cornerSize).draw(rect.pos.movedBy(m_style.cornerSize, rect.h - m_style.cornerSize));
			m_frame(left).resized(m_style.cornerSize, rect.h - (m_style.cornerSize * 2)).draw(rect.pos.movedBy(0, m_style.cornerSize));
			m_frame(right).resized(m_style.cornerSize, rect.h - (m_style.cornerSize * 2)).draw(rect.pos.movedBy(rect.w - m_style.cornerSize, m_style.cornerSize));
		}

		{
			const Rect topLeft{ 0, 0, m_style.cornerSize };
			const Rect topRight{ (m_style.frameRect.w - m_style.cornerSize), 0, m_style.cornerSize };
			const Rect bottomLeft{ 0, (m_style.frameRect.h - m_style.cornerSize), m_style.cornerSize };
			const Rect bottomRight{ (m_style.frameRect.w - m_style.cornerSize), (m_style.frameRect.h - m_style.cornerSize), m_style.cornerSize };

			m_frame(topLeft).draw(rect.pos);
			m_frame(topRight).draw(rect.pos.movedBy(rect.w - m_style.cornerSize, 0));
			m_frame(bottomLeft).draw(rect.pos.movedBy(0, rect.h - m_style.cornerSize));
			m_frame(bottomRight).draw(rect.pos.movedBy(rect.w - m_style.cornerSize, rect.h - m_style.cornerSize));
		}
	}

	[[nodiscard]]
	const Texture& getBackgroundTexture() const noexcept
	{
		return m_background;
	}

	[[nodiscard]]
	const Texture& getFrameTexture() const noexcept
	{
		return m_frame;
	}

private:

	Style m_style;

	Texture m_background;

	Texture m_frame;
};

#endif
