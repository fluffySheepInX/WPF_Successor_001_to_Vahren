#pragma once

#ifndef GAMEUITOOLKIT_H_
#define GAMEUITOOLKIT_H_
namespace GameUIToolkit {
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

	class LiquidBar
	{
	public:

		LiquidBar() = default;

		explicit LiquidBar(const Rect& rect)
			: m_rect{ rect } {}

		void update(double targetHP)
		{
			m_targetHP = targetHP;
			m_liquidHP = Math::SmoothDamp(m_liquidHP, targetHP, m_liquidHPVelocity, LiquidSmoothTime);

			if (m_solidHP < targetHP)
			{
				m_solidHP = targetHP;
			}
			else
			{
				m_solidHP = Math::SmoothDamp(m_solidHP, targetHP, m_solidHPVelocity, SolidSmoothTime, MaxSolidBarSpeed);
			}
		}

		const LiquidBar& draw(const ColorF& liquidColorFront, const ColorF& liquidColorBack, const ColorF& solidColor) const
		{
			// バーの背景を描く
			m_rect.draw(ColorF{ 0.2, 0.15, 0.25 });

			// バーの枠を描く
			m_rect.drawFrame(2, 0);

			const Point basePos = m_rect.pos.movedBy(FrameThickness, FrameThickness);
			const int32 height = (m_rect.h - (FrameThickness * 2));
			const double width = (m_rect.w - (FrameThickness * 2));

			const double solidWidth = Min(Max((width * m_solidHP) + (height * 0.5 * 0.3), 0.0), width);
			const double liquidWidth = (width * m_liquidHP);

			// 固体バーを描く
			{
				const RectF solidBar{ basePos, solidWidth, height };
				const double alpha = ((0.005 < AbsDiff(m_targetHP, m_solidHP)) ? 1.0 : (AbsDiff(m_targetHP, m_solidHP) / 0.005));
				solidBar.draw(ColorF{ solidColor, alpha });
			}

			// 液体バーを描く
			{
				const double t = Scene::Time();
				const double offsetScale = ((m_liquidHP < 0.05) ? (m_liquidHP / 0.05) : (0.98 < m_liquidHP) ? 0.0 : 1.0);

				// 背景の液体バーを描く
				for (int32 i = 0; i < height; ++i)
				{
					const Vec2 pos = basePos.movedBy(0, i);
					const double waveOffset = (i * 0.3)
						+ (Math::Sin(i * 17_deg + t * 800_deg) * 0.8)
						+ (Math::Sin(i * 11_deg + t * 700_deg) * 1.2)
						+ (Math::Sin(i * 7_deg + t * 550_deg) * 1.6);
					const RectF rect{ pos, Clamp((liquidWidth + waveOffset * offsetScale), 0.0, width), 1 };

					const double distance = Clamp(1.0 - i / (height - 1.0) + 0.7, 0.0, 1.0);
					HSV hsv{ liquidColorBack };
					hsv.v *= Math::Pow(distance, 2.0);
					rect.draw(hsv);
				}

				// 前景の液体バーを描く
				for (int32 i = 0; i < height; ++i)
				{
					const Vec2 pos = basePos.movedBy(0, i);
					const double waveOffset = (i * 0.3)
						+ (Math::Sin(i * 17_deg - t * 800_deg) * 0.8)
						+ (Math::Sin(i * 11_deg - t * 700_deg) * 1.2)
						+ (Math::Sin(i * 7_deg - t * 550_deg) * 1.6);
					const RectF rect{ pos, Clamp((liquidWidth + waveOffset * offsetScale), 0.0, width), 1 };

					const double distance = Clamp(1.0 - i / (height - 1.0) + 0.7, 0.0, 1.0);
					HSV hsv{ liquidColorFront };
					hsv.v *= Math::Pow(distance, 2.0);
					rect.draw(hsv);
				}
			}
			return *this;
		}
		// 左揃えでテキストを描く
		const LiquidBar& withLabel(const DrawableText drawableText, double fontSize, const ColorF& textColor, double offsetX, const Vec2& textPosOffset = Vec2(0, 0)) const
		{
			const RectF textSize = drawableText.region();
			const Point textPos = (Vec2(m_rect.x + offsetX, m_rect.y + (m_rect.h - textSize.y) / 2) + textPosOffset).asPoint();
			drawableText.draw(fontSize, textPos, ColorF(0.1));
			drawableText.draw(fontSize, textPos, textColor);
			return *this;
		}

	private:

		// 液体バーが減少するときの平滑化時間（小さいと早く目標に到達）
		static constexpr double LiquidSmoothTime = 0.03;

		// 固体バーが減少するときの平滑化時間（小さいと早く目標に到達）
		static constexpr double SolidSmoothTime = 0.5;

