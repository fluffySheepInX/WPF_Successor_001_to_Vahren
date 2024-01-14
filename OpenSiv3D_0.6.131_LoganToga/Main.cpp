# include <Siv3D.hpp> // OpenSiv3D v0.6.9
# include "ClassAStar.h" 
# include "ClassConfigString.h" 
# include "ClassGameStatus.h" 
# include "ClassMap.h" 
# include "ClassTestBattle.h" 
# include "ClassUnit.h" 
# include "ClassExecuteSkills.h" 
# include "ClassObjectMapTip.h"
# include "ClassBattle.h" 
# include "ClassScenario.h" 
# include "ClassStaticCommonMethod.h" 
# include "MapCreator.h" 
# include "DoubleClick.h" 
# include "SasaGUI.h" 
# include "ClassPower.h" 
# include "GameUIToolkit.h" 
# include <ranges>
# include "Enum.h"
#include <future>
#include <functional> // std::ref を使用

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

namespace s3dx
{
	class SceneMessageBoxImpl
	{
	public:
		RectF m_buttonC = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(0, -20)), MessageBoxButtonSize };

		RectF m_buttonL = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(-80, -20)), MessageBoxButtonSize };

		RectF m_buttonR = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(80, -20)), MessageBoxButtonSize };

		static constexpr Size MessageBoxSize{ 360, 240 };

		static constexpr Size MessageBoxButtonSize{ 120, 40 };

		static constexpr ColorF MessageBoxBackgroundColor{ 0.96 };

		static constexpr ColorF MessageBoxActiveButtonColor{ 1.0 };

		static constexpr ColorF MessageBoxTextColor{ 0.11 };

		SceneMessageBoxImpl()
		{
			//System::SetTerminationTriggers(UserAction::NoAction);
			//Scene::SetBackground(ColorF{ 0.11 });
		}

		~SceneMessageBoxImpl()
		{
			//System::SetTerminationTriggers(m_triggers);
			//Scene::SetBackground(m_bgColor);
		}

		void set(Camera2D& camera2D)
		{
			m_pos = ((camera2D.getCenter() - MessageBoxSize) * 0.5);
			m_messageBoxRect = { m_pos, MessageBoxSize };
			m_buttonC = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(0, -20)), MessageBoxButtonSize };
			m_buttonL = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(-80, -20)), MessageBoxButtonSize };
			m_buttonR = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(80, -20)), MessageBoxButtonSize };
			m_display = true;
		}
		void set()
		{
			m_pos = ((Scene::Center() - (MessageBoxSize * 0.5)));
			m_messageBoxRect = { m_pos, MessageBoxSize };
			m_buttonC = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(0, -20)), MessageBoxButtonSize };
			m_buttonL = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(-80, -20)), MessageBoxButtonSize };
			m_buttonR = RectF{ Arg::bottomCenter(m_messageBoxRect.bottomCenter().movedBy(80, -20)), MessageBoxButtonSize };
			m_display = true;
		}

		void show(StringView text) const
		{
			if (m_display == false)
			{
				return;
			}
			drawMessageBox(text);
			m_buttonC.draw(m_buttonC.mouseOver() ? MessageBoxActiveButtonColor : MessageBoxBackgroundColor).drawFrame(0, 1, MessageBoxTextColor);
			m_font(U"OK").drawAt(m_buttonC.center().moveBy(0, -1), MessageBoxTextColor);
		}

		MessageBoxResult show(const StringView text, const std::pair<String, MessageBoxResult>& button0, const std::pair<String, MessageBoxResult>& button1) const
		{
			MessageBoxResult result = MessageBoxResult::Cancel;

			drawMessageBox(text);
			m_buttonL.draw(m_buttonL.mouseOver() ? MessageBoxActiveButtonColor : MessageBoxBackgroundColor).drawFrame(0, 1, MessageBoxTextColor);
			m_buttonR.draw(m_buttonR.mouseOver() ? MessageBoxActiveButtonColor : MessageBoxBackgroundColor).drawFrame(0, 1, MessageBoxTextColor);
			m_font(button0.first).drawAt(m_buttonL.center().moveBy(0, -1), MessageBoxTextColor);
			m_font(button1.first).drawAt(m_buttonR.center().moveBy(0, -1), MessageBoxTextColor);

			if (m_buttonL.mouseOver())
			{
				Cursor::RequestStyle(CursorStyle::Hand);

				if (MouseL.down())
				{
					result = button0.second;
					return result;
				}
			}
			else if (m_buttonR.mouseOver())
			{
				Cursor::RequestStyle(CursorStyle::Hand);

				if (MouseL.down())
				{
					result = button1.second;
					return result;
				}
			}
			return result;
		}

	private:
		uint32 m_triggers = System::GetTerminationTriggers();

		ColorF m_bgColor = Scene::GetBackground();

		Vec2 m_pos = ((Scene::Size() - MessageBoxSize) * 0.5);;

		RectF m_messageBoxRect{ m_pos, MessageBoxSize };

		Font m_font = SimpleGUI::GetFont();

		bool m_display = false;

		void drawMessageBox(StringView text) const
		{
			m_messageBoxRect.draw(MessageBoxBackgroundColor).stretched(-5).drawFrame(1, 0, MessageBoxTextColor);
			m_font(text).draw(14, m_messageBoxRect.stretched(-20, -20, -80, -20), MessageBoxTextColor);
		}
	};

	//inline MessageBoxResult SceneMessageBoxOK(StringView text)
	//{
	//	return SceneMessageBoxImpl{}.show(text, { U"OK", MessageBoxResult::OK });
	//}

	//[[nodiscard]]
	//inline MessageBoxResult SceneMessageBoxOKCancel(StringView text)
	//{
	//	return SceneMessageBoxImpl{}.show(text, { U"OK", MessageBoxResult::OK }, { U"キャンセル", MessageBoxResult::Cancel });
	//}

	//[[nodiscard]]
	//inline MessageBoxResult SceneMessageBoxYesNo(StringView text)
	//{
	//	return SceneMessageBoxImpl{}.show(text, { U"はい", MessageBoxResult::Yes }, { U"いいえ", MessageBoxResult::No });
	//}
}

// フェード描画クラスのインスタンスをランダムに返す
auto randomFade()
{
	Array<std::function<std::unique_ptr<IFade>()>> makeFadeFuncs = {
		[]() -> std::unique_ptr<IFade> { return std::make_unique<Fade4>(); },
	};

	return Sample(makeFadeFuncs)();
}
MapCreator mapCreator;
Array<Quad> columnQuads;
Array<Quad> rowQuads;
Optional<ClassAStar*> SearchMinScore(const Array<ClassAStar*>& ls) {
	int minScore = std::numeric_limits<int>::max();
	int minCost = std::numeric_limits<int>::max();
	Optional<ClassAStar*> targetClassAStar;

	for (const auto& itemClassAStar : ls) {
		if (itemClassAStar->GetAStarStatus() != AStarStatus::Open)
		{
			continue;
		}
		int score = itemClassAStar->GetCost() + itemClassAStar->GetHCost();

		if (score < minScore || (score == minScore && itemClassAStar->GetCost() < minCost)) {
			minScore = score;
			minCost = itemClassAStar->GetCost();
			targetClassAStar = itemClassAStar;
		}
	}

	return targetClassAStar;
}

