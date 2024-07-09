#pragma once
# include "000_SystemString.h"

const String PATHBASE = U"000_Warehouse/";
String PATH_DEFAULT_GAME = U"000_DefaultGame/";
const String PathFont = PATHBASE + PATH_DEFAULT_GAME + U"005_Font";

const String PathImage = PATHBASE + PATH_DEFAULT_GAME + U"005_image0001";
const String PathMusic = PATHBASE + PATH_DEFAULT_GAME + U"music001";
const String PathSound = PATHBASE + PATH_DEFAULT_GAME + U"015_sound001";
const String PathLang = PATHBASE + PATH_DEFAULT_GAME;
const int32 INIT_WINDOW_SIZE_WIDTH = 800;
const int32 INIT_WINDOW_SIZE_HEIGHT = 800;
const int32 WINDOWSIZEWIDTH000 = 1600;
const int32 WINDOWSIZEHEIGHT000 = 900;
const int32 WINDOWSIZEWIDTH001 = 1200;
const int32 WINDOWSIZEHEIGHT001 = 600;
const String STRINGSLICE9000 = (PATHBASE + PATH_DEFAULT_GAME + U"/000_SystemImage" + U"/wnd0.png");
const String STRINGSLICE9001 = (PATHBASE + PATH_DEFAULT_GAME + U"/000_SystemImage" + U"/wnd1.png");
const String STRINGSLICE9002 = (PATHBASE + PATH_DEFAULT_GAME + U"/000_SystemImage" + U"/wnd2.png");
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
