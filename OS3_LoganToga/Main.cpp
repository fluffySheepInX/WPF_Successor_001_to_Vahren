# include <Siv3D.hpp> // OpenSiv3D v0.6.9
# include "ClassConfigString.h" 
# include "ClassGameStatus.h" 
# include "ClassMap.h" 
# include "ClassTestBattle.h" 
# include "ClassUnit.h" 
# include "ClassObjectMapTip.h"
# include "ClassBattle.h" 
# include "ClassScenario.h" 
# include "ClassStaticCommonMethod.h" 
# include "MapCreator.h" 
# include "DoubleClick.h" 
# include "SasaGUI.h" 
# include "ClassExecutedSkillInBattle.h" 
# include "ClassPower.h" 
# include "GameUIToolkit.h" 
#include <ranges>
# include "Enum.h" 
const bool debug = true;
const String newlineCode = U"\r\n";
// ウィンドウの幅
const int32 WindowSizeWidth = 1920;
// ウィンドウの高さ
const int32 WindowSizeHeight = 1017;
const String PathImage = U"image0001";
const String PathMusic = U"music001";
const String PathSound = U"sound001";

Language language;

// フェードイン・フェードアウトの描画をするクラス
struct IFade
{
	virtual ~IFade() = default;
	virtual void fade(double t) = 0;
};
struct Fade4 : public IFade
{
	Array<int> rectPos;
	const int size = 50;
	const int w = Scene::Width() / size;
	const int h = Scene::Height() / size;

	Fade4()
	{
		rectPos = Iota(w * h);
		Shuffle(rectPos);
	}

	void fade(double t) override
	{
		for (auto [index, pos] : Indexed(rectPos))
		{
			if (index > t * w * h) break;

			const auto x = pos % w;
			const auto y = pos / w;
			Rect{ x * size, y * size, size }.draw(Color{ 0,0,0,255 });
		}
	}
};

// フェード描画クラスのインスタンスをランダムに返す
auto randomFade()
{
	Array<std::function<std::unique_ptr<IFade>()>> makeFadeFuncs = {
		[]() -> std::unique_ptr<IFade> { return std::make_unique<Fade4>(); },
	};

	return Sample(makeFadeFuncs)();
}

/// @brief 共有するデータ構造体
struct GameData
{
	/// @brief 
	Font fontNormal = Font{ 50, U"font/DotGothic16-Regular.ttf" ,FontStyle::Bitmap };
	/// @brief 
	Font fontHeadline = Font{ 80, U"font/DotGothic16-Regular.ttf" ,FontStyle::Bitmap };
	/// @brief 
	Font fontTitle = Font{ 80, U"font/DotGothic16-Regular.ttf",FontStyle::BoldItalic };
	/// @brief 
	Font fontSelectChar1 = Font{ 50, U"font/DotGothic16-Regular.ttf" ,FontStyle::Bitmap };
	Font fontSelectChar2 = Font{ 30, U"font/DotGothic16-Regular.ttf" ,FontStyle::Bitmap };
	Font fontScenarioMenu = Font{ 40, U"font/DotGothic16-Regular.ttf" ,FontStyle::Bitmap };
	Font fontNovelHelp = Font{ 30, U"font/DotGothic16-Regular.ttf" ,FontStyle::Bitmap };
	const Slice9 slice9{ U"001_Warehouse/001_DefaultGame/001_SystemImage/wnd0.png", Slice9::Style{
	.backgroundRect = Rect{ 0, 0, 64, 64 },
	.frameRect = Rect{ 64, 0, 64, 64 },
	.cornerSize = 8,
	.backgroundRepeat = false
} };

	ClassGameStatus classGameStatus;
	ClassConfigString classConfigString;
	ClassScenario selectClassScenario;
	ClassPower selectClassPower;
	String NovelPower = U"";
	String NovelNumber = U"";
};

using App = SceneManager<String, GameData>;

/// @brief 言語選択シーン
class SelectLang : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	SelectLang(const InitData& init)
		: IScene{ init }
	{
		language = Language::English;
		// 画像を使用する為のTexture宣言
		String lan001 = PathImage + U"/waku0001.png";
		String lan002 = PathImage + U"/waku0002.png";
		textureWaku001 = Texture{ lan001, TextureDesc::Unmipped };
		textureWaku002 = Texture{ lan002, TextureDesc::Unmipped };
		int32 yohakuHidari = (Scene::Size().x / 2) - (wakuZentaiYoko / 2);
		// 左縦
		for (size_t i = 0; i < wakuTateLength; i++)
		{
			rectWaku001Hidari.push_back(Rect{ yohakuHidari, 60 + (i * wakuHenPX),wakuHenPX,wakuHenPX });
		}
		// 右縦
		for (size_t i = 0; i < wakuTateLength; i++)
		{
			rectWaku001Migi.push_back(Rect{ Scene::Size().x - yohakuHidari - wakuHenPX, 60 + (i * wakuHenPX),wakuHenPX,wakuHenPX });
		}
		// 上横
		for (size_t i = 0; i < wakuTateLength; i++)
		{
			rectWaku001Ue.push_back(Rect{ yohakuHidari + wakuHenPX + (i * wakuHenPX), 60 ,wakuHenPX,wakuHenPX });
		}
		// 下横
		for (size_t i = 0; i < wakuTateLength; i++)
		{
			rectWaku001Shita.push_back(Rect{ yohakuHidari + wakuHenPX + (i * wakuHenPX), 810 ,wakuHenPX,wakuHenPX });
		}
		// 左縦
		for (size_t i = 0; i < wakuTateLength002; i++)
		{
			rectWaku002Hidari.push_back(Rect{ yohakuHidari - 15, 60 + (i * wakuHenPX002),wakuHenPX002,wakuHenPX002 });
		}
		// 右縦
		for (size_t i = 0; i < wakuTateLength002; i++)
		{
			rectWaku002Migi.push_back(Rect{ Scene::Size().x - yohakuHidari - wakuHenPX002 + 15, 60 + (i * wakuHenPX002),wakuHenPX002,wakuHenPX002 });
		}
		// 上横
		for (size_t i = 0; i < wakuTateLength002; i++)
		{
			rectWaku002Ue.push_back(Rect{ yohakuHidari + (i * wakuHenPX002) + (150), 45 ,wakuHenPX002,wakuHenPX002 });
		}
		// 下横
		for (size_t i = 0; i < wakuTateLength002; i++)
		{
			rectWaku002Shita.push_back(Rect{ yohakuHidari + (i * wakuHenPX002) + (150), 960 ,wakuHenPX002,wakuHenPX002 });
		}

		text1 = U"English";
		text2 = U"日本語";
		text3 = U"Select";
		text4 = U"Language";
		// fontNormal を使って textPlayGame を pos の位置に描画したときのテキストの領域を取得
		RectF rectText = getData().fontNormal(text1).region();
		RectF rectText2 = getData().fontNormal(text2).region();
		RectF rectText3 = getData().fontHeadline(text3).region();
		RectF rectTex4 = getData().fontHeadline(text4).region();

		buttonEngX = (Scene::Size().x / 2) - (rectText.w / 2);
		buttonEngY = (Scene::Size().y - 500);
		buttonJaX = (Scene::Size().x / 2) - (rectText2.w / 2);
		buttonJaY = (Scene::Size().y - 390);
		lblSelectX = (Scene::Size().x / 2) - (rectText3.w) - 30;
		lblSelectY = (Scene::Size().y - 700);
		lblLanguageX = (Scene::Size().x / 2) + 30;
		lblLanguageY = (Scene::Size().y - 650);

		int32 marumi = 10;
		rectBtnEng = { buttonEngX, buttonEngY,rectText.w,rectText.h ,marumi };
		rectBtnJa = { buttonJaX, buttonJaY,rectText2.w,rectText2.h,marumi };
		rectSelect = { lblSelectX, lblSelectY,rectText3.w,rectText3.h,marumi };
		rectLanguage = { lblLanguageX, lblLanguageY,rectTex4.w,rectTex4.h,marumi };
	}

	// 更新関数（オプション）
	void update() override
	{
		//押されたら次の画面へ進む
		if (rectBtnEng.leftClicked())
		{
			AudioAsset(U"click").play();
			language = Language::English;

			getData().classConfigString = ClassStaticCommonMethod::GetClassConfigString(U"en");

			// 遷移
			changeScene(U"Title", 0.9s);
		}
		if (rectBtnJa.leftClicked())
		{
			AudioAsset(U"click").play();
			language = Language::Japan;

			getData().classConfigString = ClassStaticCommonMethod::GetClassConfigString(U"jp");

			// 遷移
			changeScene(U"Title", 0.9s);
		}
	}

	// 描画関数（オプション）
	void draw() const override
	{
		//// 背景を描画
		// 左縦
		for (size_t i = 0; i < wakuTateLength; i++)
		{
			rectWaku001Hidari[i](textureWaku001).draw();
		}
		// 右縦
		for (size_t i = 0; i < wakuTateLength; i++)
		{
			rectWaku001Migi[i](textureWaku001).draw();
		}
		// 上横
		for (size_t i = 0; i < wakuTateLength; i++)
		{
			rectWaku001Ue[i](textureWaku001).draw();
		}
		// 下横
		for (size_t i = 0; i < wakuTateLength; i++)
		{
			rectWaku001Shita[i](textureWaku001).draw();
		}
		// 左縦
		for (size_t i = 0; i < wakuTateLength002; i++)
		{
			rectWaku002Hidari[i](textureWaku002).draw();
		}
		// 右縦
		for (size_t i = 0; i < wakuTateLength002; i++)
		{
			rectWaku002Migi[i](textureWaku002).draw();
		}
		// 上横
		for (size_t i = 0; i < wakuTateLength002; i++)
		{
			rectWaku002Ue[i](textureWaku002).draw();
		}
		// 下横
		for (size_t i = 0; i < wakuTateLength002; i++)
		{
			rectWaku002Shita[i](textureWaku002).draw();
		}

		////ボタン描写
		rectBtnEng.draw(Arg::top = ColorF{ 0.5 }, Arg::bottom = ColorF{ 1.0 }).drawFrame(0, 6, Palette::Orange);
		rectBtnJa.draw(Arg::top = ColorF{ 0.5 }, Arg::bottom = ColorF{ 1.0 }).drawFrame(0, 6, Palette::Orange);
		getData().fontNormal(text1).draw(buttonEngX, buttonEngY, ColorF{ 0.25 });
		getData().fontNormal(text2).draw(buttonJaX, buttonJaY, ColorF{ 0.25 });
		getData().fontHeadline(text3).draw(lblSelectX, lblSelectY, Palette::White);
		getData().fontHeadline(text4).draw(lblLanguageX, lblLanguageY, Palette::White);
	}

	void drawFadeIn(double t) const override
	{
		draw();

		m_fadeInFunction->fade(1 - t);
	}

	void drawFadeOut(double t) const override
	{
		draw();

		m_fadeOutFunction->fade(t);
	}
