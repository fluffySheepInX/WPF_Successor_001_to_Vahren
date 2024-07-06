#pragma once
# include "000_SystemString.h"

const String PATHBASE = U"000_Warehouse/";
String PATHDEFAULTGAME = U"000_DefaultGame/";
const String PathFont = PATHBASE + PATHDEFAULTGAME + U"005_Font";

const String PathImage = PATHBASE + PATHDEFAULTGAME + U"005_image0001";
const String PathMusic = PATHBASE + PATHDEFAULTGAME + U"music001";
const String PathSound = PATHBASE + PATHDEFAULTGAME + U"015_sound001";
const String PathLang = PATHBASE + PATHDEFAULTGAME + U"001_lang";
const int32 WINDOWSIZEWIDTH000 = 1600;
const int32 WINDOWSIZEHEIGHT000 = 900;
const int32 WINDOWSIZEWIDTH001 = 1200;
const int32 WINDOWSIZEHEIGHT001 = 600;
const String STRINGSLICE9000 = (PathImage + U"/wnd0.png");
const String STRINGSLICE9001 = (PathImage + U"/wnd1.png");
const String FONTJA = (PathFont + U"/x12y12pxMaruMinya.ttf");

SystemString SYSTEMSTRING;
double SCALE = 1.0;
Vec2 OFFSET = { 0, 0 };
Polygon EXITBTNPOLYGON = {};
Rect EXITBTNRECT = {};

bool IS_DEBAGU_MODE = true;
bool IS_SCENE_MODAL_PAUSED = false;

Array<std::pair<String, String>> LANDIS = {
	{U"Japan",U"日本語" },
};