		// 固体バーが減少するときの最大の速さ
		static constexpr double MaxSolidBarSpeed = 0.25;

		static constexpr int32 FrameThickness = 2;

		Rect m_rect = Rect::Empty();

		double m_targetHP = 1.0;
		double m_liquidHP = 1.0;
		double m_solidHP = 1.0;
		double m_liquidHPVelocity = 0.0;
		double m_solidHPVelocity = 0.0;
	};
	class LiquidBarBattle
	{
	public:

		LiquidBarBattle() = default;

		explicit LiquidBarBattle(const Rect& rect)
			: m_rect{ rect } {}

		void update(double targetHP)
		{
			m_targetHP = targetHP;
			m_liquidHP = Math::SmoothDamp(m_liquidHP, targetHP, m_liquidHPVelocity, LiquidSmoothTime);

			if (m_solidHP < targetHP)
			{
				m_solidHP = targetHP;
			}
			else
			{
				m_solidHP = Math::SmoothDamp(m_solidHP, targetHP, m_solidHPVelocity, SolidSmoothTime, MaxSolidBarSpeed);
			}
		}

		void ChangePoint(const Vec2 v)
		{
			m_rect.x = v.x;
			m_rect.y = v.y;
		}

		const LiquidBarBattle& draw(const ColorF& liquidColorFront, const ColorF& liquidColorBack, const ColorF& solidColor) const
		{
			// バーの背景を描く
			m_rect.draw(ColorF{ 0.2, 0.15, 0.25 });

			// バーの枠を描く
			m_rect.drawFrame(2, 0);

			const Point basePos = m_rect.pos.movedBy(FrameThickness, FrameThickness);
			const int32 height = (m_rect.h - (FrameThickness * 2));
			const double width = (m_rect.w - (FrameThickness * 2));

			const double solidWidth = Min(Max((width * m_solidHP) + (height * 0.5 * 0.3), 0.0), width);
			const double liquidWidth = (width * m_liquidHP);

			// 固体バーを描く
			{
				const RectF solidBar{ basePos, solidWidth, height };
				const double alpha = ((0.005 < AbsDiff(m_targetHP, m_solidHP)) ? 1.0 : (AbsDiff(m_targetHP, m_solidHP) / 0.005));
				solidBar.draw(ColorF{ solidColor, alpha });
			}

			// 液体バーを描く
			{
				const double t = Scene::Time();
				const double offsetScale = ((m_liquidHP < 0.05) ? (m_liquidHP / 0.05) : (0.98 < m_liquidHP) ? 0.0 : 1.0);

				// 背景の液体バーを描く
				for (int32 i = 0; i < height; ++i)
				{
					const Vec2 pos = basePos.movedBy(0, i);
					const double waveOffset = (i * 0.3)
						+ (Math::Sin(i * 17_deg + t * 800_deg) * 0.8)
						+ (Math::Sin(i * 11_deg + t * 700_deg) * 1.2)
						+ (Math::Sin(i * 7_deg + t * 550_deg) * 1.6);
					const RectF rect{ pos, Clamp((liquidWidth + waveOffset * offsetScale), 0.0, width), 1 };

					const double distance = Clamp(1.0 - i / (height - 1.0) + 0.7, 0.0, 1.0);
					HSV hsv{ liquidColorBack };
					hsv.v *= Math::Pow(distance, 2.0);
					rect.draw(hsv);
				}

				// 前景の液体バーを描く
				for (int32 i = 0; i < height; ++i)
				{
					const Vec2 pos = basePos.movedBy(0, i);
					const double waveOffset = (i * 0.3)
						+ (Math::Sin(i * 17_deg - t * 800_deg) * 0.8)
						+ (Math::Sin(i * 11_deg - t * 700_deg) * 1.2)
						+ (Math::Sin(i * 7_deg - t * 550_deg) * 1.6);
					const RectF rect{ pos, Clamp((liquidWidth + waveOffset * offsetScale), 0.0, width), 1 };

					const double distance = Clamp(1.0 - i / (height - 1.0) + 0.7, 0.0, 1.0);
					HSV hsv{ liquidColorFront };
					hsv.v *= Math::Pow(distance, 2.0);
					rect.draw(hsv);
				}
			}
			return *this;
		}
		// 左揃えでテキストを描く
		const LiquidBarBattle& withLabel(const DrawableText drawableText, double fontSize, const ColorF& textColor, double offsetX, const Vec2& textPosOffset = Vec2(0, 0)) const
		{
			const RectF textSize = drawableText.region();
			const Point textPos = (Vec2(m_rect.x + offsetX, m_rect.y + (m_rect.h - textSize.y) / 2) + textPosOffset).asPoint();
			drawableText.draw(fontSize, textPos, ColorF(0.1));
			drawableText.draw(fontSize, textPos, textColor);
			return *this;
		}