private:
	////枠情報
	const int32 wakuTateLength = 6;
	const int32 wakuHenPX = 150;
	const int32 wakuTateLength002 = 60;
	const int32 wakuHenPX002 = 15;
	const int32 wakuZentaiYoko = wakuHenPX * wakuTateLength + (wakuHenPX * 2);
	const int32 wakuZentaiTate = wakuHenPX * wakuTateLength;
	Texture textureWaku001;
	Texture textureWaku002;
	////枠配列
	//左縦
	Array<Rect> rectWaku001Hidari;
	//右縦
	Array<Rect> rectWaku001Migi;
	//上横
	Array<Rect> rectWaku001Ue;
	//下横
	Array<Rect> rectWaku001Shita;
	//左縦
	Array<Rect> rectWaku002Hidari;
	//右縦
	Array<Rect> rectWaku002Migi;
	//上横
	Array<Rect> rectWaku002Ue;
	//下横
	Array<Rect> rectWaku002Shita;
	////ボタン情報
	String text1 = U"en";
	int32 buttonEngX = -1;
	int32 buttonEngY = -1;
	String text2 = U"jp";
	int32 buttonJaX = -1;
	int32 buttonJaY = -1;
	String text3;
	int32 lblSelectX = -1;
	int32 lblSelectY = -1;
	String text4;
	int32 lblLanguageX = -1;
	int32 lblLanguageY = -1;
	RoundRect rectBtnEng;
	RoundRect rectBtnJa;
	RoundRect rectSelect;
	RoundRect rectLanguage;

	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
};
class TestBattle : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	TestBattle(const InitData& init)
		: IScene{ init }
	{
	}
	// 更新関数（オプション）
	void update() override
	{
		if (execute == false)
		{
			// TOML ファイルからデータを読み込む
			const TOMLReader tomlConfig{ U"001_Warehouse/001_DefaultGame/config.toml" };

			if (not tomlConfig) // もし読み込みに失敗したら
			{
				throw Error{ U"Failed to load `config.toml`" };
			}

			// TOML ファイルからデータを読み込む
			const TOMLReader tomlSystemString{ U"001_Warehouse/001_DefaultGame/SystemString.toml" };

			if (not tomlSystemString) // もし読み込みに失敗したら
			{
				throw Error{ U"Failed to load `SystemString.toml`" };
			}

			Array<ClassConfigString> arrayClassConfigString;
			for (const auto& table : tomlSystemString[U"SystemString"].tableArrayView()) {
				ClassConfigString ccs;

				ccs.lang = table[U"lang"].get<String>();
				ccs.configSave = table[U"configSave"].get<String>();
				ccs.configLoad = table[U"configLoad"].get<String>();
				ccs.selectScenario = table[U"selectScenario"].get<String>();
				ccs.selectScenario2 = table[U"selectScenario2"].get<String>();
				ccs.selectChara1 = table[U"selectChara1"].get<String>();
				ccs.DoYouWantToQuitTheGame = table[U"DoYouWantToQuitTheGame"].get<String>();

				arrayClassConfigString << ccs;
			}

			//TODO
			getData().classConfigString = arrayClassConfigString[0];

			// TOML ファイルからデータを読み込む
			const TOMLReader tomlTestBattle{ U"001_Warehouse/001_DefaultGame/070_Scenario/InfoTestBattle/testBattle.toml" };

			if (not tomlTestBattle) // もし読み込みに失敗したら
			{
				throw Error{ U"Failed to load `testBattle.toml`" };
			}

			Array<ClassTestBattle> arrayStructTestBattle;
			for (const auto& table : tomlTestBattle[U"TestBattle"].tableArrayView()) {
				ClassTestBattle tb;
				tb.name = table[U"name"].get<String>();
				tb.map = table[U"map"].get<String>();
				{
					const String str = table[U"memberKougeki"].get<String>();
					const Array<String> strArray = str.split(U',');
					for (auto& s : strArray)
					{
						tb.memberKougeki.push_back(s);
					}
				}
				{
					const String str = table[U"memberBouei"].get<String>();
					const Array<String> strArray = str.split(U',');
					for (auto& s : strArray)
					{
						tb.memberBouei.push_back(s);
					}
				}
				{
					const String str = table[U"player"].get<String>();
					BattleWhichIsThePlayer z;
					if (str == U"Def")
					{
						z = BattleWhichIsThePlayer::Def;
					}
					else if (str == U"Sortie")
					{
						z = BattleWhichIsThePlayer::Sortie;
					}
					else
					{
						z = BattleWhichIsThePlayer::None;
					}
					tb.player = z;
				}

				arrayStructTestBattle << tb;
			}

			// TOML ファイルからデータを読み込む
			const TOMLReader tomlTestMap{ U"001_Warehouse/001_DefaultGame/070_Scenario/InfoTestBattle/testMap.toml" };

			if (not tomlTestMap) // もし読み込みに失敗したら
			{
				throw Error{ U"Failed to load `testMap.toml`" };
			}

			ClassMap sM;
			for (const auto& table : tomlTestMap[U"Map"].tableArrayView()) {
				const String name = table[U"name"].get<String>();

				{
					int32 counter = 0;
					while (true)
					{
						String aaa = U"ele{}"_fmt(counter);
						const String ele = table[aaa].get<String>();
						sM.ele.emplace(aaa, ele);
						counter++;
						if (ele == U"")
						{
							break;
						}
					}
				}
				{
					namespace views = std::views;
					const String str = table[U"data"].get<String>();
					for (const auto sv : str | views::split(U",@,"_sv))
					{
						String re = ClassStaticCommonMethod::ReplaceNewLine(String(sv.begin(), sv.end()));
						if (re != U"")
						{
							sM.data.push_back(ClassStaticCommonMethod::ReplaceNewLine(re));
						}
					}
				}
			}

			// TOML ファイルからデータを読み込む
			const JSON jsonUnit = JSON::Load(U"001_Warehouse/001_DefaultGame/070_Scenario/InfoUnit/Unit.json");

			if (not jsonUnit) // もし読み込みに失敗したら
			{
				throw Error{ U"Failed to load `Unit.json`" };
			}

			Array<ClassUnit> arrayClassUnit;
			for (const auto& [key, value] : jsonUnit[U"Unit"]) {
				ClassUnit cu;
				cu.NameTag = value[U"name_tag"].getString();
				cu.Name = value[U"name"].getString();
				cu.Image = value[U"image"].getString();
				cu.Speed = Parse<double>(value[U"speed"].getString());
				String sNa = value[U"skill"].getString();
				if (sNa.contains(',') == true)
				{
					cu.SkillName = sNa.split(',');
				}
				else
				{
					cu.SkillName.push_back(sNa);
				}
				arrayClassUnit.push_back(std::move(cu));
			}

			// TOML ファイルからデータを読み込む
			const JSON skillData = JSON::Load(U"001_Warehouse/001_DefaultGame/070_Scenario/InfoSkill/skill.json");

			if (not skillData) // もし読み込みに失敗したら
			{
				throw Error{ U"Failed to load `skill.json`" };
			}

			Array<ClassSkill> arrayClassSkill;
			for (const auto& [key, value] : skillData[U"Skill"]) {
				ClassSkill cu;
				cu.nameTag = value[U"name_tag"].get<String>();
				cu.name = value[U"name"].get<String>();

				arrayClassSkill.push_back(std::move(cu));
			}

			// TOML ファイルからデータを読み込む
			const JSON objData = JSON::Load(U"001_Warehouse/001_DefaultGame/070_Scenario/Info_Object/obj.json");

			if (not objData) // もし読み込みに失敗したら
			{
				throw Error{ U"Failed to load `obj.json`" };
			}

			Array<ClassObjectMapTip> arrayClassObj;
			for (const auto& [key, value] : objData[U"obj"]) {
				ClassObjectMapTip cu;
				cu.nameTag = value[U"name"].get<String>();
				String ty = value[U"type"].get<String>();
				if (ty == U"wall2")
				{
					cu.type = MapTipObjectType::WALL2;
				}
				else if (ty == U"gate")
				{
					cu.type = MapTipObjectType::GATE;
				}
				cu.noWall2 = value[U"no_wall2"].get<int32>();
				cu.castle = value[U"castle"].get<int32>();
				cu.castleDefense = value[U"castle_defense"].get<int32>();
				cu.castleMagdef = value[U"castle_magdef"].get<int32>();

				arrayClassObj.push_back(std::move(cu));
			}

			//unitのスキル名からスキルクラスを探し、unitに格納
			for (auto& itemUnit : arrayClassUnit)
			{
				for (const auto& itemSkillName : itemUnit.SkillName)
				{
					for (const auto& skill : arrayClassSkill)
					{
						if (skill.nameTag == itemSkillName)
						{
							itemUnit.Skill.emplace_back(skill);
							break;
						}
					}
				}
			}

			ClassGameStatus cgs;
			cgs.classTestBattle = arrayStructTestBattle[0];
			cgs.arrayClassMap = Array{ sM };
			cgs.arrayClassUnit = arrayClassUnit;
			cgs.arrayClassSkill = arrayClassSkill;
			cgs.arrayClassObjectMapTip = arrayClassObj;
			{
				ClassBattle cb;
				cb.classMapBattle = ClassStaticCommonMethod::GetClassMapBattle(sM);
				cb.battleWhichIsThePlayer = arrayStructTestBattle[0].player;

				for (String var : arrayStructTestBattle[0].memberKougeki)
				{
					Array re = var.split(U'*');
					if (re.size() == 1)
					{
						ClassHorizontalUnit chu;
						auto it = std::find_if(cgs.arrayClassUnit.begin(), cgs.arrayClassUnit.end(),
							[&](const ClassUnit& unit) { return unit.NameTag == re[0]; });
						if (it == cgs.arrayClassUnit.end())
						{
							continue;
						}
						chu.ListClassUnit.push_back(*it);
						cb.sortieUnitGroup.push_back(chu);
					}
					else
					{
						ClassHorizontalUnit chu;
						auto it = std::find_if(cgs.arrayClassUnit.begin(), cgs.arrayClassUnit.end(),
							[&](const ClassUnit& unit) { return unit.NameTag == re[0]; });
						if (it == cgs.arrayClassUnit.end())
						{
							continue;
						}
						for (size_t i = 0; i < Parse<int32>(re[1]); i++)
						{
							chu.ListClassUnit.push_back(*it);
						}
						cb.sortieUnitGroup.push_back(chu);
					}
				}
				for (String var : arrayStructTestBattle[0].memberBouei)
				{
					Array re = var.split(U'*');
					if (re.size() == 1)
					{
						ClassHorizontalUnit chu;
						auto it = std::find_if(cgs.arrayClassUnit.begin(), cgs.arrayClassUnit.end(),
							[&](const ClassUnit& unit) { return unit.NameTag == re[0]; });
						if (it == cgs.arrayClassUnit.end())
						{
							continue;
						}
						chu.ListClassUnit.push_back(*it);
						cb.defUnitGroup.push_back(chu);
					}
					else
					{
						ClassHorizontalUnit chu;
						auto it = std::find_if(cgs.arrayClassUnit.begin(), cgs.arrayClassUnit.end(),
							[&](const ClassUnit& unit) { return unit.NameTag == re[0]; });
						if (it == cgs.arrayClassUnit.end())
						{
							continue;
						}
						for (size_t i = 0; i < Parse<int32>(re[1]); i++)
						{
							chu.ListClassUnit.push_back(*it);
						}
						cb.defUnitGroup.push_back(chu);
					}
				}

				cgs.classBattle = cb;
			}

			cgs.DistanceBetweenUnit = tomlConfig[U"config.DistanceBetweenUnit"].get<int32>();
			cgs.DistanceBetweenUnitTate = tomlConfig[U"config.DistanceBetweenUnitTate"].get<int32>();
			ClassStaticCommonMethod::AddBuilding(&cgs);
			getData().classGameStatus = cgs;
			execute = true;
		}
		else
		{
			changeScene(U"Battle", 0.9s);
		}
	}
	// 描画関数（オプション）
	void draw() const override
	{

	}

	void drawFadeIn(double t) const override
	{
		draw();

		m_fadeInFunction->fade(1 - t);
	}

	void drawFadeOut(double t) const override
	{
		draw();

		m_fadeOutFunction->fade(t);
	}
