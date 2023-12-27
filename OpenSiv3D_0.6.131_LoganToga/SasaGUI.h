#pragma once
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