	private:

		// 液体バーが減少するときの平滑化時間（小さいと早く目標に到達）
		static constexpr double LiquidSmoothTime = 0.03;

		// 固体バーが減少するときの平滑化時間（小さいと早く目標に到達）
		static constexpr double SolidSmoothTime = 0.5;

		// 固体バーが減少するときの最大の速さ
		static constexpr double MaxSolidBarSpeed = 0.25;

		static constexpr int32 FrameThickness = 2;

		Rect m_rect = Rect::Empty();

		double m_targetHP = 1.0;
		double m_liquidHP = 1.0;
		double m_solidHP = 1.0;
		double m_liquidHPVelocity = 0.0;
		double m_solidHPVelocity = 0.0;
	};

	class HPBar
	{
	public:
		struct Style
		{
			ColorF backgroundColor{ 0.0, 0.6 };
			ColorF delayColor{ 0.9, 0.8, 0.3 };
			ColorF hpColor{ 0.8, 0.2, 0.2 };
			ColorF frameColor{ 0.1 };
			double frameThickness = 1.5;
		};

		HPBar() = default;

		explicit constexpr HPBar(int32 maxHP) noexcept
			: m_maxHP{ maxHP }
			, m_currentHP{ 0 } // 初期HPを0に設定
			, m_delayHP{ 0.0 } {}

		constexpr HPBar(int32 maxHP, int32 currentHP) noexcept
			: m_maxHP{ maxHP }
			, m_currentHP{ currentHP }
			, m_delayHP{ static_cast<double>(m_currentHP) } {}

		void update(double smoothTimeSec = 0.4)
		{
			m_delayHP = Math::SmoothDamp(m_delayHP, m_currentHP, m_delayVelocity, smoothTimeSec);
		}

		void draw(const RectF& rect) const
		{
			if (healFlag)
			{
				drawHeal(rect, Style{});
			}
			else
			{
				draw(rect, Style{});
			}
		}

		void draw(const RectF& rect, const Style& style) const
		{
			const RectF rectDelay{ rect.pos, (rect.w * getDelayHPRatio()), rect.h };
			const RectF rectHP{ rect.pos, (rect.w * getHPRatio()), rect.h };

			rect.draw(style.backgroundColor);
			rectDelay.draw(style.delayColor);
			rectHP.draw(style.hpColor);
			rect.drawFrame(style.frameThickness, style.frameColor);
		}
		void drawHeal(const RectF& rect, const Style& style) const
		{
			const RectF rectDelay{ rect.pos, (rect.w * getHPRatio()), rect.h };
			const RectF rectHP{ rect.pos, (rect.w * getDelayHPRatio()), rect.h };

			rect.draw(style.backgroundColor);
			rectDelay.draw(style.delayColor);
			rectHP.draw(style.hpColor);
			rect.drawFrame(style.frameThickness, style.frameColor);
		}

		void drawHex(const RectF& rect) const
		{
			drawHex(rect, Style{});
		}

		void drawHex(const RectF& rect, const Style& style) const
		{
			const RectF rectDelay{ rect.pos, (rect.w * getDelayHPRatio()), rect.h };
			const RectF rectHP{ rect.pos, (rect.w * getHPRatio()), rect.h };
			const Polygon hex = MakeHexPolygon(rect);

			hex.draw(style.backgroundColor);

			for (const auto& shape : Geometry2D::And(hex, rectDelay))
			{
				shape.draw(style.delayColor);
			}

			for (const auto& shape : Geometry2D::And(hex, rectHP))
			{
				shape.draw(style.hpColor);
			}

			hex.drawFrame(style.frameThickness, style.frameColor);
		}

		[[nodiscard]]
		constexpr int32 getHP() const noexcept
		{
			return m_currentHP;
		}

		[[nodiscard]]
		constexpr int32 getMaxHP() const noexcept
		{
			return m_maxHP;
		}

		[[nodiscard]]
		constexpr double getHPRatio() const noexcept
		{
			return (static_cast<double>(m_currentHP) / m_maxHP);
		}

		[[nodiscard]]
		constexpr double getDelayHPRatio() const noexcept
		{
			return (m_delayHP / m_maxHP);
		}

		constexpr void setHP(int32 hp) noexcept
		{
			m_currentHP = Clamp(hp, 0, m_maxHP);
			m_delayVelocity = 0.0; // ここでm_delayHPを変更しない
		}

		constexpr void damage(int32 damage) noexcept
		{
			m_currentHP = Clamp((m_currentHP - damage), 0, m_maxHP);
			healFlag = false;
		}

		constexpr void heal(int32 heal) noexcept
		{
			setHP(m_currentHP + heal);
		}