private:
	bool execute = false;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
};
class Battle : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	Battle(const InitData& init)
		: IScene{ init }
	{
		////マップ
		// maptip フォルダ内のファイルを列挙する
		for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/015_BattleMapCellImage/"))
		{
			String filename = FileSystem::FileName(filePath);
			TextureAsset::Register(filename, filePath);
		}
		for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/040_ChipImage/"))
		{
			String filename = FileSystem::FileName(filePath);
			TextureAsset::Register(filename, filePath);
		}

		mapCreator.N = getData().classGameStatus.classBattle.classMapBattle.value().mapData.size();

		// 各列の四角形
		columnQuads = mapCreator.MakeColumnQuads(mapCreator.N);
		// 各行の四角形
		rowQuads = mapCreator.MakeRowQuads(mapCreator.N);

		Grid<int32> gridWork(Size{ mapCreator.N, mapCreator.N });
		mapCreator.grid = gridWork;

		//始点設定
		viewPos = { 0,0 };

		int32 counterXSor = 0;
		int32 counterYSor = 0;
		bool flagSor = false;
		for (const auto target : getData().classGameStatus.classBattle.classMapBattle.value().mapData)
		{
			for (const auto wid : target)
			{
				if (wid.kougekiButaiNoIti == true)
				{
					flagSor = true;
					break;
				}
				else
				{
					counterYSor++;
				}
			}

			if (flagSor == true)
			{
				break;
			}
			counterYSor = 0;
			counterXSor++;
		}
		int32 counterXDef = 0;
		int32 counterYDef = 0;
		bool flagDef = false;
		for (const auto target : getData().classGameStatus.classBattle.classMapBattle.value().mapData)
		{
			for (const auto wid : target)
			{
				if (wid.boueiButaiNoIti == true)
				{
					flagDef = true;
					break;
				}
				else
				{
					counterYDef++;
				}
			}

			if (flagDef == true)
			{
				break;
			}
			counterYDef = 0;
			counterXDef++;
		}

		//ユニットの初期位置設定
		for (auto& item : getData().classGameStatus.classBattle.sortieUnitGroup)
		{
			if (!item.FlagBuilding &&
				!item.ListClassUnit.empty())
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					Point pt = Point(counterXSor, counterYSor);
					Vec2 reV = mapCreator.ToTileBottomCenter(pt, mapCreator.N);
					itemUnit.nowPosiLeft = Vec2(reV.x + Random(-50, 50), reV.y + Random(-50, 50));
				}
			}
		}
		for (auto& item : getData().classGameStatus.classBattle.defUnitGroup)
		{
			if (!item.FlagBuilding &&
				!item.ListClassUnit.empty())
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					Point pt = Point(counterXDef, counterYDef);
					Vec2 reV = mapCreator.ToTileBottomCenter(pt, mapCreator.N);
					itemUnit.nowPosiLeft = Vec2(reV.x + Random(-50, 50), reV.y + Random(-50, 50));
				}
			}
		}
		//建築物
		for (const auto& x : getData().classGameStatus.classBattle.defUnitGroup)
		{
			if (x.FlagBuilding == true)
			{
				bui.push_back(x);
			}
		}
	}
	// 更新関数（オプション）
	void update() override
	{
		const auto t = camera.createTransformer();

		if (MouseL.pressed() == true)
		{
			viewPos.moveBy(-Cursor::Delta());
			camera.jumpTo(viewPos, 1.0);
		}
		if (MouseR.pressed() == false && getData().classGameStatus.IsBattleMove == false)
		{
			if (MouseR.up() == false)
			{
				cursPos = Cursor::Pos();
			}
		}
		else if (MouseR.pressed() == false && getData().classGameStatus.IsBattleMove == true)
		{
			if (MouseR.up() == false)
			{
				cursPos = Cursor::Pos();
			}
		}
		else if (MouseR.down() == true && getData().classGameStatus.IsBattleMove == true)
		{
			cursPos = Cursor::Pos();
		}

		if (MouseR.up() == true)
		{
			Point start = cursPos;
			Point end = Cursor::Pos();
			//部隊を選択状態にする。もしくは既に選択状態なら移動させる
			Array<ClassHorizontalUnit>* lisClassHorizontalUnit;
			switch (getData().classGameStatus.classBattle.battleWhichIsThePlayer)
			{
			case BattleWhichIsThePlayer::Sortie:
				lisClassHorizontalUnit = &getData().classGameStatus.classBattle.sortieUnitGroup;
				break;
			case BattleWhichIsThePlayer::Def:
				lisClassHorizontalUnit = &getData().classGameStatus.classBattle.defUnitGroup;
				break;
			case BattleWhichIsThePlayer::None:
				//AI同士の戦いにフラグは立てない
				return;
			default:
				return;
			}

			if (getData().classGameStatus.IsBattleMove == true)
			{
				Array<ClassUnit*> lisUnit;
				for (auto& target : *lisClassHorizontalUnit)
				{
					for (auto& unit : target.ListClassUnit)
					{
						if (unit.FlagMove == true)
						{
							lisUnit.push_back(&unit);
						}
					}
				}

				if (lisUnit.size() == 1)
				{
					ClassUnit* cu = lisUnit[0];
					// 移動先の座標算出
					cu->orderPosiLeft = end;
					Vec2 nor = Vec2(end - start).normalized();
					Vec2 moved = cu->nowPosiLeft + Vec2(nor.x * cu->Speed, nor.y * cu->Speed);
					//マップチップ座標に変換している
					auto index = mapCreator.ToIndex(moved, columnQuads, rowQuads);
					if (not index.has_value())
					{
						cu->FlagMove = false;
						return;
					}

					//移動
					cu->vecMove = Vec2(cu->orderPosiLeft - cu->nowPosiLeft).normalized();
					cu->orderPosiLeft = end;
					cu->FlagMove = false;
					cu->FlagMoving = true;
				}

				int32 counterLisClassHorizontalUnit = 0;
				for (auto& target : *lisClassHorizontalUnit)
				{
					Array<ClassUnit*> re;
					for (auto& unit : target.ListClassUnit)
					{
						if (unit.FlagMove == true)
						{
							re.push_back(&unit);
						}
					}
					if (re.size() == 0)
					{
						counterLisClassHorizontalUnit++;
						continue;
					}

					//その部隊の人数を取得
					int32 unitCount = re.size();

					//商の数
					int32 result = unitCount / 2;

					//角度
					// X軸との角度を計算
					//θ'=直線とx軸のなす角度
					double angle2 = Math::Atan2(end.y - start.y,
										   end.x - start.x);
					//θ
					double angle = Math::Pi / 2 - angle2;

					//移動フラグが立っているユニットだけ、繰り返す
					//偶奇判定
					if (unitCount % 2 == 1)
					{
						////奇数の場合
						int32 counter = 0;
						for (auto& unit : re)
						{
							//px+(b-切り捨て商)＊dcosθ+a＊d'cosθ’
							double xPos = end.x
								+ (
									(counter - (result))
									* (getData().classGameStatus.DistanceBetweenUnit * Math::Cos(angle))
									)
								-
								(counterLisClassHorizontalUnit * (getData().classGameStatus.DistanceBetweenUnitTate * Math::Cos(angle2)));
							//py+(b-切り捨て商)＊dsinθ-a＊d'sinθ’
							double yPos = end.y
								- (
								(counter - (result))
								* (getData().classGameStatus.DistanceBetweenUnit * Math::Sin(angle))

								)
								-
								(counterLisClassHorizontalUnit * (getData().classGameStatus.DistanceBetweenUnitTate * Math::Sin(angle2)));

							//移動
							unit->orderPosiLeft = Vec2(xPos, yPos);
							unit->vecMove = Vec2(unit->orderPosiLeft - unit->nowPosiLeft).normalized();
							unit->FlagMove = false;
							unit->FlagMoving = true;

							counter++;
						}
					}
					else
					{
						int32 counter = 0;
						for (auto& unit : re)
						{
							//px+(b-切り捨て商)＊dcosθ+a＊d'cosθ’
							double xPos = end.x
								+ (
									(counter - (result))
									* (getData().classGameStatus.DistanceBetweenUnit * Math::Cos(angle))
									)
								-
								(counterLisClassHorizontalUnit * (getData().classGameStatus.DistanceBetweenUnitTate * Math::Cos(angle2)));
							//py+(b-切り捨て商)＊dsinθ-a＊d'sinθ’
							double yPos = end.y
								- (
								(counter - (result))
								* (getData().classGameStatus.DistanceBetweenUnit * Math::Sin(angle))

								)
								-
								(counterLisClassHorizontalUnit * (getData().classGameStatus.DistanceBetweenUnitTate * Math::Sin(angle2)));

							//移動
							unit->orderPosiLeft = Vec2(xPos, yPos);
							unit->vecMove = Vec2(unit->orderPosiLeft - unit->nowPosiLeft).normalized();
							unit->FlagMove = false;
							unit->FlagMoving = true;

							counter++;
						}
					}

					counterLisClassHorizontalUnit++;
				}
				getData().classGameStatus.IsBattleMove = false;
			}
			else
			{
				//範囲選択
				for (auto& target : *lisClassHorizontalUnit)
				{
					for (auto& unit : target.ListClassUnit)
					{
						Vec2 gnpc = unit.GetNowPosiCenter();
						if (start.x > end.x)
						{
							//左
							if (start.y > end.y)
							{
								//上
								if (gnpc.x >= end.x && gnpc.x <= start.x
									&& gnpc.y >= end.y && gnpc.y <= start.y)
								{
									unit.FlagMove = true;
									getData().classGameStatus.IsBattleMove = true;
								}
								else
								{
									unit.FlagMove = false;
								}
							}
							else
							{
								//下
								if (gnpc.x >= end.x && gnpc.x <= start.x
									&& gnpc.y >= start.y && gnpc.y <= end.y)
								{
									unit.FlagMove = true;
									getData().classGameStatus.IsBattleMove = true;
								}
								else
								{
									unit.FlagMove = false;
								}
							}
						}
						else
						{
							//右
							if (start.y > end.y)
							{
								//上
								if (gnpc.x >= start.x && gnpc.x <= end.x
									&& gnpc.y >= end.y && gnpc.y <= start.y)
								{
									unit.FlagMove = true;
									getData().classGameStatus.IsBattleMove = true;
								}
								else
								{
									unit.FlagMove = false;
								}
							}
							else
							{
								//下
								if (gnpc.x >= start.x && gnpc.x <= end.x
									&& gnpc.y >= start.y && gnpc.y <= end.y)
								{
									unit.FlagMove = true;
									getData().classGameStatus.IsBattleMove = true;
								}
								else
								{
									unit.FlagMove = false;
								}
							}
						}
					}
				}
			}
		}

		//移動処理
		for (auto& item : getData().classGameStatus.classBattle.sortieUnitGroup)
		{
			for (auto& itemUnit : item.ListClassUnit)
			{
				if (itemUnit.FlagMoving == false)
				{
					continue;
				}

				itemUnit.nowPosiLeft = itemUnit.nowPosiLeft + itemUnit.vecMove;
				if (itemUnit.nowPosiLeft.x <= itemUnit.orderPosiLeft.x + 10 && itemUnit.nowPosiLeft.x >= itemUnit.orderPosiLeft.x - 10
					&& itemUnit.nowPosiLeft.y <= itemUnit.orderPosiLeft.y + 10 && itemUnit.nowPosiLeft.y >= itemUnit.orderPosiLeft.y - 10)
				{
					itemUnit.FlagMoving = false;
				}
			}
		}
		for (auto& item : getData().classGameStatus.classBattle.defUnitGroup)
		{
			for (auto& itemUnit : item.ListClassUnit)
			{
				if (itemUnit.FlagMoving == false)
				{
					continue;
				}

				itemUnit.nowPosiLeft = itemUnit.nowPosiLeft + itemUnit.vecMove;
				if (itemUnit.nowPosiLeft.x <= itemUnit.orderPosiLeft.x + 10 && itemUnit.nowPosiLeft.x >= itemUnit.orderPosiLeft.x - 10
					&& itemUnit.nowPosiLeft.y <= itemUnit.orderPosiLeft.y + 10 && itemUnit.nowPosiLeft.y >= itemUnit.orderPosiLeft.y - 10)
				{
					itemUnit.FlagMoving = false;
				}
			}
		}

		//skill処理

	}
	// 描画関数（オプション）
	void draw() const override
	{
		const auto t = camera.createTransformer();

		//// マップを描く | Draw the map
		// 上から順にタイルを描く
		for (int32 i = 0; i < (mapCreator.N * 2 - 1); ++i)
		{
			// x の開始インデックス
			const int32 xi = (i < (mapCreator.N - 1)) ? 0 : (i - (mapCreator.N - 1));

			// y の開始インデックス
			const int32 yi = (i < (mapCreator.N - 1)) ? i : (mapCreator.N - 1);

			// 左から順にタイルを描く
			for (int32 k = 0; k < (mapCreator.N - Abs(mapCreator.N - i - 1)); ++k)
			{
				// タイルのインデックス
				const Point index{ (xi + k), (yi - k) };

				// そのタイルの底辺中央の座標
				const int32 i = index.manhattanLength();
				const int32 xi = (i < (mapCreator.N - 1)) ? 0 : (i - (mapCreator.N - 1));
				const int32 yi = (i < (mapCreator.N - 1)) ? i : (mapCreator.N - 1);
				const int32 k2 = (index.manhattanDistanceFrom(Point{ xi, yi }) / 2);
				const double posX = ((i < (mapCreator.N - 1)) ? (i * -mapCreator.TileOffset.x) : ((i - 2 * mapCreator.N + 2) * mapCreator.TileOffset.x));
				const double posY = (i * mapCreator.TileOffset.y);
				const Vec2 pos = { (posX + mapCreator.TileOffset.x * 2 * k2), posY };

				// 底辺中央を基準にタイルを描く
				String tip = getData().classGameStatus.classBattle.classMapBattle.value().mapData[index.x][index.y].tip;
				TextureAsset(tip + U".png").draw(Arg::bottomCenter = pos);
			}
		}


		// 底辺中央を基準にタイルを描く
		for (auto ttt : bui)
		{
			for (auto aaa : ttt.ListClassUnit)
			{
				// タイルのインデックス
				const Point index{ aaa.rowBuilding, aaa.colBuilding };

				// そのタイルの底辺中央の座標
				const int32 i = index.manhattanLength();
				const int32 xi = (i < (mapCreator.N - 1)) ? 0 : (i - (mapCreator.N - 1));
				const int32 yi = (i < (mapCreator.N - 1)) ? i : (mapCreator.N - 1);
				const int32 k2 = (index.manhattanDistanceFrom(Point{ xi, yi }) / 2);
				const double posX = ((i < (mapCreator.N - 1)) ? (i * -mapCreator.TileOffset.x) : ((i - 2 * mapCreator.N + 2) * mapCreator.TileOffset.x));
				const double yyy = (TextureAsset(aaa.Image + U".png").height()) - ((mapCreator.TileOffset.y * 2) - (mapCreator.TileThickness));
				const double posY = (i * mapCreator.TileOffset.y) - mapCreator.TileThickness;
				const Vec2 pos = { (posX + mapCreator.TileOffset.x * 2 * k2), posY };

				TextureAsset(aaa.Image + U".png").draw(Arg::bottomCenter = pos);
			}
		}

		//unit
		for (auto& item : getData().classGameStatus.classBattle.sortieUnitGroup)
		{
			if (!item.FlagBuilding &&
				!item.ListClassUnit.empty() &&
				item.ListClassUnit[0].Formation == BattleFormation::F)
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					if (itemUnit.FlagMove == true)
					{
						TextureAsset(itemUnit.Image).drawAt(itemUnit.GetNowPosiCenter()).drawFrame(3, 3, Palette::Orange);
					}
					else
					{
						TextureAsset(itemUnit.Image).drawAt(itemUnit.GetNowPosiCenter());
					}
				}
			}
		}
		for (auto& item : getData().classGameStatus.classBattle.defUnitGroup)
		{
			if (!item.FlagBuilding &&
				!item.ListClassUnit.empty() &&
				item.ListClassUnit[0].Formation == BattleFormation::F)
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					if (itemUnit.FlagMove == true)
					{
						TextureAsset(itemUnit.Image).drawAt(itemUnit.GetNowPosiCenter()).drawFrame(3, 3, Palette::Orange);
					}
					else
					{
						TextureAsset(itemUnit.Image).drawAt(itemUnit.GetNowPosiCenter());
					}
				}
			}
		}

		if (MouseR.pressed())
		{
			if (getData().classGameStatus.IsBattleMove == false)
			{
				const double thickness = 3.0;
				double offset = 0.0;

				offset += (Scene::DeltaTime() * 10);

				const Rect rect{ cursPos, Cursor::Pos() - cursPos };
				rect.top().draw(LineStyle::SquareDot(offset), thickness);
				rect.right().draw(LineStyle::SquareDot(offset), thickness);
				rect.bottom().draw(LineStyle::SquareDot(offset), thickness);
				rect.left().draw(LineStyle::SquareDot(offset), thickness);
			}
			else
			{
				Line{ cursPos, Cursor::Pos() }
				.drawArrow(10, Vec2{ 40, 80 }, Palette::Orange);
			}
		}
	}

	void drawFadeIn(double t) const override
	{
		draw();

		m_fadeInFunction->fade(1 - t);
	}

	void drawFadeOut(double t) const override
	{
		draw();

		m_fadeOutFunction->fade(t);
	}
