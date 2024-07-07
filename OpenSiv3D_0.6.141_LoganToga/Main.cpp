# include <Siv3D.hpp> // Siv3D v0.6.14
# include "000_SystemString.h"
# include "001_GLOBAL.h"
# include "005_GameUIToolkit.h"
# include "100_StructEffect.h"
# include "101_StructHomography.h"
# include "102_StructGameData.h"
# include "150_enumClassLanguageSteamFullSuport.h"
# include "200_ClassGaussianClass.h"
# include "201_ClassPauseWindow.h"

using App = SceneManager<String, GameData>;

auto randomFade()
{
	Array<std::function<std::unique_ptr<IFade>()>> makeFadeFuncs = {
		[]() -> std::unique_ptr<IFade> { return std::make_unique<Fade4>(); },
	};

	return Sample(makeFadeFuncs)();
}
void DrawUnder000()
{
	//枠
	Rect{ Scene::Size().x - 1, 0, 1, Scene::Size().y }.draw(Palette::Yellow);
	Rect{ 0, 0, Scene::Size().x, 1 }.draw(Palette::Yellow);
	Rect{ 0, 0, 1, Scene::Size().y }.draw(Palette::Yellow);
	//下のエリア
	Rect{ 0, Scene::Size().y - 30, Scene::Size().x, 30 }.draw(Palette::Yellow);
}
void DrawUnder001()
{
	//exit
	Shape2D::Cross(10, 5, Vec2{ Scene::Size().x - 10, Scene::Size().y - 10 }).draw(Palette::Black);
}
// 描画された最大のアルファ成分を保持するブレンドステートを作成する
BlendState MakeBlendState()
{
	BlendState blendState = BlendState::Default2D;
	blendState.srcAlpha = Blend::SrcAlpha;
	blendState.dstAlpha = Blend::DestAlpha;
	blendState.opAlpha = BlendOp::Max;
	return blendState;
}
void SetWindSize(int32 w, int32 h)
{
	// ゲームのシーンサイズ
	const Size sceneSize{ w, h };
	// 必要なシーンサイズよりやや大きめの領域（タイトルバーやフレームを考慮）
	const Size requiredAreadSize{ sceneSize + Size{ 60, 10 } };
	// プレイヤーのワークエリア（画面サイズからタスクバーを除いた領域）のサイズ
	const Size workAreaSize = System::GetCurrentMonitor().workArea.size;
	// OS の UI のスケール（多くの場合 1.0～2.0）
	const double uiScaling = Window::GetState().scaling;
	// UI スケールを考慮したワークエリアサイズ
	const Size availableWorkAreaSize = (SizeF{ workAreaSize } / uiScaling).asPoint();
	// ゲームのシーンサイズがプレイヤーのワークエリア内に収まるか
	const bool ok = (requiredAreadSize.x <= availableWorkAreaSize.x) && (requiredAreadSize.y <= availableWorkAreaSize.y);

	if (ok)
	{
		Window::Resize(sceneSize);
	}
	else
	{
		// UI 倍率 1.0 相当でリサイズ
		Scene::SetResizeMode(ResizeMode::Keep);
		Scene::Resize(sceneSize);
		Window::ResizeActual(sceneSize);
	}
}
// オリジナルのシーンを何倍すればよいかを返す関数
double CalculateScale(const Vec2& baseSize, const Vec2& currentSize)
{
	return Min((currentSize.x / baseSize.x), (currentSize.y / baseSize.y));
}
// 画面の中央に配置するためのオフセットを返す関数
Vec2 CalculateOffset(const Vec2& baseSize, const Vec2& currentSize)
{
	return ((currentSize - baseSize * CalculateScale(baseSize, currentSize)) / 2.0);
}
void DragProcess(Optional<std::pair<Point, Point>>& dragStart)
{
	// ドラッグ処理
	if (Rect{ 0, 0, Scene::Size().x, 60 }.mouseOver() == true)
	{
		Cursor::RequestStyle(U"MyCursorHand");
	}
	if (dragStart)
	{
		if (not MouseL.pressed())
		{
			dragStart.reset();
		}
		else
		{
			Window::SetPos(dragStart->second + (Cursor::ScreenPos() - dragStart->first));
		}
	}

	// ドラッグの開始
	if (Rect{ 0, 0, Scene::Size().x, 60 }.leftClicked())
	{
		dragStart = { Cursor::ScreenPos(), Window::GetState().bounds.pos };
	}
}
void WriteIni(INI ini)
{
	if (not ini) // もし読み込みに失敗したら
	{
		ini.write(U"data", U"winSize", 1600);
		ini.write(U"data", U"winSizeCheck", false);
		ini.write(U"data", U"FluExit", true);
		ini.save(U"data.ini");
	}
}