		void healEffect(int32 heal) noexcept
		{
			m_currentHP = Clamp((m_currentHP + heal), 0, m_maxHP);
			healFlag = true;
		}

	private:
		int32 m_maxHP = 1;
		int32 m_currentHP = 0;
		double m_delayHP = 0.0;
		double m_delayVelocity = 0.0;
		bool healFlag = false;
		[[nodiscard]]
		static Polygon MakeHexPolygon(const RectF& rect)
		{
			const Vec2 offsetH{ (rect.h * 0.5), 0.0 };
			const Vec2 offsetV{ 0.0, (rect.h * 0.5) };
			return Polygon{ { (rect.tl() + offsetH), (rect.tr() - offsetH), (rect.tr() + offsetV),
				(rect.br() - offsetH), (rect.bl() + offsetH), (rect.tl() + offsetV) } };
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

	class NewsPaperText
	{
	public:
		NewsPaperText() = default;

		explicit NewsPaperText(
			const Font& fontNormal,
			const Font& fontLine,
			const String& text,
			const Vec2& pos,
			const ColorF& textColor = ColorF{ 1 },
			const Size& padding = Size(10, 10),
			const int32 width = 280)
			: fontNormal{ fontNormal }
			, fontLine{ fontLine }
			, m_text{ text }
			, m_pos{ pos }
			, m_textColor{ textColor }
			, m_padding{ padding }
			, m_width{ width } {}

		void draw() const
		{
			if (m_text == U"")
			{
				return;
			}
			RectF reFontNormal = fontNormal(m_text.substr(0, 1)).region();
			String aaa = m_text.substr(1);
			RectF reFontLine = fontLine(aaa).region();
			Rect rect1{ -9999, -9999, (int32)(m_width - reFontNormal.w), (int32)(reFontLine.h) };

			// 一文字目を描く
			fontNormal(m_text.substr(0, 1)).draw(m_pos, m_textColor);

			int32 count = 1;
			for (size_t i = 1; i < m_text.size(); i++)
			{
				if (not fontLine(m_text.substr(1, count)).draw(rect1, ColorF{ 0 }))
				{
					// 文字が省略されたらそこで終了
					break;
				}
				count++;
				if (count == m_text.size())
				{
					break;
				}
			}

			// 1行目を描く
			fontLine(m_text.substr(1, count)).draw(m_pos.movedBy(reFontNormal.w, 0), m_textColor);
			if (count >= m_text.size())
			{
				return;
			}

			// 2行目の準備
			int32 line2CountStart = count;
			int32 line2Count = 0;
			for (size_t i = line2CountStart + 1; i < m_text.size(); i++)
			{
				if (not fontLine(m_text.substr(line2CountStart + 1, line2Count)).draw(rect1, ColorF{ 0 }))
				{
					// 文字が省略されたらそこで終了
					break;
				}
				line2Count++;
				if (line2Count == m_text.size() - line2CountStart - 1)
				{
					break;
				}
			}

			// 2行目を描く
			fontLine(m_text.substr(line2CountStart + 1, line2Count)).draw(m_pos.movedBy(reFontNormal.w, rect1.h), m_textColor);
			if (line2CountStart + 1 + line2Count >= m_text.size())
			{
				return;
			}

			// 3行目の準備
			RectF rect3{ m_pos.x, m_pos.y + rect1.h * 2, (int32)(m_width + 20), 600 };
			int32 line3CountStart = line2CountStart + 1 + line2Count;
			// 3行目以降を描く
			fontLine(m_text.substr(line3CountStart)).draw(rect3, m_textColor);

		}
	private:
		Font fontNormal;
		Font fontLine;
		String m_text = U"";
		ColorF m_textColor = ColorF{ 1 };
		Vec2 m_pos = Vec2(0, 0);
		Size m_padding = Size(0, 0);
		int32 m_width = 0;
	};

}
namespace SasaGUI
{
	namespace Config
	{
		struct ScrollBar
		{
			// |          ______________     |
			// |  =======|______________|==  |
			// |                             |
			//    ^---------Track---------^
			//           ^-----Thumb----^

			constexpr static ColorF BackgroundColor{ 1, 0 };
			constexpr static ColorF TrackColor{ 0, 0 };
			constexpr static ColorF ThumbColor{ 0.4, 0.7 };
			constexpr static ColorF HoveredBackgroundColor{ 1, 1 };
			constexpr static ColorF HoveredTrackColor{ 0, 0.1 };
			constexpr static ColorF HoveredThumbColor{ 0.4, 1 };

			constexpr static int32 TrackMargin = 3;
			constexpr static int32 ThumbThickness = 6;
			constexpr static int32 ThumbMinLength = 20;