private:
	Array<ClassHorizontalUnit> bui;
	Vec2 viewPos;
	Point cursPos;
	MapCreator mapCreator;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
	Camera2D camera;
	Array<Quad> columnQuads;
	Array<Quad> rowQuads;
	Array<std::unique_ptr<ISkill>> skills;

};
class Title : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	Title(const InitData& init)
		: IScene{ init }
	{
		// TOML ファイルからデータを読み込む
		const TOMLReader tomlConfig{ U"001_Warehouse/001_DefaultGame/config.toml" };

		if (not tomlConfig) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load `config.toml`" };
		}

		TitleMenuX = tomlConfig[U"config.TitleMenuX"].get<int32>();
		TitleMenuY = tomlConfig[U"config.TitleMenuY"].get<int32>();
		space = tomlConfig[U"config.TitleMenuSpace"].get<int32>();

		for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/001_SystemImage/015_TitleMenuImage/"))
		{
			String filename = FileSystem::FileName(filePath);
			TextureAsset::Register(filename, filePath);
		}
		easyRect = Rect(TitleMenuX, TitleMenuY, 160, 40);
		normalRect = Rect(TitleMenuX, (TitleMenuY + (40 * 1)) + space * 1, 160, 40);
		hardRect = Rect(TitleMenuX, (TitleMenuY + (40 * 2)) + space * 2, 160, 40);
		lunaRect = Rect(TitleMenuX, (TitleMenuY + (40 * 3)) + space * 3, 160, 40);

	}
	// 更新関数（オプション）
	void update() override
	{
		if (easyRect.leftClicked() == true)
		{
			changeScene(U"ScenarioMenu", 0.9s);
		}
		if (normalRect.leftClicked() == true)
		{
			changeScene(U"ScenarioMenu", 0.9s);
		}
		if (hardRect.leftClicked() == true)
		{
			changeScene(U"ScenarioMenu", 0.9s);
		}
		if (lunaRect.leftClicked() == true)
		{
			changeScene(U"ScenarioMenu", 0.9s);
		}
	}
	// 描画関数（オプション）
	void draw() const override
	{
		TextureAsset(U"0001_easy.png").draw(TitleMenuX, TitleMenuY);
		TextureAsset(U"0002_normal.png").draw(TitleMenuX, (TitleMenuY + (40 * 1)) + space * 1);
		TextureAsset(U"0003_hard.png").draw(TitleMenuX, (TitleMenuY + (40 * 2)) + space * 2);
		TextureAsset(U"0004_luna.png").draw(TitleMenuX, (TitleMenuY + (40 * 3)) + space * 3);
		easyRect.draw(ColorF{ 0, 0, 0, 0 });
		normalRect.draw(ColorF{ 0, 0, 0, 0 });
		hardRect.draw(ColorF{ 0, 0, 0, 0 });
		lunaRect.draw(ColorF{ 0, 0, 0, 0 });
	}

	void drawFadeIn(double t) const override
	{
		draw();

		m_fadeInFunction->fade(1 - t);
	}
	void drawFadeOut(double t) const override
	{
		draw();

		m_fadeOutFunction->fade(t);
	}
