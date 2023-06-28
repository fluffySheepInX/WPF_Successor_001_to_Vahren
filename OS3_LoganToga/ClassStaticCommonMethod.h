#pragma once
# include "ClassMap.h" 
# include "ClassMapBattle.h" 
# include "ClassGameStatus.h" 
# include "ClassUnit.h" 
# include "ClassHorizontalUnit.h" 
# include "ClassObjectMapTip.h" 

class ClassStaticCommonMethod {
public:
	static ClassConfigString GetClassConfigString(String lan)
	{
		// TOML ファイルからデータを読み込む
		const TOMLReader tomlSystemString{ U"001_Warehouse/001_DefaultGame/SystemString.toml" };

		if (not tomlSystemString) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load `SystemString.toml`" };
		}

		ClassConfigString ccs;
		for (const auto& table : tomlSystemString[U"SystemString"].tableArrayView()) {
			String lan = table[U"lang"].get<String>();
			if (lan == U"en")
			{
				ccs.configSave = table[U"configSave"].get<String>();
				ccs.configLoad = table[U"configLoad"].get<String>();
				ccs.selectScenario = table[U"selectScenario"].get<String>();
				ccs.selectScenario2 = table[U"selectScenario2"].get<String>();
				ccs.selectChara1 = table[U"selectChara1"].get<String>();
				ccs.DoYouWantToQuitTheGame = table[U"DoYouWantToQuitTheGame"].get<String>();
				ccs.strategyMenu000 = table[U"strategyMenu000"].get<String>();
				ccs.strategyMenu001 = table[U"strategyMenu001"].get<String>();
				ccs.strategyMenu002 = table[U"strategyMenu002"].get<String>();
				ccs.strategyMenu003 = table[U"strategyMenu003"].get<String>();
				ccs.strategyMenu004 = table[U"strategyMenu004"].get<String>();
				ccs.strategyMenu005 = table[U"strategyMenu005"].get<String>();
				ccs.strategyMenu006 = table[U"strategyMenu006"].get<String>();
				ccs.strategyMenu007 = table[U"strategyMenu007"].get<String>();
				ccs.strategyMenu008 = table[U"strategyMenu008"].get<String>();
				ccs.strategyMenu009 = table[U"strategyMenu009"].get<String>();
			}
		}

		return ccs;
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
					Array splitB = splitA[1].split(U'$');
					for (String item : splitB)
					{
						String re = cm.ele[item];
						if (re != U"")
						{
							HashTable<String, long> tar;
							tar.emplace(re, -1);
							md.building.push_back(tar);
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
	static String ReplaceNewLine(String target)
	{
		return target.replaced(U"\r\n", U"")
			.replaced(U"\r", U"")
			.replaced(U"\n", U"")
			.replaced(U" ", U"")
			.replaced(U"\t", U"");
	}
	static String MoldingScenarioText(String target)
	{
		return target
			.replaced(U" ", U"")
			.replaced(U"\t", U"")
			.replaced(U"@", U" ");
	}
	static String MoldingPowerText(String target)
	{
		return target
			.replaced(U" ", U"")
			.replaced(U"\t", U"")
			.replaced(U"@", U" ");
	}
	static void AddBuilding(ClassGameStatus* gameStatus)
	{
		if (gameStatus == nullptr) return;
		if (gameStatus->classBattle.classMapBattle.has_value() == false) return;
		if (gameStatus->classBattle.classMapBattle.value().mapData.size() == 0) return;

		// Building settings
		Array<std::tuple<String, int, int>> buildings;
		for (int row = 0; row < gameStatus->classBattle.classMapBattle.value().mapData.size(); ++row)
		{
			for (int col = 0; col < gameStatus->classBattle.classMapBattle.value().mapData[row].size(); ++col)
			{
				if (gameStatus->classBattle.classMapBattle.value().mapData[row][col].building.size() != 0)
				{
					for (const auto& building : gameStatus->classBattle.classMapBattle.value().mapData[row][col].building)
					{
						for (auto [key, value] : building)
						{
							buildings.push_back({ key, row, col });
						}
					}
				}
			}
		}

		// Assign IDs to buildings and map details
		Array<ClassUnit> units;
		ClassHorizontalUnit horizontalUnit;
		horizontalUnit.FlagBuilding = true;
		for (const auto& item : buildings)
		{
			auto found = std::find_if(gameStatus->arrayClassObjectMapTip.begin(), gameStatus->arrayClassObjectMapTip.end(),
									  [&item](const auto& object) { return object.nameTag == std::get<0>(item) && object.type == MapTipObjectType::GATE; });
			if (found == gameStatus->arrayClassObjectMapTip.end()) continue;
			long id = gameStatus->getIDCount();

			ClassUnit cu;
			cu.IsBuilding = true;
			cu.rowBuilding = std::get<1>(item);
			cu.colBuilding = std::get<2>(item);
			cu.mapTipObjectType = MapTipObjectType::GATE;
			cu.HPCastle = found->castle;
			cu.CastleDefense = found->castleDefense;
			cu.CastleMagdef = found->castleMagdef;
			cu.Image = found->nameTag;
			units.push_back(cu);

			gameStatus->classBattle.classMapBattle.value().mapData[std::get<1>(item)][std::get<2>(item)].building[0][std::get<0>(item)] = id;
		}
		horizontalUnit.ListClassUnit = units;
		gameStatus->classBattle.defUnitGroup.push_back(horizontalUnit);
	}
private:
	ClassStaticCommonMethod();
};
