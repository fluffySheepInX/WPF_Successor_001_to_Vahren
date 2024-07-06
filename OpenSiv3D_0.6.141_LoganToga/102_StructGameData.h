﻿#pragma once
struct GameData {
	/// @brief 日本語
	const Font fontNormal = Font{ FontMethod::MSDF,50, FONTJA };
	const Font fontMini = Font{ FontMethod::MSDF,40, FONTJA };
	const Font fontLine = Font{ FontMethod::MSDF,25, FONTJA };
	const GameUIToolkit::Slice9 slice9{ STRINGSLICE9000, GameUIToolkit::Slice9::Style{
.backgroundRect = Rect{ 0, 0, 64, 64 },
	.frameRect = Rect{ 64, 0, 64, 64 },
	.cornerSize = 8,
	.backgroundRepeat = false
} };
	const GameUIToolkit::Slice9 slice9Talk{ STRINGSLICE9001, GameUIToolkit::Slice9::Style{
.backgroundRect = Rect{ 0, 0, 64, 64 },
	.frameRect = Rect{ 64, 0, 64, 64 },
	.cornerSize = 8,
	.backgroundRepeat = false
} };
};