			constexpr static Duration ScrollSmoothTime = 0.1s;
			constexpr static Duration LargeScrollDelay = 0.8s;
			constexpr static Duration LargeScrollInterval = 0.1s;
			constexpr static Duration FadeInDuration = 0.06s;
			constexpr static Duration FadeOutDuration = 0.5s;
			constexpr static Duration FadeOutDelay = 2s;
		};
	}

	class Delay
	{
	public:

		constexpr Delay(Duration duration) noexcept
			: m_duration(duration.count())
			, m_time(m_duration)
		{
			assert(duration >= 0s);
		}

		bool update(bool in, double d = Scene::DeltaTime()) noexcept
		{
			if (in)
			{
				m_time = 0.0;
			}
			else
			{
				m_time += d;
			}
			return in || m_time <= m_duration;
		}

	private:

		double m_duration;

		double m_time;
	};

	struct Repeat
	{
	public:

		Repeat(Duration interval, Duration delay = 0s)
			: m_interval(interval.count())
			, m_delay(delay.count())
		{
			assert(interval > 0s);
			assert(delay >= 0s);
		}

		bool update(bool in, double d = Scene::DeltaTime()) noexcept
		{
			if (not in)
			{
				m_first = true;
				m_delayed = false;
				return false;
			}

			bool out = false;

			if (m_first)
			{
				out = true;
				m_accumulation = 0.0;
				m_first = false;
			}

			m_accumulation += d;

			if (not m_delayed)
			{
				if (m_accumulation < m_delay)
				{
					return out;
				}

				out = true;
				m_accumulation -= m_delay;
				m_delayed = true;
			}

			double count = std::floor(m_accumulation / m_interval);
			out |= count > 0.0;
			m_accumulation -= count * m_interval;

			return out;
		}

	private:

		double m_interval;

		double m_delay;

		bool m_first = true;

		bool m_delayed = false;

		double m_accumulation = 0.0;
	};

	enum class Orientation
	{
		Horizontal, Vertical
	};

	class ScrollBar
	{
	public:

		using Config = Config::ScrollBar;

		constexpr static int32 Thickness = Config::TrackMargin * 2 + Config::ThumbThickness;
		constexpr static int32 MinLength = Config::TrackMargin * 2 + Config::ThumbMinLength + 10;
		constexpr static double ThumbRoundness = Config::ThumbThickness * 0.5;

		ScrollBar(Orientation orientation)
			: orientation(orientation)
		{
			updateLayout({ 0, 0, 0, 0 });
		}

	public:

		const Orientation orientation;

		Rect rect() const { return m_rect; }

		double minimum() const { return m_minimum; }

		double maximum() const { return m_maximum; }

		double value() const { return m_value; }

		double viewportSize() const { return m_viewportSize; }

		void updateLayout(Rect rect)
		{
			m_rect = rect;
			switch (orientation)
			{
			case Orientation::Horizontal:
				m_rect.w = Max(m_rect.w, MinLength);
				m_rect.h = Thickness;
				m_trackLength = m_rect.w - Config::TrackMargin * 2;
				break;
			case Orientation::Vertical:
				m_rect.w = Thickness;
				m_rect.h = Max(m_rect.h, MinLength);
				m_trackLength = m_rect.h - Config::TrackMargin * 2;
				break;
			}
			updateThumbLength();
		}

		void updateConstraints(double minimum, double maximum, double viewportSize)
		{
			m_minimum = minimum;
			m_maximum = maximum;
			m_viewportSize = viewportSize;

			m_value = Max(Min(m_value, m_maximum - m_viewportSize), m_minimum);

			updateThumbLength();
		}

		void show()
		{
			m_colorTransitionDelay.update(true);
		}

		void scroll(double delta, bool teleport = false)
		{
			if (teleport)
			{
				m_value = Max(Min(m_value + delta, m_maximum - m_viewportSize), m_minimum);
				m_scrollTarget = m_value;
				m_scrollVelocity = 0.0;
				return;
			}

			if (delta == 0.0)
			{
				return;
			}

			if (Math::Sign(delta) != Math::Sign(m_scrollVelocity))
			{
				m_scrollVelocity = 0.0;
			}
			m_scrollTarget = Max(Min(m_value + delta, m_maximum - m_viewportSize), m_minimum);
		}

		void moveTo(double value)
		{
			m_value = Clamp(value, m_minimum, m_maximum - m_viewportSize);
			m_scrollTarget = m_value;
			m_scrollVelocity = 0.0;
		}