int32 BattleMoveAStar(Array<ClassHorizontalUnit>& target,
						Array<ClassHorizontalUnit>& enemy,
						MapCreator mapCreator,
						Array<Array<MapDetail>> mapData,
						ClassGameStatus& classGameStatus,
						Array<Array<Point>>& debugRoot,
						Array<ClassAStar*>& list)
{
	Array<ClassObjectMapTip> arrayClassObjectMapTip = classGameStatus.arrayClassObjectMapTip;
	Array<ClassObjectMapTip> arrayClassObjectMapTip2;
	while (true)
	{
		////アスターアルゴリズムで移動経路取得
		for (auto& aaa : target)
		{
			if (aaa.FlagBuilding == true)
			{
				continue;
			}
			for (auto& bbb : aaa.ListClassUnit)
			{
				if (bbb.IsBuilding == true && bbb.mapTipObjectType == MapTipObjectType::WALL2)
				{
					continue;
				}
				if (bbb.IsBattleEnable == false)
				{
					continue;
				}
				if (bbb.FlagMoving == true)
				{
					continue;
				}
				if (bbb.FlagMovingEnd == false)
				{
					continue;
				}
				Array<Point> listRoot;

				//まず現在のマップチップを取得
				s3d::Optional<Size> nowIndex = mapCreator.ToIndex(bbb.GetNowPosiCenter(), columnQuads, rowQuads);
				if (nowIndex.has_value() == false)
				{
					continue;
				}

				//最寄りの敵の座標を取得
				Vec2 xy1 = bbb.GetNowPosiCenter();
				xy1.x = xy1.x * xy1.x;
				xy1.y = xy1.y * xy1.y;
				double disA = xy1.x + xy1.y;
				HashTable<double, ClassUnit> dicDis;
				try
				{
					for (auto& ccc : enemy)
					{
						for (auto& ddd : ccc.ListClassUnit)
						{
							if (ddd.IsBattleEnable == false)
							{
								continue;
							}
							Vec2 xy2 = ddd.GetNowPosiCenter();
							xy2.x = xy2.x * xy2.x;
							xy2.y = xy2.y * xy2.y;
							double disB = xy2.x + xy2.y;
							dicDis.emplace(disA - disB, ddd);
						}
					}
				}
				catch (const std::exception&)
				{
					throw;
				}

				if (dicDis.size() == 0)
				{
					continue;
				}

				auto minElement = dicDis.begin();
				for (auto it = dicDis.begin(); it != dicDis.end(); ++it) {
					if (it->first < minElement->first) {
						minElement = it;
					}
				}

				//最寄りの敵のマップチップを取得
				s3d::Optional<Size> nowIndexEnemy = mapCreator.ToIndex(minElement->second.GetNowPosiCenter(), columnQuads, rowQuads);
				if (nowIndexEnemy.has_value() == false)
				{
					continue;
				}

				////現在地を開く
				ClassAStarManager classAStarManager(nowIndexEnemy.value().x, nowIndexEnemy.value().y);
				Optional<ClassAStar*> startAstar = classAStarManager.OpenOne(nowIndex.value().x, nowIndex.value().y, 0, nullptr, mapCreator.N);
				MicrosecClock mc;
				////移動経路取得
				while (true)
				{
					try
					{
						if (startAstar.has_value() == false)
						{
							listRoot.clear();
							break;
						}

						Print << U"AAAAAAAAAAAAAAAAA:" + Format(mc.us());
						classAStarManager.OpenAround(startAstar.value(),
														mapData,
														enemy,
														target,
														arrayClassObjectMapTip2,
														mapCreator.N
						);
						Print << U"BBBBBBBBBBBBBBBB:" + Format(mc.us());
						startAstar.value()->SetAStarStatus(AStarStatus::Closed);

						classAStarManager.RemoveClassAStar(startAstar.value());

						if (classAStarManager.GetListClassAStar().size() != 0)
						{
							startAstar = SearchMinScore(classAStarManager.GetListClassAStar());
						}

						if (startAstar.has_value() == false)
						{
							continue;
						}

						//敵まで到達したか
						if (startAstar.value()->GetRow() == classAStarManager.GetEndX() && startAstar.value()->GetCol() == classAStarManager.GetEndY())
						{
							startAstar.value()->GetRoot(listRoot);
							listRoot.reverse();
							break;
						}
					}
					catch (const std::exception&)
					{
						throw;
					}
				}

				if (listRoot.size() != 0)
				{
					classGameStatus.aiRoot[bbb.ID] = listRoot;
					debugRoot.push_back(listRoot);
				}
			}
		}
	}

	return -1;
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
	Font fontBuyMenu = Font{ 20, U"font/DotGothic16-Regular.ttf" ,FontStyle::Bitmap };
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
	int32 NovelNumber = 0;
	int32 Money = 0;
	int32 Wave = 0;
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
		bool ran = false;
		for (auto& item : getData().classGameStatus.classBattle.sortieUnitGroup)
		{
			if (!item.FlagBuilding &&
				!item.ListClassUnit.empty())
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					Point pt = Point(counterXSor, counterYSor);
					Vec2 reV = mapCreator.ToTileBottomCenter(pt, mapCreator.N);
					if (ran == true)
					{
						itemUnit.nowPosiLeft = Vec2(reV.x + Random(-50, 50), reV.y + Random(-50, 50));
					}
					else
					{
						itemUnit.nowPosiLeft = Vec2(reV.x, reV.y - itemUnit.TakasaUnit - 15);
					}
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
					if (ran == true)
					{
						itemUnit.nowPosiLeft = Vec2(reV.x + Random(-50, 50), reV.y + Random(-50, 50));
					}
					else
					{
						itemUnit.nowPosiLeft = Vec2(reV.x, reV.y - itemUnit.TakasaUnit - 15);
					}
				}
			}
		}

		// buiの初期位置
		for (auto& item : getData().classGameStatus.classBattle.sortieUnitGroup)
		{
			if (item.FlagBuilding == true &&
				!item.ListClassUnit.empty())
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					Point pt = Point(itemUnit.rowBuilding, itemUnit.colBuilding);
					Vec2 vv = mapCreator.ToTileBottomCenter(pt, mapCreator.N);
					vv = { vv.x,vv.y - (25 + 15) };
					itemUnit.nowPosiLeft = vv;
				}
			}
		}
		for (auto& item : getData().classGameStatus.classBattle.defUnitGroup)
		{
			if (item.FlagBuilding == true &&
				!item.ListClassUnit.empty())
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					Point pt = Point(itemUnit.rowBuilding, itemUnit.colBuilding);
					Vec2 vv = mapCreator.ToTileBottomCenter(pt, mapCreator.N);
					vv = { vv.x,vv.y - (25 + 15) };
					itemUnit.nowPosiLeft = vv;
				}
			}
		}

		task = Async(BattleMoveAStar,
						std::ref(getData().classGameStatus.classBattle.defUnitGroup),
						std::ref(getData().classGameStatus.classBattle.sortieUnitGroup),
						std::ref(mapCreator),
						std::ref(getData().classGameStatus.classBattle.classMapBattle.value().mapData),
						std::ref(getData().classGameStatus),
						std::ref(debugRoot), std::ref(debugAstar));

	}
	// 更新関数（オプション）
	void update() override
	{
		Cursor::RequestStyle(U"MyCursor");

		const auto t = camera.createTransformer();

		switch (battleStatus)
		{
		case BattleStatus::Battle:
		{
			if (SimpleGUI::Button(U"Call", Vec2{ 600, 20 }, unspecified, (not task.isValid())))
			{
			}

			//カメラ移動
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

			//部隊を選択状態にする。もしくは既に選択状態なら移動させる
			if (MouseR.up() == true)
			{
				Point start = cursPos;
				Point end = Cursor::Pos();

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
			for (auto& item : getData().classGameStatus.classBattle.defUnitGroup)
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					if (itemUnit.IsBuilding == true && itemUnit.mapTipObjectType == MapTipObjectType::WALL2)
					{
						continue;
					}
					if (itemUnit.IsBattleEnable == false)
					{
						continue;
					}
					if (itemUnit.FlagMoving == true)
					{
						itemUnit.nowPosiLeft = itemUnit.nowPosiLeft + (itemUnit.vecMove * (itemUnit.Move / 100));

						Circle c = { itemUnit.nowPosiLeft ,1 };
						Circle cc = { itemUnit.orderPosiLeft ,1 };

						if (c.intersects(cc))
						{
							itemUnit.FlagMoving = false;
						}
						//if (itemUnit.nowPosiLeft.x <= itemUnit.orderPosiLeft.x + 10 && itemUnit.nowPosiLeft.x >= itemUnit.orderPosiLeft.x - 10
						//	&& itemUnit.nowPosiLeft.y <= itemUnit.orderPosiLeft.y + 10 && itemUnit.nowPosiLeft.y >= itemUnit.orderPosiLeft.y - 10)
						//{
						//	itemUnit.FlagMoving = false;
						//}
						continue;
					}
					//auto rootPo = getData().classGameStatus.aiRoot;
					//for (auto [key, value] : rootPo)
					//{
					//	Print << key << U": " << value;
					//}
					if (getData().classGameStatus.aiRoot[itemUnit.ID].isEmpty() == true)
					{
						continue;
					}
					if (getData().classGameStatus.aiRoot[itemUnit.ID].size() == 1)
					{
						itemUnit.FlagMovingEnd = true;
						itemUnit.FlagMoving = false;
						continue;
					}

					// タイルのインデックス
					Point index;
					try
					{
						getData().classGameStatus.aiRoot[itemUnit.ID].pop_front();
						auto rthrthrt = getData().classGameStatus.aiRoot[itemUnit.ID];
						index = getData().classGameStatus.aiRoot[itemUnit.ID][0];
					}
					catch (const std::exception&)
					{
						throw;
						continue;
					}
					//Print << index;
					//Print << getData().classGameStatus.aiRoot[itemUnit.ID];

					// そのタイルの底辺中央の座標
					const int32 i = index.manhattanLength();
					const int32 xi = (i < (mapCreator.N - 1)) ? 0 : (i - (mapCreator.N - 1));
					const int32 yi = (i < (mapCreator.N - 1)) ? i : (mapCreator.N - 1);
					const int32 k2 = (index.manhattanDistanceFrom(Point{ xi, yi }) / 2);
					const double posX = ((i < (mapCreator.N - 1)) ? (i * -mapCreator.TileOffset.x) : ((i - 2 * mapCreator.N + 2) * mapCreator.TileOffset.x));
					const double posY = (i * mapCreator.TileOffset.y) - mapCreator.TileThickness;
					const Vec2 pos = { (posX + mapCreator.TileOffset.x * 2 * k2) - (itemUnit.yokoUnit / 2), posY - itemUnit.TakasaUnit - 15 };

					itemUnit.orderPosiLeft = pos;
					Vec2 hhh = itemUnit.GetOrderPosiCenter() - itemUnit.GetNowPosiCenter();
					if (hhh.x == 0 && hhh.y == 0)
					{
						itemUnit.vecMove = { 0,0 };
					}
					else
					{
						itemUnit.vecMove = hhh.normalized();
					}
					itemUnit.FlagMoving = true;
					itemUnit.FlagMovingEnd = false;
				}
			}

			for (auto& item : getData().classGameStatus.classBattle.sortieUnitGroup)
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					if (itemUnit.IsBattleEnable == false)
					{
						continue;
					}
					if (itemUnit.FlagMoving == false)
					{
						continue;
					}

					itemUnit.nowPosiLeft = itemUnit.nowPosiLeft + (itemUnit.vecMove * (itemUnit.Move / 100));
					if (itemUnit.nowPosiLeft.x <= itemUnit.orderPosiLeft.x + 10 && itemUnit.nowPosiLeft.x >= itemUnit.orderPosiLeft.x - 10
						&& itemUnit.nowPosiLeft.y <= itemUnit.orderPosiLeft.y + 10 && itemUnit.nowPosiLeft.y >= itemUnit.orderPosiLeft.y - 10)
					{
						itemUnit.FlagMoving = false;
					}
				}
			}
			for (auto& item : getData().classGameStatus.classBattle.neutralUnitGroup)
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					if (itemUnit.IsBattleEnable == false)
					{
						continue;
					}
					if (itemUnit.FlagMoving == false)
					{
						continue;
					}

					itemUnit.nowPosiLeft = itemUnit.nowPosiLeft + (itemUnit.vecMove * (itemUnit.Move / 100));

					if (itemUnit.nowPosiLeft.x <= itemUnit.orderPosiLeft.x + mapCreator.TileOffset.x && itemUnit.nowPosiLeft.x >= itemUnit.orderPosiLeft.x - mapCreator.TileOffset.x
						&& itemUnit.nowPosiLeft.y <= itemUnit.orderPosiLeft.y + mapCreator.TileOffset.y && itemUnit.nowPosiLeft.y >= itemUnit.orderPosiLeft.y - mapCreator.TileOffset.y)
					{
						itemUnit.FlagMoving = false;
						getData().classGameStatus.aiRoot[itemUnit.ID].pop_front();
					}
				}
			}

			//skill処理
			SkillProcess(getData().classGameStatus.classBattle.sortieUnitGroup, getData().classGameStatus.classBattle.defUnitGroup, m_Battle_player_skills);
			SkillProcess(getData().classGameStatus.classBattle.defUnitGroup, getData().classGameStatus.classBattle.sortieUnitGroup, m_Battle_enemy_skills);

			//skill実行処理
			{
				Array<ClassExecuteSkills> deleteCES;
				for (ClassExecuteSkills& loop_Battle_player_skills : m_Battle_player_skills)
				{
					Array<int32> arrayNo;
					for (auto& target : loop_Battle_player_skills.ArrayClassBullet)
					{
						target.lifeTime += Scene::DeltaTime();

						if (target.lifeTime > target.duration)
						{
							loop_Battle_player_skills.classUnit->FlagMovingSkill = false;

							//消滅
							arrayNo.push_back(target.No);

							break;
						}
						else
						{
							target.NowPosition = Vec2((target.NowPosition.x + (target.MoveVec.x * (loop_Battle_player_skills.classSkill.speed / 100))),
														(target.NowPosition.y + (target.MoveVec.y * (loop_Battle_player_skills.classSkill.speed / 100))));
						}

						//衝突したらunitのHPを減らし、消滅
						Circle c = Circle{ target.NowPosition.x,target.NowPosition.y,30 };

						for (auto& itemTargetHo : getData().classGameStatus.classBattle.defUnitGroup)
						{
							for (auto& itemTarget : itemTargetHo.ListClassUnit)
							{
								if (itemTarget.IsBattleEnable == false)
								{
									continue;
								}

								Point pt = Point(itemTarget.rowBuilding, itemTarget.colBuilding);
								Vec2 vv = mapCreator.ToTileBottomCenter(pt, mapCreator.N);
								vv = { vv.x,vv.y - (25 + 15) };
								Circle cTar = Circle{ vv,30 };
								if (c.intersects(cTar) == true)
								{
									loop_Battle_player_skills.classUnit->FlagMovingSkill = false;

									if (itemTarget.IsBuilding == true)
									{
										itemTarget.HPCastle = itemTarget.HPCastle - (10000);
									}
									else
									{
										itemTarget.Hp = itemTarget.Hp - (10000);
									}

									//消滅
									arrayNo.push_back(target.No);
								}
							}
						}
					}

					loop_Battle_player_skills.ArrayClassBullet.remove_if([&](const ClassBullets& cb)
						{
							if (arrayNo.includes(cb.No))
							{
								//Print << U"suc";
								return true;
							}
							else
							{
								//Print << U"no";
								return false;
							}
						});
					arrayNo.clear();
				}
				m_Battle_player_skills.remove_if([&](const ClassExecuteSkills& a) { return a.ArrayClassBullet.size() == 0; });
			}
			{
				Array<ClassExecuteSkills> deleteCES;
				for (ClassExecuteSkills& loop_Battle_player_skills : m_Battle_enemy_skills)
				{
					Array<int32> arrayNo;
					for (auto& target : loop_Battle_player_skills.ArrayClassBullet)
					{
						target.lifeTime += Scene::DeltaTime();

						if (target.lifeTime > target.duration)
						{
							loop_Battle_player_skills.classUnit->FlagMovingSkill = false;

							//消滅
							arrayNo.push_back(target.No);

							break;
						}
						else
						{
							target.NowPosition = Vec2((target.NowPosition.x + (target.MoveVec.x * (loop_Battle_player_skills.classSkill.speed / 100))),
														(target.NowPosition.y + (target.MoveVec.y * (loop_Battle_player_skills.classSkill.speed / 100))));
						}

						//衝突したらunitのHPを減らし、消滅
						Circle c = Circle{ target.NowPosition.x,target.NowPosition.y,30 };

						for (auto& itemTargetHo : getData().classGameStatus.classBattle.sortieUnitGroup)
						{
							for (auto& itemTarget : itemTargetHo.ListClassUnit)
							{
								if (itemTarget.IsBattleEnable == false)
								{
									continue;
								}

								Point pt = Point(itemTarget.rowBuilding, itemTarget.colBuilding);
								Vec2 vv = mapCreator.ToTileBottomCenter(pt, mapCreator.N);
								vv = { vv.x,vv.y - (25 + 15) };
								Circle cTar = Circle{ vv,30 };
								if (c.intersects(cTar) == true)
								{
									loop_Battle_player_skills.classUnit->FlagMovingSkill = false;

									if (itemTarget.IsBuilding == true)
									{
										itemTarget.HPCastle = itemTarget.HPCastle - (10000);
									}
									else
									{
										itemTarget.Hp = itemTarget.Hp - (10000);
									}

									//消滅
									arrayNo.push_back(target.No);
								}
							}
						}
					}

					loop_Battle_player_skills.ArrayClassBullet.remove_if([&](const ClassBullets& cb)
						{
							if (arrayNo.includes(cb.No))
							{
								//Print << U"suc";
								return true;
							}
							else
							{
								//Print << U"no";
								return false;
							}
						});
					arrayNo.clear();
				}
				m_Battle_enemy_skills.remove_if([&](const ClassExecuteSkills& a) { return a.ArrayClassBullet.size() == 0; });
			}

			//体力が無くなったunit削除処理
			for (auto& item : getData().classGameStatus.classBattle.sortieUnitGroup)
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					if (itemUnit.isValidBuilding() == true)
					{
						if (itemUnit.HPCastle <= 0)
						{
							itemUnit.IsBattleEnable = false;
						}
					}
					else
					{
						if (itemUnit.Hp <= 0)
						{
							itemUnit.IsBattleEnable = false;
						}
					}
				}
			}
			for (auto& item : getData().classGameStatus.classBattle.defUnitGroup)
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					if (itemUnit.isValidBuilding() == true)
					{
						if (itemUnit.HPCastle <= 0)
						{
							itemUnit.IsBattleEnable = false;
						}
					}
					else
					{
						if (itemUnit.Hp <= 0)
						{
							itemUnit.IsBattleEnable = false;
						}
					}
				}
			}
			for (auto& item : getData().classGameStatus.classBattle.neutralUnitGroup)
			{
				for (auto& itemUnit : item.ListClassUnit)
				{
					if (itemUnit.isValidBuilding() == true)
					{
						if (itemUnit.HPCastle <= 0)
						{
							itemUnit.IsBattleEnable = false;
						}
					}
					else
					{
						if (itemUnit.Hp <= 0)
						{
							itemUnit.IsBattleEnable = false;
						}
					}
				}
			}

			//戦闘終了条件を確認
			int32 countSortieUnitGroup = 0;
			for (auto& item : getData().classGameStatus.classBattle.sortieUnitGroup)
			{
				for (auto& itemListClassUnit : item.ListClassUnit)
				{
					if (itemListClassUnit.IsBattleEnable == true)
					{
						countSortieUnitGroup++;
					}
				}
			}

			if (countSortieUnitGroup == 0)
			{
				changeScene(U"Buy", 0.9s);
			}

			int32 countDefUnitGroup = 0;
			for (auto& item : getData().classGameStatus.classBattle.defUnitGroup)
			{
				for (auto& itemListClassUnit : item.ListClassUnit)
				{
					if (itemListClassUnit.IsBattleEnable == true)
					{
						countDefUnitGroup++;
					}
				}
			}

			if (countDefUnitGroup == 0)
			{
				getData().NovelNumber = getData().NovelNumber + 1;
				getData().Wave = getData().Wave + 1;

				changeScene(U"Novel", 0.9s);
			}
		}
		break;
		case BattleStatus::Message:
		{
			if (BattleMessage001)
			{
				sceneMessageBoxImpl.set(camera);
				if (sceneMessageBoxImpl.m_buttonC.mouseOver())
				{
					Cursor::RequestStyle(CursorStyle::Hand);

					if (MouseL.down())
					{
						BattleMessage001 = false;
						battleStatus = BattleStatus::Battle;



					}
				}
			}
		}
		break;
		case BattleStatus::Event:
			break;
		default:
			break;
		}

		// 非同期タスクが完了したら
		if (task.isReady())
		{
			//// 結果を取得する
			//Print << task.get();
		}
	}
	// 描画関数（オプション）
	void draw() const override
	{
		const auto t = camera.createTransformer();

		Array<ClassUnit> bui;
		bui.append(getData().classGameStatus.classBattle.sortieUnitGroup[0].ListClassUnit);
		bui.append(getData().classGameStatus.classBattle.defUnitGroup[0].ListClassUnit);
		bui.append(getData().classGameStatus.classBattle.neutralUnitGroup[0].ListClassUnit);

		Array<std::pair<Vec2, String>> buiTex;

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
				Vec2 pos = { (posX + mapCreator.TileOffset.x * 2 * k2), posY };

				// 底辺中央を基準にタイルを描く
				String tip = getData().classGameStatus.classBattle.classMapBattle.value().mapData[index.x][index.y].tip;
				TextureAsset(tip + U".png").draw(Arg::bottomCenter = pos);

				for (auto& abc : bui)
				{
					if (abc.IsBattleEnable == false)
					{
						continue;
					}

					if (abc.rowBuilding == (xi + k) && abc.colBuilding == (yi - k))
					{
						std::pair<Vec2, String> hhhh = { pos, abc.Image + U".png" };
						buiTex.push_back(hhhh);
					}
				}
			}
		}

		if (debugMap.size() != 0)
		{
			//// タイルのインデックス
			//const Point index{ (debugMap.back().begin()->GetRow()),(debugMap.back().begin()->GetCol()) };

			//// そのタイルの底辺中央の座標
			//const int32 i = index.manhattanLength();
			//const int32 xi = (i < (mapCreator.N - 1)) ? 0 : (i - (mapCreator.N - 1));
			//const int32 yi = (i < (mapCreator.N - 1)) ? i : (mapCreator.N - 1);
			//const int32 k2 = (index.manhattanDistanceFrom(Point{ xi, yi }) / 2);
			//const double posX = ((i < (mapCreator.N - 1)) ? (i * -mapCreator.TileOffset.x) : ((i - 2 * mapCreator.N + 2) * mapCreator.TileOffset.x));
			//const double posY = (i * mapCreator.TileOffset.y) - mapCreator.TileThickness;
			//const Vec2 pos = { (posX + mapCreator.TileOffset.x * 2 * k2), posY };

			//Circle ccccc = Circle{ Arg::bottomCenter = pos,30 };
			//ccccc.draw();
		}
		if (debugRoot.size() != 0)
		{
			//for (auto abcd : debugRoot.back())
			//{
			//	// タイルのインデックス
			//	const Point index{ abcd.x,abcd.y };

			//	// そのタイルの底辺中央の座標
			//	const int32 i = index.manhattanLength();
			//	const int32 xi = (i < (mapCreator.N - 1)) ? 0 : (i - (mapCreator.N - 1));
			//	const int32 yi = (i < (mapCreator.N - 1)) ? i : (mapCreator.N - 1);
			//	const int32 k2 = (index.manhattanDistanceFrom(Point{ xi, yi }) / 2);
			//	const double posX = ((i < (mapCreator.N - 1)) ? (i * -mapCreator.TileOffset.x) : ((i - 2 * mapCreator.N + 2) * mapCreator.TileOffset.x));
			//	const double posY = (i * mapCreator.TileOffset.y) - mapCreator.TileThickness;
			//	const Vec2 pos = { (posX + mapCreator.TileOffset.x * 2 * k2), posY };

			//	Circle ccccc = Circle{ Arg::bottomCenter = pos,30 };
			//	ccccc.draw(Palette::Red);
			//}
		}
		if (debugAstar.size() != 0)
		{
			//for (auto hfdfsjf : debugAstar)
			//{
			//	// タイルのインデックス
			//	const Point index{ hfdfsjf->GetRow(),hfdfsjf->GetCol()};

			//	// そのタイルの底辺中央の座標
			//	const int32 i = index.manhattanLength();
			//	const int32 xi = (i < (mapCreator.N - 1)) ? 0 : (i - (mapCreator.N - 1));
			//	const int32 yi = (i < (mapCreator.N - 1)) ? i : (mapCreator.N - 1);
			//	const int32 k2 = (index.manhattanDistanceFrom(Point{ xi, yi }) / 2);
			//	const double posX = ((i < (mapCreator.N - 1)) ? (i * -mapCreator.TileOffset.x) : ((i - 2 * mapCreator.N + 2) * mapCreator.TileOffset.x));
			//	const double posY = (i * mapCreator.TileOffset.y) - mapCreator.TileThickness;
			//	const Vec2 pos = { (posX + mapCreator.TileOffset.x * 2 * k2), posY };

			//	Circle ccccc = Circle{ Arg::bottomCenter = pos,15 };
			//	ccccc.draw(Palette::Yellow);
			//}
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
					if (itemUnit.IsBattleEnable == false)
					{
						continue;
					}
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
					if (itemUnit.IsBattleEnable == false)
					{
						continue;
					}
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

		//bui
		for (auto aaa : buiTex)
		{
			TextureAsset(aaa.second).draw(Arg::bottomCenter = aaa.first);
		}

		//範囲指定もしくは移動先矢印
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

		for (auto& skill : m_Battle_player_skills)
		{
			for (auto& acb : skill.ArrayClassBullet)
			{
				if (skill.classSkill.image == U"")
				{
					Circle{ acb.NowPosition.x,acb.NowPosition.y,30 }.draw();
				}
				else
				{
					if (acb.degree == 0 || acb.degree == 90 || acb.degree == 180 || acb.degree == 270)
					{
						TextureAsset(skill.classSkill.image + U"N.png").rotated(acb.radian + Math::ToRadians(90)).draw(acb.NowPosition.x, acb.NowPosition.y);
					}
					else
					{
						Line{ acb.StartPosition, acb.NowPosition }.draw(4, Palette::White);
						TextureAsset(skill.classSkill.image + U"NW.png").rotated(acb.radian + Math::ToRadians(135)).draw(acb.NowPosition.x - (TextureAsset(skill.classSkill.image + U"NW.png").size().x / 2), acb.NowPosition.y - (TextureAsset(skill.classSkill.image + U"NW.png").size().y / 2));
					}
				}
			}
		}
		for (auto& skill : m_Battle_enemy_skills)
		{
			for (auto& acb : skill.ArrayClassBullet)
			{
				if (skill.classSkill.image == U"")
				{
					Circle{ acb.NowPosition.x,acb.NowPosition.y,30 }.draw();
				}
				else
				{
					if (acb.degree == 0 || acb.degree == 90 || acb.degree == 180 || acb.degree == 270)
					{
						TextureAsset(skill.classSkill.image + U"N.png").rotated(acb.radian + Math::ToRadians(90)).draw(acb.NowPosition.x, acb.NowPosition.y);
					}
					else
					{
						Line{ acb.StartPosition, acb.NowPosition }.draw(4, Palette::White);
						TextureAsset(skill.classSkill.image + U"NW.png").rotated(acb.radian + Math::ToRadians(135)).draw(acb.NowPosition.x - (TextureAsset(skill.classSkill.image + U"NW.png").size().x / 2), acb.NowPosition.y - (TextureAsset(skill.classSkill.image + U"NW.png").size().y / 2));
					}
				}
			}
		}

		switch (battleStatus)
		{
		case BattleStatus::Battle:
			break;
		case BattleStatus::Message:
			sceneMessageBoxImpl.show(getData().classConfigString.BattleMessage001);
			break;
		case BattleStatus::Event:
			break;
		default:
			break;
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
	Array<ClassAStar*> debugAstar;
	AsyncTask<int32> task;
	Array<ClassHorizontalUnit> bui;
	Vec2 viewPos;
	Point cursPos = Cursor::Pos();
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
	Camera2D camera;
	bool BattleMessage001 = true;
	BattleStatus battleStatus = BattleStatus::Message;
	s3dx::SceneMessageBoxImpl sceneMessageBoxImpl;
	Array<ClassExecuteSkills> m_Battle_player_skills;
	Array<ClassExecuteSkills> m_Battle_enemy_skills;
	Array<ClassExecuteSkills> m_Battle_neutral_skills;
	Array<Array<ClassAStar>> debugMap;
	Array<Array<Point>> debugRoot;

	void SkillProcess(Array<ClassHorizontalUnit>& ach, Array<ClassHorizontalUnit>& achTarget, Array<ClassExecuteSkills>& aces)
	{
		for (auto& item : ach)
		{
			for (auto& itemUnit : item.ListClassUnit)
			{
				//発動中もしくは死亡ユニットはスキップ
				if (itemUnit.FlagMovingSkill == true || itemUnit.IsBattleEnable == false)
				{
					continue;
				}

				//昇順（小さい値から大きい値へ）
				for (const ClassSkill& itemSkill : itemUnit.Skill.sort_by([](const auto& item1, const auto& item2) { return  item1.sortKey < item2.sortKey; }))
				{
					//ターゲットとなるユニットを抽出し、
					//スキル射程範囲を確認
					const auto xA = itemUnit.GetNowPosiCenter();

					for (auto& itemTargetHo : achTarget)
					{
						for (auto& itemTarget : itemTargetHo.ListClassUnit)
						{
							//スキル発動条件確認
							if (itemTarget.IsBattleEnable == false)
							{
								continue;
							}
							if (itemTarget.IsBuilding == true)
							{
								if (itemTarget.HPCastle <= 0)
								{
									continue;
								}
							}

							//三平方の定理から射程内か確認
							const Vec2 xB = itemTarget.GetNowPosiCenter();
							const double teihen = xA.x - xB.x;
							const double takasa = xA.y - xB.y;
							const double syahen = (teihen * teihen) + (takasa * takasa);
							const double kyori = std::sqrt(syahen);

							const double xAHankei = (itemUnit.yokoUnit / 2.0) + itemSkill.range;
							const double xBHankei = itemTarget.yokoUnit / 2.0;

							bool check = true;
							if (kyori > (xAHankei + xBHankei))
							{
								check = false;
							}
							// チェック
							if (check == false)
							{
								continue;
							}

							int32 random = getData().classGameStatus.getBattleIDCount();
							int singleAttackNumber = random;

							itemUnit.FlagMovingSkill = true;

							//rush数だけ実行する
							int32 rushBase = 1;
							if (itemSkill.rush > 1) rushBase = itemSkill.rush;

							ClassExecuteSkills ces;
							ces.No = getData().classGameStatus.getDeleteCESIDCount();
							ces.classSkill = itemSkill;
							ces.classUnit = &itemUnit;

							for (int iii = 0; iii < rushBase; iii++)
							{
								ClassBullets cbItemUnit;
								cbItemUnit.No = singleAttackNumber;
								cbItemUnit.NowPosition = itemUnit.GetNowPosiCenter();
								cbItemUnit.StartPosition = itemUnit.GetNowPosiCenter();
								cbItemUnit.OrderPosition = itemTarget.GetNowPosiCenter();
								cbItemUnit.duration = (itemSkill.range + itemSkill.speed - 1) / itemSkill.speed;
								cbItemUnit.lifeTime = 0;

								Vec2 ve = cbItemUnit.OrderPosition - cbItemUnit.NowPosition;
								cbItemUnit.MoveVec = ve.normalized();

								//二点間の角度を求める
								cbItemUnit.radian = Math::Atan2((float)(cbItemUnit.OrderPosition.y - cbItemUnit.NowPosition.y),
													(float)(cbItemUnit.OrderPosition.x - cbItemUnit.NowPosition.x));
								cbItemUnit.degree = cbItemUnit.radian * (180 / Math::Pi);

								ces.ArrayClassBullet.push_back(cbItemUnit);
							}

							aces.push_back(ces);

							return;
						}
					}
				}
			}
		}
	}
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
				cp.Wave = Parse<int32>(value[U"Wave"].getString());
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
				// TOML ファイルからデータを読み込む
				const TOMLReader tomlInfoProcess{ U"001_Warehouse/001_DefaultGame/070_Scenario/InfoProcess/" + ttt.PowerTag + U".toml" };

				if (not tomlInfoProcess) // もし読み込みに失敗したら
				{
					throw Error{ U"Failed to load `tomlInfoProcess.toml`" };
				}

				for (const auto& table : tomlInfoProcess[U"Process"].tableArrayView()) {
					String map = table[U"map"].get<String>();
					getData().classGameStatus.arrayInfoProcessSelectCharaMap = map.split(U',');
					for (auto& map : getData().classGameStatus.arrayInfoProcessSelectCharaMap)
					{
						String ene = table[map].get<String>();
						getData().classGameStatus.arrayInfoProcessSelectCharaEnemyUnit = ene.split(U',');
					}
				}
				getData().classGameStatus.nowPowerTag = ttt.PowerTag;
				getData().NovelPower = ttt.PowerTag;
				getData().NovelNumber = 0;

				for (auto& aaa : getData().classGameStatus.arrayClassPower)
				{
					if (aaa.PowerTag == ttt.PowerTag)
					{
						getData().selectClassPower = aaa;
					}
				}

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
		int32 nn = getData().NovelNumber;
		String path = U"001_Warehouse/001_DefaultGame/070_Scenario/InfoStory/" + np + U"+" + Format(nn) + U".csv";
		csv = CSV{ path };
		if (not csv) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load " + np + U"+" + Format(nn) + U".csv" };
		}

		nowRow = 0;
		if (csv[nowRow][0].c_str()[0] == '#')
		{
			nowRow++;
		}

		rectText = { 50,Scene::Size().y - 325,Scene::Size().x - 100,300 };
		rectHelp = { 70,Scene::Size().y - 325 - 70,400,70 };
		rectFace = { Scene::Size().x - 100 - 206 - 50,Scene::Size().y - 325 + 50,206,206 };
		stopwatch = Stopwatch{ StartImmediately::Yes };
	}
	// 更新関数（オプション）
	void update() override
	{
		length = stopwatch.sF() / 0.05;
		if (csv[nowRow][0].substr(0, 3) == U"end")
		{
			if (getData().Wave >= getData().selectClassPower.Wave)
			{
				changeScene(U"Title", 0.9s);
			}
			else
			{
				changeScene(U"Buy", 0.9s);
			}
		}

		if (csv[nowRow][0].c_str()[0] == '#')
		{
			nowRow++;
		}

		if (MouseL.down() == true)
		{
			stopwatch.restart();
			length = 0;
			nowRow++;
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
	Stopwatch stopwatch;
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
		//左上
		arrayRectMenuBack.push_back(Rect{ 16,16,400,800 });
		//中央上
		arrayRectMenuBack.push_back(Rect{ 432,16,500,500 });
		////中央下
		//arrayRectMenuBack.push_back(Rect{ 432,516,500,500 });

		//メニューボタン
		{
			int32 counter = 0;
			if (getData().classGameStatus.strategyMenu000 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
			if (getData().classGameStatus.strategyMenu001 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
			if (getData().classGameStatus.strategyMenu002 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
			if (getData().classGameStatus.strategyMenu003 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
			if (getData().classGameStatus.strategyMenu004 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
			if (getData().classGameStatus.strategyMenu005 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
			if (getData().classGameStatus.strategyMenu006 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
			if (getData().classGameStatus.strategyMenu007 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
			if (getData().classGameStatus.strategyMenu008 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
			if (getData().classGameStatus.strategyMenu009 == true)
			{
				htMenuBtn.emplace(counter, Rect{ 32,32 + (counter * 64),300,64 });
			}
			counter++;
		}

		//初期化
		for (auto ttt : htMenuBtnDisplay)
		{
			ttt.second = false;
		}
		{
			int32 counter = 0;
			for (auto& ttt : getData().classGameStatus.arrayClassUnit)
			{
				ttt.rectExecuteBtnStrategyMenu = Rect{ 448,548 + (counter * 64),300,64 };
				counter++;
			}
		}
		vbar001.emplace(SasaGUI::Orientation::Vertical);;
		vbar002.emplace(SasaGUI::Orientation::Vertical);;
		vbar001.value().updateLayout({
			(int32)(432 + 500 + SasaGUI::ScrollBar::Thickness), (int32)(16),
			SasaGUI::ScrollBar::Thickness,
			(int32)500
		});
		vbar002.value().updateLayout({
			(int32)(432 + 500 + SasaGUI::ScrollBar::Thickness), (int32)516,
			SasaGUI::ScrollBar::Thickness,
			(int32)500
		});
		vbar001.value().updateConstraints(0.0, 2000.0, Scene::Height());
		vbar002.value().updateConstraints(0.0, 2000.0, Scene::Height());

	}
	// 更新関数（オプション）
	void update() override
	{
		Cursor::RequestStyle(U"MyCursor");

		switch (formBuyDisplayStatus)
		{
		case FormBuyDisplayStatus::Normal:
		{
			for (auto& ttt : htMenuBtn)
			{
				if (ttt.second.mouseOver())
				{
					htMenuBtnDisplay[0] = false;
					htMenuBtnDisplay[1] = false;
					htMenuBtnDisplay[2] = false;
					htMenuBtnDisplay[3] = false;
					htMenuBtnDisplay[4] = false;
					htMenuBtnDisplay[5] = false;
					htMenuBtnDisplay[6] = false;
					htMenuBtnDisplay[7] = false;
					htMenuBtnDisplay[8] = false;
					htMenuBtnDisplay[9] = false;
					htMenuBtnDisplay[ttt.first] = true;
				}
				switch (ttt.first)
				{
				case 9:
				{
					if (ttt.second.leftClicked() == true)
					{
						String targetMap = getData().classGameStatus.arrayInfoProcessSelectCharaMap[getData().Wave];

						const TOMLReader tomlMap{ U"001_Warehouse/001_DefaultGame/016_BattleMap/" + targetMap };
						if (not tomlMap) // もし読み込みに失敗したら
						{
							throw Error{ U"Failed to load `tomlMap`" };
						}

						ClassMap sM;
						for (const auto& table : tomlMap[U"Map"].tableArrayView()) {
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

						ClassBattle cb;
						const TOMLReader tomlInfoProcess{ U"001_Warehouse/001_DefaultGame/070_Scenario/InfoProcess/" + getData().classGameStatus.nowPowerTag + U".toml" };
						if (not tomlInfoProcess) // もし読み込みに失敗したら
						{
							throw Error{ U"Failed to load `tomlInfoProcess`" };
						}

						//建物関係
						cb.classMapBattle = ClassStaticCommonMethod::GetClassMapBattle(sM);
						ClassHorizontalUnit chuSor;
						ClassHorizontalUnit chuDef;
						ClassHorizontalUnit chuNa;
						chuSor.FlagBuilding = true;
						chuDef.FlagBuilding = true;
						chuNa.FlagBuilding = true;
						for (size_t indexRow = 0; indexRow < cb.classMapBattle.value().mapData.size(); ++indexRow)
						{
							for (size_t indexCol = 0; indexCol < cb.classMapBattle.value().mapData[indexRow].size(); ++indexCol)
							{
								for (auto& bui : cb.classMapBattle.value().mapData[indexRow][indexCol].building)
								{
									String key = std::get<0>(bui);
									BattleWhichIsThePlayer bw = std::get<2>(bui);

									// arrayClassObjectMapTip から適切な ClassObjectMapTip オブジェクトを見つける
									for (const auto& mapTip : getData().classGameStatus.arrayClassObjectMapTip)
									{
										if (mapTip.nameTag == key)
										{
											// ClassUnit の設定を行う
											ClassUnit unitBui;
											unitBui.IsBuilding = true;
											unitBui.ID = getData().classGameStatus.getIDCount();
											std::get<1>(bui) = unitBui.ID;
											unitBui.mapTipObjectType = mapTip.type;
											unitBui.NoWall2 = mapTip.noWall2;
											unitBui.HPCastle = mapTip.castle;
											unitBui.CastleDefense = mapTip.castleDefense;
											unitBui.CastleMagdef = mapTip.castleMagdef;
											unitBui.Image = mapTip.nameTag;
											unitBui.rowBuilding = indexRow;
											unitBui.colBuilding = indexCol;

											if (bw == BattleWhichIsThePlayer::Sortie)
											{
												chuSor.ListClassUnit.push_back(unitBui);
											}
											else if (bw == BattleWhichIsThePlayer::Def)
											{
												chuDef.ListClassUnit.push_back(unitBui);
											}
											else
											{
												chuNa.ListClassUnit.push_back(unitBui);
											}
											break; // 適切なオブジェクトが見つかったのでループを抜ける
										}
									}
								}
							}
						}
						cb.sortieUnitGroup.push_back(chuSor);
						cb.defUnitGroup.push_back(chuDef);
						cb.neutralUnitGroup.push_back(chuNa);

						for (const auto& table : tomlInfoProcess[U"Process"].tableArrayView()) {
							String mapUnit = table[targetMap].get<String>();
							Array<String> arrayMapUnit = mapUnit.split(U',');
							for (auto& unitYoko : arrayMapUnit)
							{
								ClassHorizontalUnit chu;
								Array<String> unitInfo = unitYoko.split(U'*');
								auto it = std::find_if(getData().classGameStatus.arrayClassUnit.begin(), getData().classGameStatus.arrayClassUnit.end(),
											[&](const ClassUnit& unit) { return unit.NameTag == unitInfo[0]; });
								if (it == getData().classGameStatus.arrayClassUnit.end())
								{
									continue;
								}
								for (size_t i = 0; i < Parse<int32>(unitInfo[1]); i++)
								{
									it->ID = getData().classGameStatus.getIDCount();
									chu.ListClassUnit.push_back(*it);
								}
								cb.defUnitGroup.push_back(chu);
							}
						}

						cb.battleWhichIsThePlayer = BattleWhichIsThePlayer::Sortie;
						//C++11以降では、std::move を使ってコピーを避け、効率的に要素を追加することもできます。
						//これは特に大きな Array オブジェクトを扱う場合に有用です
						cb.sortieUnitGroup.append(std::move(getData().classGameStatus.arrayPlayerUnit));

						getData().classGameStatus.classBattle = cb;

						changeScene(U"Battle", 0.9s);
					}
				}
				break;
				default:
					break;
				}
			}
			for (auto& ttt : htMenuBtnDisplay)
			{
				if (ttt.second == true)
				{
					rectExecuteBtn = Rect{ 432,516,500,500 };
				}
			}
			//徴兵処理
			for (auto& nowHtRectPlusUnit : getData().classGameStatus.arrayClassUnit)
			{
				if (nowHtRectPlusUnit.rectExecuteBtnStrategyMenu.leftClicked() == true)
				{
					bool check = true;
					int32 rowCounter = 0;
					for (auto che : getData().classGameStatus.arrayPlayerUnit)
					{
						for (auto che2 : che.ListClassUnit)
						{
							if (che2.pressedDetailStrategyMenu == true)
							{
								check = false;
								if (nowHtRectPlusUnit.Price > getData().Money)
								{
									formBuyDisplayStatus = FormBuyDisplayStatus::Message;
									Message001 = true;
									break;
								}

								getData().Money = getData().Money - nowHtRectPlusUnit.Price;

								ClassUnit cu = nowHtRectPlusUnit;
								cu.ID = getData().classGameStatus.getIDCount();
								cu.rectDetailStrategyMenu = Rect{ 432 + 16 + (getData().classGameStatus.arrayPlayerUnit[rowCounter].ListClassUnit.size() * 32),16 + 16 + (rowCounter * 32),32,32 };
								getData().classGameStatus.arrayPlayerUnit[rowCounter].ListClassUnit.push_back(cu);
								break;
							}
						}
						if (check == false)
						{
							break;
						}
						rowCounter++;
					}
					if (check == true)
					{
						if (nowHtRectPlusUnit.Price > getData().Money)
						{
							formBuyDisplayStatus = FormBuyDisplayStatus::Message;
							Message001 = true;
							break;
						}

						getData().Money = getData().Money - nowHtRectPlusUnit.Price;

						ClassHorizontalUnit chu;
						ClassUnit cu = nowHtRectPlusUnit;
						cu.ID = getData().classGameStatus.getIDCount();
						cu.rectDetailStrategyMenu = Rect{ 432 + 16,16 + 16 + (getData().classGameStatus.arrayPlayerUnit.size() * 32),32,32 };
						chu.ListClassUnit.push_back(cu);
						getData().classGameStatus.arrayPlayerUnit.push_back(chu);
					}
					else
					{

					}
				}
				else if (nowHtRectPlusUnit.rectExecuteBtnStrategyMenu.rightClicked() == true)
				{
					ClassHorizontalUnit chu;
					ClassUnit cu = nowHtRectPlusUnit;
					cu.rectDetailStrategyMenu = Rect{ 0,0,0,0 };
					chu.ListClassUnit.push_back(cu);
					getData().classGameStatus.arrayPlayerUnit.push_back(chu);
				}
			}
			//スクロール上
			{
				int32 counterUnit = 0;
				for (auto& nowarrayPlayerUnit : getData().classGameStatus.arrayPlayerUnit)
				{
					int32 yyy = 16 + 16 + (counterUnit * 32) - vbar001.value().value();
					for (auto& aaa : nowarrayPlayerUnit.ListClassUnit)
					{
						aaa.rectDetailStrategyMenu.y = yyy;
					}
					counterUnit++;
				}
			}
			//クリック処理
			for (auto& nowArrayPlayerUnit : getData().classGameStatus.arrayPlayerUnit)
			{
				for (auto& aaa : nowArrayPlayerUnit.ListClassUnit)
				{
					if (aaa.rectDetailStrategyMenu.leftClicked() == true)
					{
						for (auto& nowarrayPlayerUnit : getData().classGameStatus.arrayPlayerUnit)
						{
							for (auto& bbb : nowarrayPlayerUnit.ListClassUnit)
							{
								bbb.pressedDetailStrategyMenu = false;
							}
						}

						aaa.pressedDetailStrategyMenu = true;
					}
				}
			}

			if (arrayRectMenuBack[1].mouseOver() == true)
			{
				vbar001.value().scroll(Mouse::Wheel() * 60);
			}
			if (rectExecuteBtn.mouseOver() == true)
			{
				vbar002.value().scroll(Mouse::Wheel() * 60);
			}
			vbar001.value().update();
			vbar002.value().update();

		}
		break;
		case FormBuyDisplayStatus::Message:
		{
			if (Message001)
			{
				sceneMessageBoxImpl.set();
				if (sceneMessageBoxImpl.m_buttonC.mouseOver())
				{
					Cursor::RequestStyle(CursorStyle::Hand);

					if (MouseL.down())
					{
						Message001 = false;
						formBuyDisplayStatus = FormBuyDisplayStatus::Normal;
					}
				}
			}
		}
		break;
		case FormBuyDisplayStatus::Event:
			break;
		default:
			break;
		}
	}
	// 描画関数（オプション）
	void draw() const override
	{
		getData().slice9.draw(rectExecuteBtn);

		for (auto& ttt : arrayRectMenuBack)
		{
			getData().slice9.draw(ttt);
		}
		for (auto& ttt : getData().classGameStatus.arrayPlayerUnit)
		{
			for (auto& aaa : ttt.ListClassUnit)
			{
				if (aaa.rectDetailStrategyMenu.y > (13 * 32) + 16 + 16 || aaa.rectDetailStrategyMenu.y < 16 + 15)
				{

				}
				else
				{
					if (aaa.pressedDetailStrategyMenu == true)
					{
						aaa.rectDetailStrategyMenu(TextureAsset(aaa.Image)).draw().drawFrame(0, 3, Palette::Red);
					}
					else
					{
						aaa.rectDetailStrategyMenu(TextureAsset(aaa.Image)).draw().drawFrame(0, 3, Palette::Orange);
					}
				}
			}
		}
		for (auto& ttt : htMenuBtn)
		{
			getData().slice9.draw(ttt.second);
			switch (ttt.first)
			{
			case 0:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu000).draw(ttt.second.stretched(-10));
				break;
			case 1:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu001).draw(ttt.second.stretched(-10));
				break;
			case 2:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu002).draw(ttt.second.stretched(-10));
				break;
			case 3:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu003).draw(ttt.second.stretched(-10));
				break;
			case 4:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu004).draw(ttt.second.stretched(-10));
				break;
			case 5:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu005).draw(ttt.second.stretched(-10));
				break;
			case 6:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu006).draw(ttt.second.stretched(-10));
				break;
			case 7:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu007).draw(ttt.second.stretched(-10));
				break;
			case 8:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu008).draw(ttt.second.stretched(-10));
				break;
			case 9:
				getData().fontBuyMenu(getData().classConfigString.strategyMenu009).draw(ttt.second.stretched(-10));
				getData().fontBuyMenu(getData().Money).draw(ttt.second.stretched(-10).moveBy(0, ttt.second.h));
				break;
			default:
				break;
			}
		}
		for (auto& ttt : htMenuBtnDisplay)
		{
			switch (ttt.first)
			{
			case 0:
				if (ttt.second == true)
				{
					for (auto nowHtRectPlusUnit : getData().classGameStatus.arrayClassUnit)
					{
						getData().slice9.draw(nowHtRectPlusUnit.rectExecuteBtnStrategyMenu);
						getData().fontBuyMenu(nowHtRectPlusUnit.Name).draw(nowHtRectPlusUnit.rectExecuteBtnStrategyMenu.stretched(-10));
						vbar002.value().draw();
					}
				}
				break;
			case 1:
				break;
			case 2:
				break;
			case 3:
				break;
			case 4:
				break;
			case 5:
				break;
			case 6:
				break;
			case 7:
				break;
			case 8:
				break;
			case 9:
				break;
			default:
				break;
			}
		}

		vbar001.value().draw();

		switch (formBuyDisplayStatus)
		{
		case FormBuyDisplayStatus::Normal:
			break;
		case FormBuyDisplayStatus::Message:
			sceneMessageBoxImpl.show(getData().classConfigString.BuyMessage001);
			break;
		case FormBuyDisplayStatus::Event:
			break;
		default:
			break;
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
	/// @brief 左上メニュー、画面上部ユニット群の強制表示枠
	Array <Rect> arrayRectMenuBack;
	/// @brief 画面下部の詳細実行枠
	Rect rectExecuteBtn{ 0,0,0,0 };
	HashTable<int32, Rect> htMenuBtn;
	HashTable<int32, bool> htMenuBtnDisplay;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
	Optional<SasaGUI::ScrollBar> vbar001;
	Optional<SasaGUI::ScrollBar> vbar002;
	bool Message001 = false;
	FormBuyDisplayStatus formBuyDisplayStatus = FormBuyDisplayStatus::Normal;
	s3dx::SceneMessageBoxImpl sceneMessageBoxImpl;

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
	Window::SetTitle(U"R-Ver0.1");

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
	for (const auto& filePath : FileSystem::DirectoryContents(U"001_Warehouse/001_DefaultGame/040_ChipImage/"))
	{
		String filename = FileSystem::FileName(filePath);
		TextureAsset::Register(filename, filePath);
	}
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
			cp.Money = Parse<int32>(value[U"Money"].getString());
			cp.Wave = Parse<int32>(value[U"Wave"].getString());
			manager.get().get()->classGameStatus.arrayClassPower.push_back(std::move(cp));
		}
	}

	// config.tomlからデータを読み込む
	{
		const TOMLReader tomlConfig{ U"001_Warehouse/001_DefaultGame/config.toml" };

		if (not tomlConfig) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load `config.toml`" };
		}
		manager.get().get()->classGameStatus.DistanceBetweenUnit = tomlConfig[U"config.DistanceBetweenUnit"].get<int32>();
		manager.get().get()->classGameStatus.DistanceBetweenUnitTate = tomlConfig[U"config.DistanceBetweenUnitTate"].get<int32>();
		manager.get().get()->classGameStatus.strategyMenu000 = tomlConfig[U"config.strategyMenu000"].get<bool>();
		manager.get().get()->classGameStatus.strategyMenu001 = tomlConfig[U"config.strategyMenu001"].get<bool>();
		manager.get().get()->classGameStatus.strategyMenu002 = tomlConfig[U"config.strategyMenu002"].get<bool>();
		manager.get().get()->classGameStatus.strategyMenu003 = tomlConfig[U"config.strategyMenu003"].get<bool>();
		manager.get().get()->classGameStatus.strategyMenu004 = tomlConfig[U"config.strategyMenu004"].get<bool>();
		manager.get().get()->classGameStatus.strategyMenu005 = tomlConfig[U"config.strategyMenu005"].get<bool>();
		manager.get().get()->classGameStatus.strategyMenu006 = tomlConfig[U"config.strategyMenu006"].get<bool>();
		manager.get().get()->classGameStatus.strategyMenu007 = tomlConfig[U"config.strategyMenu007"].get<bool>();
		manager.get().get()->classGameStatus.strategyMenu008 = tomlConfig[U"config.strategyMenu008"].get<bool>();
		manager.get().get()->classGameStatus.strategyMenu009 = tomlConfig[U"config.strategyMenu009"].get<bool>();
	}
	// Unit.jsonからデータを読み込む
	{
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
			cu.Hp = Parse<int32>(value[U"hp"].getString());
			cu.Attack = Parse<int32>(value[U"attack"].getString());
			cu.Speed = Parse<double>(value[U"speed"].getString());
			cu.Price = Parse<int32>(value[U"price"].getString());
			cu.Move = Parse<int32>(value[U"move"].getString());
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

		// unitのスキルを読み込み
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
			cu.range = Parse<int32>(value[U"range"].get<String>());
			cu.w = Parse<int32>(value[U"w"].get<String>());
			cu.speed = Parse<int32>(value[U"speed"].get<String>());
			cu.image = value[U"image"].get<String>();
			TextureAsset::Register(cu.image + U"NW.png", U"001_Warehouse/001_DefaultGame/042_ChipImageSkillEffect/" + cu.image + U"NW.png");
			TextureAsset::Register(cu.image + U"N.png", U"001_Warehouse/001_DefaultGame/042_ChipImageSkillEffect/" + cu.image + U"N.png");

			arrayClassSkill.push_back(std::move(cu));
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

		manager.get().get()->classGameStatus.arrayClassUnit = arrayClassUnit;
	}
	// obj.jsonからデータを読み込む
	{
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
		manager.get().get()->classGameStatus.arrayClassObjectMapTip = arrayClassObj;
	}

	if (System::GetCommandLineArgs().size() == 0)
	{

	}
	else
	{
		//manager.init(U"SelectLang");

		//manager.init(U"TestBattle");
		//manager.init(U"Title");

		//manager.get().get()->NovelPower = U"sc_a_p_b";
		//manager.get().get()->NovelNumber = U"0";
		//manager.init(U"Novel");

		{
			manager.get().get()->classConfigString = ClassStaticCommonMethod::GetClassConfigString(U"en");
			const TOMLReader tomlInfoProcess{ U"001_Warehouse/001_DefaultGame/070_Scenario/InfoProcess/sc_a_p_b.toml" };

			if (not tomlInfoProcess) // もし読み込みに失敗したら
			{
				throw Error{ U"Failed to load `tomlInfoProcess.toml`" };
			}

			for (const auto& table : tomlInfoProcess[U"Process"].tableArrayView()) {
				String map = table[U"map"].get<String>();
				manager.get().get()->classGameStatus.arrayInfoProcessSelectCharaMap = map.split(U',');
				for (auto& map : manager.get().get()->classGameStatus.arrayInfoProcessSelectCharaMap)
				{
					String ene = table[map].get<String>();
					manager.get().get()->classGameStatus.arrayInfoProcessSelectCharaEnemyUnit = ene.split(U',');
				}
			}
			manager.get().get()->classGameStatus.nowPowerTag = U"sc_a_p_b";
			manager.get().get()->NovelPower = U"sc_a_p_b";
			manager.get().get()->NovelNumber = 0;
			manager.init(U"Buy");

			for (auto& aaa : manager.get().get()->classGameStatus.arrayClassPower)
			{
				if (aaa.PowerTag == manager.get().get()->NovelPower)
				{
					manager.get().get()->selectClassPower = aaa;
					manager.get().get()->Money = aaa.Money;
				}
			}

		}

	}

	while (System::Update())
	{
		if (not manager.update())
		{
			break;
		}
	}
}
