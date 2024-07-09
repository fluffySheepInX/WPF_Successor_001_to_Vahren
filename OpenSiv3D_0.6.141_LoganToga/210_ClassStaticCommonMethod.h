#pragma once
class ClassStaticCommonMethod {
public:
	static String MoldingScenarioText(String target)
	{
		return target
			.replaced(U"$", U"\r\n")
			.replaced(U"\\", U"")
			.replaced(U"\t", U"")
			.replaced(U"@", U" ");
	}
private:
	ClassStaticCommonMethod();
};
