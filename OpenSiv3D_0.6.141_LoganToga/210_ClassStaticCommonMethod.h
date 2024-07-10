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
				//flag(部隊チップの種別
				if (splitA.size() > 2)
				{
					int32 re = -1;
					try
					{
						re = Parse<int32>(splitA[2]);
					}
					catch (const std::exception&)
					{
						throw;
					}

					if (re == 0)
					{
						md.flagBattleMapUnit = FlagBattleMapUnit::Unit;
					}
					else if (re == 1)
					{
						md.flagBattleMapUnit = FlagBattleMapUnit::Var;
					}
					else if (re == 2)
					{
						md.flagBattleMapUnit = FlagBattleMapUnit::Spe;
					}
					else if (re == -1)
					{

					}
					else
					{
						throw;
					}
				}
				//部隊
				if (splitA.size() > 3)
				{
					md.unit = splitA[3];
					if (md.unit == U"@@")
					{
						md.kougekiButaiNoIti = true;
					}
					if (md.unit == U"@")
					{
						md.boueiButaiNoIti = true;
					}
				}
				//方向
				if (splitA.size() > 4)
				{
					md.houkou = splitA[4];
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
