#pragma once
# include "157_EnumFlagBattleMapUnit.h" 

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
	static String ReplaceNewLine(String target)
	{
		return target.replaced(U"\r\n", U"")
			.replaced(U"\r", U"")
			.replaced(U"\n", U"")
			.replaced(U" ", U"")
			.replaced(U"\t", U"");
	}
	static ClassMapBattle GetClassMapBattle(ClassMap cm)
	{
		ClassMapBattle cmb;
		cmb.name = cm.name;
		for (String aaa : cm.data)
		{
			Array<MapDetail> aMd;
			Array s = aaa.split(U',');
			for (auto bbb : s)
			{
				MapDetail md;

				Array splitA = bbb.split(U'*');
				//field(床画像
				md.tip = cm.ele[splitA[0]];
				//build(城壁や矢倉など
				if (splitA.size() > 1)
				{
					if (splitA[1] == U"")
					{

					}
					else
					{
						Array splitB = splitA[1].split(U'$');
						//1件だけの場合も考慮
						if (splitB.size() == 1)
						{
							Array splitWi = splitB[0].split(U':');
							String re = cm.ele[splitWi[0]];
							if (re != U"")
							{
								std::tuple<String, long, BattleWhichIsThePlayer> pp = { re,-1, BattleWhichIsThePlayer::None };
								if (splitWi.size() == 1)
								{
									pp = { re,-1, BattleWhichIsThePlayer::None };
								}
								else
								{
									if (splitWi[1] == U"sor")
									{
										pp = { re,-1, BattleWhichIsThePlayer::Sortie };
									}
									else if (splitWi[1] == U"def")
									{
										pp = { re,-1, BattleWhichIsThePlayer::Def };
									}
									else
									{
										pp = { re,-1, BattleWhichIsThePlayer::None };
									}
								}

								md.building.push_back(pp);
							}
						}
						else
						{
							for (String item : splitB)
							{
								Array splitWi = item.split(U':');
								String re = cm.ele[splitWi[0]];
								if (re != U"")
								{
									std::tuple<String, long, BattleWhichIsThePlayer> pp = { re,-1, BattleWhichIsThePlayer::None };
									if (splitWi.size() == 1)
									{
										pp = { re,-1, BattleWhichIsThePlayer::None };
									}
									else
									{
										if (splitWi[1] == U"sor")
										{
											pp = { re,-1, BattleWhichIsThePlayer::Sortie };
										}
										else if (splitWi[1] == U"def")
										{
											pp = { re,-1, BattleWhichIsThePlayer::Def };
										}
										else
										{
											pp = { re,-1, BattleWhichIsThePlayer::None };
										}
									}

									md.building.push_back(pp);
								}
							}
						}
					}
				}
				//ユニットの情報
				if (splitA.size() > 2)
				{
					Array re = splitA[2].split(U':');
					if (re.size() == 0 || re[0] == U"-1")
					{

					}
					else
					{
						md.unit = re[0];
						md.houkou = re[1];
						//
						//md.BattleWhichIsThePlayer = re[2];
					}
				}
				//【出撃、防衛、中立の位置】もしくは【退却位置】
				if (splitA.size() > 3)
				{
					md.posSpecial = splitA[3];
					if (md.posSpecial == U"@@")
					{
						md.kougekiButaiNoIti = true;
					}else if (md.posSpecial == U"@")
					{
						md.boueiButaiNoIti = true;
					}
					else
					{

					}
				}
				//陣形
				if (splitA.size() > 5)
				{
					md.zinkei = splitA[5];
				}
				aMd.push_back(std::move(md));
			}
			cmb.mapData.push_back(std::move(aMd));
		}

		return cmb;
	}

private:
	ClassStaticCommonMethod();
};