		void update(Optional<Vec2> cursorXYPos = Cursor::PosF(), double deltaTime = Scene::DeltaTime())
		{
			Optional<double> cursorPos = cursorXYPos.map([this](Vec2 v) { return getMainAxisValue(v); });
			double cursorDelta = prevCursorPos && cursorPos
				? *cursorPos - *prevCursorPos
				: 0.0;
			bool mouseOver = cursorPos && m_rect.contains(*cursorXYPos);
			auto thumbRect = getThumbRect();
			auto trackRect = getTrackRect();

			if (m_thumbLength == 0.0 || not MouseL.pressed())
			{
				m_thumbGrabbed = false;
				m_trackPressed = false;
			}
			else if (cursorXYPos.has_value() && MouseL.down())
			{
				if (thumbRect.contains(*cursorXYPos))
				{
					m_thumbGrabbed = true;
				}
				else if (getTrackRect().contains(*cursorXYPos))
				{
					m_trackPressed = true;
					m_largeScrollDirection = Math::Sign(*cursorPos - getMainAxisValue(thumbRect.pos));
					m_largeScrollTargetPos = *cursorPos;
				}
			}

			if (m_thumbGrabbed && cursorDelta != 0.0)
			{
				scroll(cursorDelta * (m_maximum - m_minimum - m_viewportSize) / (m_trackLength - m_thumbLength), true);
			}

			if (m_largeScrollTimer.update(m_trackPressed, deltaTime))
			{
				scroll(m_largeScrollDirection * m_viewportSize);

				double thumbPos = calculateThumbPos(m_scrollTarget);
				if (thumbPos <= m_largeScrollTargetPos &&
					m_largeScrollTargetPos <= thumbPos + m_thumbLength)
				{
					m_trackPressed = false;
				}
				if (m_scrollTarget <= m_minimum ||
					m_maximum - m_viewportSize <= m_scrollTarget)
				{
					m_trackPressed = false;
				}
			}

			m_value = Math::SmoothDamp(m_value, m_scrollTarget, m_scrollVelocity, Config::ScrollSmoothTime.count());

			bool show = (mouseOver || m_thumbGrabbed || m_trackPressed) && m_thumbLength != 0.0;
			m_colorTransition.update(m_colorTransitionDelay.update(show, deltaTime), deltaTime);

			prevCursorPos = cursorPos;
		}

		void draw() const
		{
			double f = m_colorTransition.value();
			ColorF backColor = Config::BackgroundColor.lerp(Config::HoveredBackgroundColor, f);
			ColorF trackColor = Config::TrackColor.lerp(Config::HoveredTrackColor, f);
			ColorF thumbColor = Config::ThumbColor.lerp(Config::HoveredThumbColor, f);
			m_rect
				.draw(backColor);
			getTrackRect()
				.rounded(ThumbRoundness)
				.draw(trackColor);
			if (m_thumbLength != 0.0)
			{
				getThumbRect()
					.rounded(ThumbRoundness)
					.draw(thumbColor);
			}
		}

	private:

		// 制限&スクロール位置

		double m_minimum = 0.0;

		double m_maximum = 1.0;

		double m_value = 0.0;

		double m_viewportSize = 1.0;

		// レイアウト

		Rect m_rect;

		double m_trackLength;

		double m_thumbLength = 0.0; // 表示しないとき0

		// 状態

		bool m_thumbGrabbed = false;

		bool m_trackPressed = false;

		// スクロール状態

		double m_scrollTarget = 0.0;

		double m_scrollVelocity = 0.0;

		int32 m_largeScrollDirection = 0;

		double m_largeScrollTargetPos = 0.0;

		// アニメーション関連

		Repeat m_largeScrollTimer{ Config::LargeScrollInterval, Config::LargeScrollDelay };

		Transition m_colorTransition{ Config::FadeInDuration, Config::FadeOutDuration };

		Delay m_colorTransitionDelay{ Config::FadeOutDelay };

		// その他

		Optional<double> prevCursorPos;

		void updateThumbLength()
		{
			double range = m_maximum - m_minimum;
			if (range <= 0.0 ||
				m_viewportSize >= range)
			{
				m_thumbLength = 0;
			}
			else
			{
				m_thumbLength = Max<double>(m_trackLength * m_viewportSize / range, Config::ThumbMinLength);
			}
		}

		Rect getTrackRect() const { return m_rect.stretched(-Config::TrackMargin); }

		inline double calculateThumbPos(double value) const
		{
			return (m_trackLength - m_thumbLength) * value / (m_maximum - m_minimum - m_viewportSize);
		}

		RectF getThumbRect() const
		{
			Point trackPos = getTrackRect().pos;
			double pos = calculateThumbPos(m_value);

			switch (orientation)
			{
			case Orientation::Horizontal:
				return {
					trackPos.x + pos,
					trackPos.y,
					m_thumbLength,
					Config::ThumbThickness
				};
			case Orientation::Vertical:
				return {
					trackPos.x,
					trackPos.y + pos,
					Config::ThumbThickness,
					m_thumbLength
				};
			}
		}