private:
	int32 TitleMenuX = 0;
	int32 TitleMenuY = 0;
	int32 space = 30;
	Rect easyRect;
	Rect normalRect;
	Rect hardRect;
	Rect lunaRect;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
};
class ScenarioMenu : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	ScenarioMenu(const InitData& init)
		: IScene{ init }
	{
		for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/070_Scenario/Info_Scenario/"))
		{
			String filename = FileSystem::FileName(filePath);
			const JSON jsonScenario = JSON::Load(filePath);
			if (not jsonScenario) // もし読み込みに失敗したら
			{
				continue;
			}

			for (const auto& [key, value] : jsonScenario[U"Scenario"]) {
				ClassScenario cs;
				cs.ButtonType = value[U"ButtonType"].getString();
				cs.ScenarioName = value[U"ScenarioName"].getString();
				cs.SortKey = Parse<int32>(value[U"Sortkey"].getString());
				if (value.hasElement(U"power") == true)
				{
					String sPower = value[U"power"].getString();
					if (sPower.contains(',') == true)
					{
						cs.ArrayPower = sPower.split(',');
					}
					else
					{
						cs.ArrayPower.push_back(sPower);
					}
				}
				if (value.hasElement(U"SelectCharaFrameImageLeft") == true)
				{
					cs.SelectCharaFrameImageLeft = value[U"SelectCharaFrameImageLeft"].getString();
				}
				if (value.hasElement(U"SelectCharaFrameImageRight") == true)
				{
					cs.SelectCharaFrameImageRight = value[U"SelectCharaFrameImageRight"].getString();
				}
				if (value.hasElement(U"HelpString") == true)
				{
					cs.HelpString = value[U"HelpString"].getString();
				}
				if (value.hasElement(U"Mail") == true)
				{
					cs.Mail = value[U"Mail"].getString();
				}
				if (value.hasElement(U"Internet") == true)
				{
					cs.Internet = value[U"Internet"].getString();
				}
				cs.Text = ClassStaticCommonMethod::MoldingScenarioText(value[U"Text"].getString());
				arrayClassScenario.push_back(std::move(cs));
			}
		}

		for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/001_SystemImage/020_ScenarioBackImage/"))
		{
			String filename = FileSystem::FileName(filePath);
			if (filename = U"pre0.jpg")
			{
				TextureAsset::Register(filename, filePath);
			}
		}

		vbar001.emplace(SasaGUI::Orientation::Vertical);;
		vbar002.emplace(SasaGUI::Orientation::Vertical);;
		Scenario1X = Scene::Size().x / 2;
		Scenario1Y = 200;
		Scenario2X = Scene::Size().x / 2;
		Scenario2Y = Scene::Size().y / 2;
		ScenarioTextX = Scene::Size().x / 2;
		ScenarioTextY = 200;
		auto max_it = std::max_element(arrayClassScenario.begin(), arrayClassScenario.end(),
									   [](const ClassScenario& a, const ClassScenario& b) {
										   return a.ScenarioName.size() < b.ScenarioName.size();
									   });
		if (max_it != arrayClassScenario.end())
		{
			ClassScenario& scenario_with_longest_name = *max_it;
			RectF re = getData().fontScenarioMenu(scenario_with_longest_name.ScenarioName).region();
			re.w = re.w + (50 * 2);
			re.h = Scenario1Y + 50 * 2;
			re.x = Scenario1X - (static_cast<int32>(re.w) + 150);
			re.y = Scenario1Y - 50;

			rectFScenario1X = re.asRect();
			rectFScenario2X = { (int32)re.x ,Scenario2Y ,(int32)re.w ,(int32)re.h };

			vbar001.value().updateLayout({
				(int32)(re.x + re.w + SasaGUI::ScrollBar::Thickness), (int32)re.y,
				SasaGUI::ScrollBar::Thickness,
				(int32)re.h
			});
			vbar001.value().updateConstraints(0.0, 2000.0, Scene::Height());

			vbar002.value().updateLayout({
				(int32)(re.x + re.w + SasaGUI::ScrollBar::Thickness), (int32)Scenario2Y,
				SasaGUI::ScrollBar::Thickness,
				(int32)re.h
			});
			vbar002.value().updateConstraints(0.0, 2000.0, Scene::Height());

			rectFScenarioTextX = Rect{ (int32)(re.x + re.w + SasaGUI::ScrollBar::Thickness) + 30,(int32)re.y ,ScenarioTextX - 100,650 };
		}

		int32 counterScenario1 = 0;
		for (auto&& e : arrayClassScenario | std::views::filter([](auto&& e) { return e.SortKey < 0; }))
		{
			RectF re = getData().fontScenarioMenu(e.ScenarioName).region();
			re.x = Scenario1X - (static_cast<int32>(re.w) + 200);

			int32 yyy = Scenario1Y + (counterScenario1 * 80) - vbar001.value().value();
			re.y = yyy;
			e.btnRectF = re;

			counterScenario1++;
		}
		int32 counterScenario2 = 0;
		for (auto&& e : arrayClassScenario | std::views::filter([](auto&& e) { return e.SortKey >= 0; }))
		{
			RectF re = getData().fontScenarioMenu(e.ScenarioName).region();
			re.x = Scenario2X - (static_cast<int32>(re.w) + 200);

			int32 yyy = Scenario2Y + (counterScenario2 * 80) - vbar002.value().value() + 50;
			re.y = yyy;
			e.btnRectF = re;

			counterScenario2++;
		}

	}
	// 更新関数（オプション）
	void update() override
	{
		int32 counterScenario1 = 0;
		for (auto&& e : arrayClassScenario | std::views::filter([](auto&& e) { return e.SortKey < 0; }))
		{
			int32 yyy = Scenario1Y + (counterScenario1 * 80) - vbar001.value().value();
			e.btnRectF.y = yyy;
			counterScenario1++;
		}
		int32 counterScenario2 = 0;
		for (auto&& e : arrayClassScenario | std::views::filter([](auto&& e) { return e.SortKey >= 0; }))
		{
			int32 yyy = Scenario2Y + (counterScenario2 * 80) - vbar002.value().value() + 50;
			e.btnRectF.y = yyy;
			counterScenario2++;
		}

		for (auto&& e : arrayClassScenario)
		{
			if (e.btnRectF.leftClicked() == true)
			{
				if (e.ButtonType == U"Scenario")
				{
					getData().selectClassScenario = e;
					changeScene(U"SelectChar", 0.9s);
				}
				else if (e.ButtonType == U"Mail")
				{
					String ttt = U"start \"aaa\" \"https://mail.google.com/mail/u/0/?tf=cm&fs=1&to";
					ttt += e.Mail;
					ttt += U"&su=game%E3%81%AE%E4%BB%B6&body=%E3%81%B5%E3%82%8F%E3%81%B5%E3%82%8F%EF%BD%9E%E3%80%82%E3%82%B2%E3%83%BC%E3%83%A0%E3%81%AE%E4%BB%B6%E3%81%A7%E8%81%9E%E3%81%8D%E3%81%9F%E3%81%84%E3%81%AE%E3%81%A7%E3%81%99%E3%81%8C%E4%BB%A5%E4%B8%8B%E8%A8%98%E8%BF%B0\"";
					std::system(ttt.narrow().c_str());
					//process = ChildProcess{ U"C:/Program Files (x86)/Google/Chrome/Application/chrome.exe", U"https://mail.google.com/mail/u/0/?tf=cm&fs=1&to" + e.Mail + U"&su=game%E3%81%AE%E4%BB%B6&body=%E3%81%B5%E3%82%8F%E3%81%B5%E3%82%8F%EF%BD%9E%E3%80%82%E3%82%B2%E3%83%BC%E3%83%A0%E3%81%AE%E4%BB%B6%E3%81%A7%E8%81%9E%E3%81%8D%E3%81%9F%E3%81%84%E3%81%AE%E3%81%A7%E3%81%99%E3%81%8C%E4%BB%A5%E4%B8%8B%E8%A8%98%E8%BF%B0" };
				}
				else if (e.ButtonType == U"Internet")
				{
					System::LaunchBrowser(e.Internet);
				}
			}
		}

		if (rectFScenario1X.mouseOver() == true)
		{
			vbar001.value().scroll(Mouse::Wheel() * 60);
		}
		if (rectFScenario2X.mouseOver() == true)
		{
			vbar002.value().scroll(Mouse::Wheel() * 60);
		}
		vbar001.value().update();
		vbar002.value().update();
	}
	// 描画関数（オプション）
	void draw() const override
	{
		TextureAsset(U"pre0.jpg").resized(WindowSizeWidth, WindowSizeHeight).draw(Arg::center = Scene::Center());

		getData().slice9.draw(rectFScenario1X);
		getData().slice9.draw(rectFScenario2X);
		getData().slice9.draw(rectFScenarioTextX);

		int32 counterScenario1 = 0;
		for (auto&& e : arrayClassScenario | std::views::filter([](auto&& e) { return e.SortKey < 0; }))
		{
			int32 yyy = Scenario1Y + (counterScenario1 * 80) - vbar001.value().value();
			if (yyy > rectFScenario1X.y)
			{
				getData().slice9.draw(e.btnRectF.asRect());
				getData().fontScenarioMenu(e.ScenarioName).draw(e.btnRectF.stretched(1), ColorF{ 0.85 });
			}

			counterScenario1++;
		}
		int32 counterScenario2 = 0;
		for (auto&& e : arrayClassScenario | std::views::filter([](auto&& e) { return e.SortKey >= 0; }))
		{
			int32 yyy = Scenario2Y + (counterScenario2 * 80) - vbar002.value().value() + 50;
			if (yyy > rectFScenario2X.y)
			{
				getData().slice9.draw(e.btnRectF.asRect());
				getData().fontScenarioMenu(e.ScenarioName).draw(e.btnRectF.stretched(1), ColorF{ 0.85 });
			}

			counterScenario2++;
		}

		for (auto&& e : arrayClassScenario)
		{
			if (e.btnRectF.mouseOver() == true)
			{
				getData().fontScenarioMenu(e.Text).draw(rectFScenarioTextX.stretched(1), ColorF{ 0.85 });
			}
		}

		vbar001.value().draw();
		vbar002.value().draw();
	}

	void drawFadeIn(double t) const override
	{
		draw();

		m_fadeInFunction->fade(1 - t);
	}
	void drawFadeOut(double t) const override
	{
		draw();

		m_fadeOutFunction->fade(t);
	}
