﻿# include <Siv3D.hpp> // OpenSiv3D v0.6.9
# include "ClassUnit.h" 
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
struct structTestBattle
{
	String name = U"";
	String map = U"";
	Array<String> memberKougeki;
	Array<String> memberBouei;
	Zinei player = Zinei::None;
};
struct structMap
{
	String name = U"";
	Array<std::pair<String, String>> ele;
	Array<Array<String>> data;
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
			// 遷移
			changeScene(U"Title", 0.9s);
		}
		if (rectBtnJa.leftClicked())
		{
			AudioAsset(U"click").play();
			language = Language::Japan;
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
	String text1;
	int32 buttonEngX = -1;
	int32 buttonEngY = -1;
	String text2;
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
		// TOML ファイルからデータを読み込む
		const TOMLReader toml{ U"001_Warehouse/001_DefaultGame/070_Scenario/InfoTestBattle/testBattle.toml" };

		if (not toml) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load `testBattle.toml`" };
		}

		Array<structTestBattle> arrayStructTestBattle;
		{
			for (const auto& table : toml[U"TestBattle"].tableArrayView()) {
				structTestBattle tb;
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
					Zinei z;
					if (str == U"Def")
					{
						z = Zinei::Def;
					}
					else if (str == U"Sortie")
					{
						z = Zinei::Sortie;
					}
					else
					{
						z = Zinei::None;
					}
					tb.player = z;
				}

				arrayStructTestBattle << tb;
			}
		}

		// TOML ファイルからデータを読み込む
		const TOMLReader tomlTestMap{ U"001_Warehouse/001_DefaultGame/070_Scenario/InfoTestBattle/testMap.toml" };

		if (not tomlTestMap) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load `testMap.toml`" };
		}

		structMap sM;
		{
			for (const auto& table : tomlTestMap[U"Map"].tableArrayView()) {
				{
					const String name = table[U"name"].get<String>();
				}
				int32 counter = 0;
				while (true)
				{
					String aaa = U"ele{}"_fmt(counter);
					const String ele = table[aaa].get<String>();
					sM.ele.push_back(std::make_pair(aaa, ele));
					counter++;
					if (ele == U"")
					{
						break;
					}
				}
				{
					const String str = table[U"data"].get<String>();
					const Array<String> strArray = str.split(U'@');
					sM.data.push_back(strArray);
					//for (auto& s : strArray)
					//{
					//}
				}
			}
		}

		// TOML ファイルからデータを読み込む
		const TOMLReader tomlUnit{ U"001_Warehouse/001_DefaultGame/070_Scenario/InfoUnit/genUnit002.toml" };

		if (not tomlUnit) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load `gen_unit_002.toml`" };
		}

		Array<ClassUnit> arrayClassUnit;
		{
			for (const auto& table : tomlUnit[U"Unit"].tableArrayView()) {
				ClassUnit cu;
				cu.NameTag = table[U"name_tag"].get<String>();
				cu.Name = table[U"name"].get<String>();

				arrayClassUnit.push_back(std::move(cu));
			}

		}
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
class common : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	common(const InitData& init)
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

	if (System::GetCommandLineArgs().size() == 0)
	{

	}
	else
	{
		manager.init(U"TestBattle");
	}

	while (System::Update())
	{
		if (not manager.update())
		{
			break;
		}
	}
}
