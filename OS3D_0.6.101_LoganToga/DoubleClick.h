#pragma once
class DoubleClick
{
public:

	void update()
	{
		if (m_step == 3)
		{
			m_step = 0;
		}

		if (MouseL.down())
		{
			if (m_step == 0)
			{
				m_step = 1;
			}
			else if (m_step == 2)
			{
				if (const uint64 d = (Time::GetMillisec() - m_previousTimeMillisec);
					d < DoubleClickThresholdMillisec)
				{
					m_step = 3;
				}
				else
				{
					m_step = 1;
				}
			}
		}

		if (m_step == 0)
		{
			return;
		}

		if (not Cursor::Delta().isZero())
		{
			m_step = 0;
		}

		if ((m_step == 1) && MouseL.up())
		{
			if (MouseL.pressedDuration() < Milliseconds{ MaxClickTimeMillisec })
			{
				m_step = 2;
				m_previousTimeMillisec = Time::GetMillisec();
			}
			else
			{
				m_step = 0;
			}
		}
	}

	[[nodiscard]]
	bool doubleClicked() const noexcept
	{
		return (m_step == 3);
	}

private:

	// 1 回目のクリックの長さ（ミリ秒）
	static constexpr int32 MaxClickTimeMillisec = 500;

	// 1 回目のクリックと 2 回目のクリックの最大間隔（ミリ秒）
	static constexpr int32 DoubleClickThresholdMillisec = 500;

	int32 m_step = 0;

	int64 m_previousTimeMillisec = 0;
};