private:
	Optional<SasaGUI::ScrollBar> vbar001;
	Optional<SasaGUI::ScrollBar> vbar002;
	//SasaGUI::ScrollBar vbar002;
	//SasaGUI::ScrollBar vbar003;
	int32 Scenario1X;
	int32 Scenario1Y;
	int32 Scenario2X;
	int32 Scenario2Y;
	int32 ScenarioTextX;
	int32 ScenarioTextY;
	Rect rectFScenario1X;
	Rect rectFScenario2X;
	Rect rectFScenarioTextX;
	Array <ClassScenario> arrayClassScenario;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
	Optional<ChildProcess> process;
};
class SelectChar : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	SelectChar(const InitData& init)
		: IScene{ init }
	{
		for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/070_Scenario/Info_Power/"))
		{
			String filename = FileSystem::FileName(filePath);
			const JSON jsonPower = JSON::Load(filePath);
			if (not jsonPower) // もし読み込みに失敗したら
			{
				continue;
			}

			for (const auto& [key, value] : jsonPower[U"Power"])
			{
				ClassPower cp;
				cp.PowerTag = value[U"PowerTag"].getString();
				cp.PowerName = value[U"PowerName"].getString();
				cp.HelpString = value[U"Help"].getString();
				cp.SortKey = Parse<int32>(value[U"SortKey"].getString());
				cp.Image = value[U"Image"].getString();
				cp.Text = value[U"Text"].getString();
				cp.Diff = value[U"Diff"].getString();
				getData().classGameStatus.arrayClassPower.push_back(std::move(cp));
			}
		}
		for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/030_SelectCharaImage/"))
		{
			String filename = FileSystem::FileName(filePath);
			TextureAsset::Register(filename, filePath);
		}

		String sc = getData().classConfigString.selectChara1;
		RectF re1 = getData().fontSelectChar1(sc).region();
		re1.x = Scene::Center().x - (re1.w / 2);
		re1.y = basePointY;
		arrayRectFSystem.push_back(re1);
		Rect re2 = { 0,0,1600,300 };
		re2.x = Scene::Center().x - (800);
		re2.y = (re1.h + basePointY + 20) + 550 + 20;
		arrayRectSystem.push_back(re2);

		int32 arrayPowerSize = getData().selectClassScenario.ArrayPower.size();
		int32 xxx = 0;
		xxx = ((arrayPowerSize * 206) / 2);
		int32 counter = 0;
		for (auto ttt : getData().selectClassScenario.ArrayPower)
		{
			for (auto&& e : getData().classGameStatus.arrayClassPower | std::views::filter([&](auto&& e) { return e.PowerTag == ttt; }))
			{
				RectF rrr = {};
				rrr = { (Scene::Center().x - xxx) + counter * 206,re1.h + basePointY + 20,206,550 };
				e.RectF = rrr;
			}
			counter++;
		}

		//Scene::SetBackground(Color{ 126,87,194,255 });
	}
	// 更新関数（オプション）
	void update() override
	{
		getData().slice9.draw(arrayRectSystem[0]);

		for (const auto ttt : getData().classGameStatus.arrayClassPower)
		{
			if (ttt.RectF.mouseOver() == true)
			{
				getData().fontSelectChar2(ttt.Text).draw(arrayRectSystem[0].stretched(-10), ColorF{ 0.85 });
			}
			if (ttt.RectF.leftClicked() == true)
			{

				changeScene(U"Novel", 0.9s);
			}
		}
	}
	// 描画関数（オプション）
	void draw() const override
	{
		TextureAsset(getData().selectClassScenario.SelectCharaFrameImageLeft).draw();
		TextureAsset(getData().selectClassScenario.SelectCharaFrameImageRight)
			.draw(Scene::Size().x - TextureAsset(getData().selectClassScenario.SelectCharaFrameImageRight).width(), 0);

		arrayRectFSystem[0].draw();
		getData().fontSelectChar1(getData().classConfigString.selectChara1).draw(arrayRectFSystem[0], ColorF{ 0.25 });
		arrayRectFSystem[0].drawFrame(3, 0, Palette::Orange);

		for (const auto ttt : getData().classGameStatus.arrayClassPower)
		{
			ttt.RectF(TextureAsset(ttt.Image)).draw();
		}
	}

	void drawFadeIn(double t) const override
	{
		draw();

		m_fadeInFunction->fade(1 - t);
	}
	void drawFadeOut(double t) const override
	{
		draw();

		m_fadeOutFunction->fade(t);
	}
