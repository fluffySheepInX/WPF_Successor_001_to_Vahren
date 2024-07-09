#pragma once
class ClassScenario
{
public:
	String ButtonType;
	String ScenarioName;
	int32 SortKey = -1;
	String HelpString;
	String SelectCharaFrameImageLeft;
	String SelectCharaFrameImageRight;
	String Text;
	String Mail;
	String Internet;
	Array<String> ArrayPower;
	RectF btnRectF = {};
};