class ClassSelectLang {
public:
	String lang = U"";
	RectF btnRectF = RectF{};
	bool isDisplayBtnRectF = true;
	int32 SortKey = 0;
};

std::unique_ptr<GaussianClass> m_gaussianClass;
PauseWindow m_pauseWindow = PauseWindow();
LanguageSteamFullSuport language;
SystemString systemString;

// タイトルシーン
/// @brief 言語選択シーン
class SelectLang : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	SelectLang(const InitData& init)
		: IScene{ init }
	{
		int32 counterAll = 0;
		int32 counter = -1;
		int32 counterCol = 0;
		for (auto ttt : LANDIS)
		{
			ClassSelectLang csl;
			csl.SortKey = counterAll;
			csl.isDisplayBtnRectF = true;
			csl.lang = ttt.second;
			RectF rectText = getData().fontNormal(ttt.second).region();
			if (counterCol % 3 == 0)
			{
				counterCol = 0;
				counter++;
			}
			else
			{
			}

			csl.btnRectF = RectF{ 100 / 2,(300) + (counterAll * (rectText.h + 20)) ,Scene::Size().x - 100,rectText.h + 20 };

			acsl.push_back(csl);
			counterCol++;
			counterAll++;
		}

		vbar001.emplace(SasaGUI::Orientation::Vertical);

		//仮置き
		language = LanguageSteamFullSuport::English;
	}

	// 更新関数（オプション）
	void update() override
	{
		for (auto& ttt : acsl)
		{
			if (ttt.btnRectF.leftClicked())
			{
				AudioAsset(U"click").play();

				LangFunc(ttt.lang);
				TextureSet();

				Optional<INI> ini{ std::in_place, U"/data.ini" };
				INI ini2 = INI(U"data.ini");
				WriteIni(ini2);
				INI ini3 = INI(U"data.ini");
				ini.emplace(ini3);

				bool temp = Parse<bool>(ini.value()[U"data.winSizeCheck"]);
				if (temp)
				{
					int32 tempWinSize = Parse<int32>(ini.value()[U"data.winSizeCheck"]);
					Size BaseSceneSize;
					if (tempWinSize == 1600)
					{
						BaseSceneSize = { WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000 };
						SetWindSize(BaseSceneSize.x, BaseSceneSize.y);
					}
					else
					{
						BaseSceneSize = { WINDOWSIZEWIDTH001, WINDOWSIZEHEIGHT001 };
						SetWindSize(BaseSceneSize.x, BaseSceneSize.y);
					}

					// シーンの拡大倍率を計算する
					SCALE = CalculateScale(BaseSceneSize, Scene::Size());
					OFFSET = CalculateOffset(BaseSceneSize, Scene::Size());

					changeScene(U"TitleScene");
				}
				else
				{
					changeScene(U"WinSizeScene");
				}

			}
		}
	}

	void LangFunc(String lang)
	{
		const JSON jsonLang = JSON::Load(PathLang + U"/SystemString.json");

		if (not jsonLang)throw Error{ U"Failed to load `SystemString.json`" };

		for (const auto& [key, value] : jsonLang[U"lang"]) {

			if (
				lang == U"日本語" && (value[U"lang"].getString() == U"Japan")
				)
			{
				SystemString lang;
				lang.TopMenuTitle = value[U"TopMenuTitle"].getString();
				lang.AppTitle = value[U"AppTitle"].getString();
				systemString = lang;
			}
		}
	}

	void TextureSet()
	{
	}

	// 描画関数（オプション） 
	void draw() const override
	{
		for (auto& ttt : acsl)
		{
			getData().slice9.draw(ttt.btnRectF.asRect());
			getData().fontMini(ttt.lang).draw(Arg::center = ttt.btnRectF.stretched(-5).center());
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
	Optional<SasaGUI::ScrollBar> vbar001;
	Array<ClassSelectLang> acsl;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
};
class TitleScene : public App::Scene {
public:
	TitleScene(const InitData& init) : IScene(init) {
		INI ini = INI(U"data.ini");
		WriteIni(ini);
		int32 tempWinSize = Parse<int32>(ini[U"data.winSize"]);
		if (tempWinSize == 1600)
		{
			SetWindSize(WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000);
		}
		else
		{
			SetWindSize(WINDOWSIZEWIDTH001, WINDOWSIZEHEIGHT001);
		}

		EXITBTNPOLYGON = Shape2D::Cross(10, 5, Vec2{ Scene::Size().x - 10, Scene::Size().y - 10 }).asPolygon();
		EXITBTNRECT = Rect{ Arg::center(EXITBTNPOLYGON.centroid().asPoint()),20,20 };

		m_gaussianClass->SetSize(Scene::Size());
	}
	void update() override {
		if (m_startButton.leftClicked()) {
			changeScene(U"GameScene000");
		}
	}

	void draw() const override {
		getData().fontNormal(SYSTEMSTRING.TopMenuTitle).draw(12, 10, Palette::White);
		m_startButton.draw(Palette::Skyblue).drawFrame(2, Palette::Black);
		m_DiscoButton.draw(Palette::Skyblue).drawFrame(2, Palette::Black);

		//仮置き
		getData().fontMini(U"Start").drawAt(m_startButton.center(), Palette::Black);
		getData().fontMini(U"Discord").drawAt(m_DiscoButton.center(), Palette::Black);
	}
private:
	Rect m_startButton = Rect(12, 100, 200, 60);
	Rect m_DiscoButton = Rect(12, 160 + 10, 200, 60);
};
class WinSizeScene : public App::Scene {
public:
	WinSizeScene(const InitData& init) : IScene(init) {
		Size tempSize = { 600,300 };
		Window::Resize(tempSize);

		// シーンの拡大倍率を計算する
		const Size BaseSceneSize{ WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000 };
		SCALE = CalculateScale(BaseSceneSize, tempSize);
		OFFSET = CalculateOffset(BaseSceneSize, tempSize);

		INI aa = INI(U"data.ini");
		WriteIni(aa);
		ini.emplace(aa);  // .emplace() で再代入

		m_gaussianClass->SetSize(Scene::Size());
	}
	void update() override {
		if (m_000Button.leftClicked() || m_001Button.leftClicked()) {
			changeScene(U"TitleScene");

			Size ss;
			if (m_000Button.leftClicked())
			{
				ini->write(U"data", U"winSize", 1600);
				ss = { WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000 };
			}
			else
			{
				ini->write(U"data", U"winSize", 1200);
				ss = { WINDOWSIZEWIDTH001, WINDOWSIZEHEIGHT001 };
			}
			ini->write(U"data", U"winSizeCheck", checked0);
			ini->save(U"data.ini");

			// シーンの拡大倍率を計算する
			const Size BaseSceneSize{ WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000 };
			SCALE = CalculateScale(BaseSceneSize, ss);
			OFFSET = CalculateOffset(BaseSceneSize, ss);
		}
		SimpleGUI::CheckBox(checked0, U"もう表示しない", m_001Button.movedBy(0, 60).pos);
	}

	void draw() const override {
		getData().fontNormal(U"Window Size").draw(12, 10, Palette::White);
		m_000Button.draw(Palette::Skyblue).drawFrame(2, Palette::Black);
		m_001Button.draw(Palette::Skyblue).drawFrame(2, Palette::Black);

		//仮置き
		getData().fontMini(U"1600*900").drawAt(m_000Button.center(), Palette::Black);
		getData().fontMini(U"1200*600").drawAt(m_001Button.center(), Palette::Black);
	}
private:
	Optional<INI> ini{ std::in_place, U"/data.ini" };
	Rect m_000Button = Rect(12, 100, 200, 60);
	Rect m_001Button = Rect(12, 160 + 10, 200, 60);
	bool checked0 = false;
};

void Main()
{
	// ウィンドウの枠を非表示にする
	Window::SetStyle(WindowStyle::Frameless);

	//中央に配置
	Window::Centering();

	// 背景の色を設定する | Set the background color
	Scene::SetBackground(ColorF(U"#0F040D"));

	App manager;
	manager.add<TitleScene>(U"TitleScene");
	manager.add<WinSizeScene>(U"WinSizeScene");
	manager.add<SelectLang>(U"SelectLang");
	manager.init(U"SelectLang");

	Size tempSize = { INIT_WINDOW_SIZE_WIDTH,INIT_WINDOW_SIZE_HEIGHT };
	Window::Resize(tempSize);

	// 関数を格納するArrayを定義する
	Array<std::function<void()>> functions;
	functions.push_back(DrawUnder000);
	functions.push_back(DrawUnder001);
	m_gaussianClass = std::make_unique<GaussianClass>(Scene::Size(), functions);

	Optional<std::pair<Point, Point>> dragStart;
	EXITBTNPOLYGON = Shape2D::Cross(10, 5, Vec2{ Scene::Size().x - 10, Scene::Size().y - 10 }).asPolygon();
	EXITBTNRECT = Rect{ Arg::center(EXITBTNPOLYGON.centroid().asPoint()),20,20 };

	while (System::Update())
	{
		if (IS_SCENE_MODAL_PAUSED)
		{
			// ポーズ状態ならポーズウィンドウを描画
			m_pauseWindow.draw(manager.get().get()->fontLine);
			return;
		}

		m_gaussianClass->Show();

		//タイトル表示
		manager.get().get()->fontLine(systemString.AppTitle).draw(5, Scene::Size().y - 30, Palette::Black);

		if (not manager.update())
		{
			break;
		}

		if (EXITBTNRECT.leftClicked() == true)
		{
			break;
		}

		DragProcess(dragStart);
	}

	m_gaussianClass.reset();

}