private:
	int32 basePointY = 50;
	Array<Rect> arrayRectSystem;
	Array<RectF> arrayRectFSystem;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
};
class Novel : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	Novel(const InitData& init)
		: IScene{ init }
	{
		String np = getData().NovelPower;
		String nn = getData().NovelNumber;
		getData().NovelPower = U"";
		getData().NovelNumber = U"";

		csv = CSV{ U"001_Warehouse/001_DefaultGame/070_Scenario/InfoStory/" + np + U"+" + nn + U".csv" };
		if (not csv) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load " + np + U"+" + nn + U".csv" };
		}

		nowRow = 0;
		if (csv[nowRow][0].c_str()[0] == '#')
		{
			nowRow++;
		}

		rectText = { 50,Scene::Size().y - 325,Scene::Size().x - 100,300 };
		rectHelp = { 70,Scene::Size().y - 325 - 70,400,70 };
		rectFace = { Scene::Size().x - 100 - 206 - 50,Scene::Size().y - 325 + 50,206,206 };
	}
	// 更新関数（オプション）
	void update() override
	{
		// 文字カウントを 0.1 秒ごとに増やす
		length = static_cast<size_t>(Scene::Time() / 0.1);

		if (csv[nowRow][0].substr(0, 3) == U"end")
		{
			changeScene(U"Buy", 0.9s);
		}

		if (csv[nowRow][0].c_str()[0] == '#')
		{
			nowRow++;
		}

		if (MouseL.down() == true)
		{
			nowRow++;
			length = 0;
		}
	}
	// 描画関数（オプション）
	void draw() const override
	{
		if (csv[nowRow][4] == U"0")
		{
			Scene::SetBackground(ColorF{ U"#000000" });
		}
		if (csv[nowRow][4] != U"-1" && csv[nowRow][4] != U"0")
		{
			TextureAsset(csv[nowRow][4]).resized(WindowSizeWidth, WindowSizeHeight).drawAt(Scene::Center());
		}

		getData().slice9.draw(rectText);

		if (csv[nowRow][3] != U"-1")
		{
			rectFace(TextureAsset(csv[nowRow][3])).draw();
		}
		if (csv[nowRow][0].c_str()[0] != '#')
		{
			getData().fontScenarioMenu(csv[nowRow][7].substr(0, length)).draw(rectText.stretched(-10), ColorF{ 0.85 });
		}
		if (csv[nowRow][1] != U"-1")
		{
			getData().slice9.draw(rectHelp);
			String he = U"";
			if (csv[nowRow][2] != U"-1")
			{
				he = csv[nowRow][1] + U" " + csv[nowRow][2];
			}
			else
			{
				he = csv[nowRow][1];
			}
			getData().fontNovelHelp(he).draw(rectHelp.stretched(-10), ColorF{ 0.85 });
		}
	}

	void drawFadeIn(double t) const override
	{
		draw();

		m_fadeInFunction->fade(1 - t);
	}

	void drawFadeOut(double t) const override
	{
		draw();

		m_fadeOutFunction->fade(t);
	}
