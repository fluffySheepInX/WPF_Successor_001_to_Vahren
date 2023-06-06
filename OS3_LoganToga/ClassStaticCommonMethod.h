#pragma once
# include "ClassMap.h" 
# include "ClassMapBattle.h" 

class ClassStaticCommonMethod {
public:
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
				if (splitA.size() >= 1)
				{
					Array splitB = splitA[0].split(U'$');
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
				if (splitA.size() >= 2)
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
				if (splitA.size() >= 3)
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
				if (splitA.size() >= 4)
				{
					md.houkou = splitA[4];
				}
				//陣形
				if (splitA.size() >= 5)
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
private:
	ClassStaticCommonMethod();
};