		double getMainAxisValue(Vec2 input) const
		{
			switch (orientation)
			{
			case Orientation::Horizontal: return input.x;
			case Orientation::Vertical: return input.y;
			}
		}
	};
}
namespace s3dx
{
	class SceneMessageBoxImpl
	{
	public:
		RectF m_buttonC = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(0, -20)), MessageBoxButtonSize };

		RectF m_buttonL = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(-80, -20)), MessageBoxButtonSize };

		RectF m_buttonR = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(80, -20)), MessageBoxButtonSize };

		static constexpr Size MessageBoxSize{ 360, 240 };

		static constexpr Size MessageBoxButtonSize{ 120, 40 };

		static constexpr ColorF MessageBoxBackgroundColor{ 0.96 };

		static constexpr ColorF MessageBoxActiveButtonColor{ 1.0 };

		static constexpr ColorF MessageBoxTextColor{ 0.11 };

		SceneMessageBoxImpl()
		{
			//System::SetTerminationTriggers(UserAction::NoAction);
			//Scene::SetBackground(ColorF{ 0.11 });
		}

		~SceneMessageBoxImpl()
		{
			//System::SetTerminationTriggers(m_triggers);
			//Scene::SetBackground(m_bgColor);
		}

		void set(Camera2D& camera2D)
		{
			m_pos = ((camera2D.getCenter() - MessageBoxSize) * 0.5);
			m_messageBoxRect = { m_pos, MessageBoxSize };
			m_buttonC = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(0, -20)), MessageBoxButtonSize };
			m_buttonL = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(-80, -20)), MessageBoxButtonSize };
			m_buttonR = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(80, -20)), MessageBoxButtonSize };
			m_display = true;
		}
		void set()
		{
			m_pos = ((Scene::Center() - (MessageBoxSize * 0.5)));
			m_messageBoxRect = { m_pos, MessageBoxSize };
			m_buttonC = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(0, -20)), MessageBoxButtonSize };
			m_buttonL = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(-80, -20)), MessageBoxButtonSize };
			m_buttonR = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(80, -20)), MessageBoxButtonSize };
			m_display = true;
		}

		void show(StringView text) const
		{
			if (m_display == false)
			{
				return;
			}
			drawMessageBox(text);
			m_buttonC.draw(m_buttonC.mouseOver() ? MessageBoxActiveButtonColor : MessageBoxBackgroundColor).drawFrame(0, 1, MessageBoxTextColor);
			m_font(U"OK").drawAt(m_buttonC.center().moveBy(0, -1), MessageBoxTextColor);
		}

		MessageBoxResult show(const StringView text, const std::pair<String, MessageBoxResult>& button0, const std::pair<String, MessageBoxResult>& button1) const
		{
			MessageBoxResult result = MessageBoxResult::Cancel;

			drawMessageBox(text);
			m_buttonL.draw(m_buttonL.mouseOver() ? MessageBoxActiveButtonColor : MessageBoxBackgroundColor).drawFrame(0, 1, MessageBoxTextColor);
			m_buttonR.draw(m_buttonR.mouseOver() ? MessageBoxActiveButtonColor : MessageBoxBackgroundColor).drawFrame(0, 1, MessageBoxTextColor);
			m_font(button0.first).drawAt(m_buttonL.center().moveBy(0, -1), MessageBoxTextColor);
			m_font(button1.first).drawAt(m_buttonR.center().moveBy(0, -1), MessageBoxTextColor);

			if (m_buttonL.mouseOver())
			{
				Cursor::RequestStyle(CursorStyle::Hand);

				if (MouseL.down())
				{
					result = button0.second;
					return result;
				}
			}
			else if (m_buttonR.mouseOver())
			{
				Cursor::RequestStyle(CursorStyle::Hand);

				if (MouseL.down())
				{
					result = button1.second;
					return result;
				}
			}
			return result;
		}

		MessageBoxResult SceneMessageBoxUnlock(const StringView text) const
		{
			return show(text, { U"OK", MessageBoxResult::Yes }, { U"unlock", MessageBoxResult::No });
		}

		bool unlock = false;

	private:
		uint32 m_triggers = System::GetTerminationTriggers();

		ColorF m_bgColor = Scene::GetBackground();

		Vec2 m_pos = ((Scene::Size() - MessageBoxSize) * 0.5);;

		RectF m_messageBoxRect{ m_pos, MessageBoxSize };

		Font m_font = SimpleGUI::GetFont();

		bool m_display = false;

		void drawMessageBox(StringView text) const
		{
			m_messageBoxRect.draw(MessageBoxBackgroundColor).stretched(-5).drawFrame(1, 0, MessageBoxTextColor);
			m_font(text).draw(14, m_messageBoxRect.stretched(-20, -20, -80, -20), MessageBoxTextColor);
		}
	};

	//inline MessageBoxResult SceneMessageBoxOK(StringView text)
	//{
	//	return SceneMessageBoxImpl{}.show(text, { U"OK", MessageBoxResult::OK });
	//}

	//[[nodiscard]]
	//inline MessageBoxResult SceneMessageBoxOKCancel(StringView text)
	//{
	//	return SceneMessageBoxImpl{}.show(text, { U"OK", MessageBoxResult::OK }, { U"キャンセル", MessageBoxResult::Cancel });
	//}

	//[[nodiscard]]
	//inline MessageBoxResult SceneMessageBoxYesNo(StringView text)
	//{
	//	return SceneMessageBoxImpl{}.show(text, { U"はい", MessageBoxResult::Yes }, { U"いいえ", MessageBoxResult::No });
	//}
}
namespace s3dxModal
{
	class SceneMessageBoxImpl
	{
	public:

		static constexpr Size MessageBoxSize{ 360, 240 };

		static constexpr Size MessageBoxButtonSize{ 120, 40 };

		static constexpr ColorF MessageBoxBackgroundColor{ 0.96 };

		static constexpr ColorF MessageBoxActiveButtonColor{ 1.0 };

		static constexpr ColorF MessageBoxTextColor{ 0.11 };

		SceneMessageBoxImpl()
		{
			//System::SetTerminationTriggers(UserAction::NoAction);
			//Scene::SetBackground(ColorF{ 0.11 });
		}

		~SceneMessageBoxImpl()
		{
			System::SetTerminationTriggers(m_triggers);
			/*		Scene::SetBackground(m_bgColor);
			*/
		}

		MessageBoxResult show(StringView text, const std::pair<String, MessageBoxResult>& button) const
		{
			while (System::Update())
			{
				drawMessageBox(text);
				m_buttonC.draw(m_buttonC.mouseOver() ? MessageBoxActiveButtonColor : MessageBoxBackgroundColor).drawFrame(0, 1, MessageBoxTextColor);
				m_font(button.first).drawAt(m_buttonC.center().moveBy(0, -1), MessageBoxTextColor);

				if (m_buttonC.mouseOver())
				{
					Cursor::RequestStyle(CursorStyle::Hand);

					if (MouseL.down())
					{
						break;
					}
				}
			}

			return button.second;
		}

		MessageBoxResult show(const StringView text, const std::pair<String, MessageBoxResult>& button0, const std::pair<String, MessageBoxResult>& button1) const
		{
			MessageBoxResult result = MessageBoxResult::Cancel;

			while (System::Update())
			{
				drawMessageBox(text);
				m_buttonL.draw(m_buttonL.mouseOver() ? MessageBoxActiveButtonColor : MessageBoxBackgroundColor).drawFrame(0, 1, MessageBoxTextColor);
				m_buttonR.draw(m_buttonR.mouseOver() ? MessageBoxActiveButtonColor : MessageBoxBackgroundColor).drawFrame(0, 1, MessageBoxTextColor);
				m_font(button0.first).drawAt(m_buttonL.center().moveBy(0, -1), MessageBoxTextColor);
				m_font(button1.first).drawAt(m_buttonR.center().moveBy(0, -1), MessageBoxTextColor);

				if (m_buttonL.mouseOver())
				{
					Cursor::RequestStyle(CursorStyle::Hand);

					if (MouseL.down())
					{
						result = button0.second;
						break;
					}
				}
				else if (m_buttonR.mouseOver())
				{
					Cursor::RequestStyle(CursorStyle::Hand);

					if (MouseL.down())
					{
						result = button1.second;
						break;
					}
				}
			}

			return result;
		}

	private:

		Transformer2D m_tr{ Mat3x2::Identity(), Mat3x2::Identity(), Transformer2D::Target::SetLocal };

		ScopedRenderStates2D m_rs{ BlendState::Default2D, SamplerState::Default2D, RasterizerState::Default2D };

		uint32 m_triggers = System::GetTerminationTriggers();

		ColorF m_bgColor = Scene::GetBackground();

		Vec2 m_pos = ((Scene::Size() - MessageBoxSize) * 0.5);

		RectF m_messageBoxRect{ m_pos, MessageBoxSize };

		RectF m_buttonC = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(0, -20)), MessageBoxButtonSize };

		RectF m_buttonL = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(-80, -20)), MessageBoxButtonSize };

		RectF m_buttonR = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(80, -20)), MessageBoxButtonSize };

		Font m_font = SimpleGUI::GetFont();

		void drawMessageBox(StringView text) const
		{
			m_messageBoxRect.draw(MessageBoxBackgroundColor).stretched(-5).drawFrame(1, 0, MessageBoxTextColor);
			m_font(text).draw(14, m_messageBoxRect.stretched(-20, -20, -80, -20), MessageBoxTextColor);
		}
	};
}
#endif