private:
	Rect rectText;
	Rect rectFace;
	Rect rectHelp;
	int32 nowRow;
	int32 length;
	CSV csv;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
};
class Buy : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	Buy(const InitData& init)
		: IScene{ init }
	{
	}
	// 更新関数（オプション）
	void update() override
	{
	}
	// 描画関数（オプション）
	void draw() const override
	{

	}

	void drawFadeIn(double t) const override
	{
		draw();

		m_fadeInFunction->fade(1 - t);
	}

	void drawFadeOut(double t) const override
	{
		draw();

		m_fadeOutFunction->fade(t);
	}
private:
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
};
class common001 : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	common001(const InitData& init)
		: IScene{ init }
	{
	}
	// 更新関数（オプション）
	void update() override
	{
	}
	// 描画関数（オプション）
	void draw() const override
	{

	}

	void drawFadeIn(double t) const override
	{
		draw();

		m_fadeInFunction->fade(1 - t);
	}

	void drawFadeOut(double t) const override
	{
		draw();

		m_fadeOutFunction->fade(t);
	}
private:
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
};

void Main()
{
	// ウィンドウのタイトル | Window title
	Window::SetTitle(U"MagicArc-Ver0.1");

	// ウィンドウのサイズ | Window size
	Window::Resize(WindowSizeWidth, WindowSizeHeight);
	//// フルスクリーンモードのデフォルトを有効にするか | Whether to enable fullscreen mode by default
	//Window::SetFullscreen(true);
	// 背景の色を設定する | Set the background color
	//Scene::SetBackground(ColorF{ 0.6, 0.8, 0.7 });
	// ウィンドウを中心に移動
	Window::Centering();
	//ウィンドウを手動でリサイズできるようにする
	//ウィンドウの最大化を可能にする
	Window::SetStyle(WindowStyle::Sizable);

	Scene::SetResizeMode(ResizeMode::Virtual);
	// ウィンドウを最大化
	Window::Maximize();

	// シーンマネージャーを作成
	// ここで GameData が初期化される
	App manager;
	// シーンを登録
	manager.add<SelectLang>(U"SelectLang");
	manager.add<TestBattle>(U"TestBattle");
	manager.add<Battle>(U"Battle");
	manager.add<Title>(U"Title");
	manager.add<ScenarioMenu>(U"ScenarioMenu");
	manager.add<SelectChar>(U"SelectChar");
	manager.add<Novel>(U"Novel");
	manager.add<Buy>(U"Buy");

	for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/010_FaceImage/"))
	{
		String filename = FileSystem::FileName(filePath);
		TextureAsset::Register(filename, filePath);
	}
	for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/005_BackgroundImage/"))
	{
		String filename = FileSystem::FileName(filePath);
		TextureAsset::Register(filename, filePath);
	}

	if (System::GetCommandLineArgs().size() == 0)
	{

	}
	else
	{
		//manager.init(U"SelectLang");

		//manager.init(U"TestBattle");
		//manager.init(U"Title");

		manager.get().get()->NovelPower = U"sc_a_p_b";
		manager.get().get()->NovelNumber = U"0";
		manager.init(U"Novel");
	}

	while (System::Update())
	{
		if (not manager.update())
		{
			break;
		}
	}
}
