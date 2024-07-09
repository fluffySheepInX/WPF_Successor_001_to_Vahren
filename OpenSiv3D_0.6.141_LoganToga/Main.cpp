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
# include "205_ClassScenario.h" 
# include "210_ClassStaticCommonMethod.h" 

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

			csl.btnRectF = RectF{ 100 / 2,(300) + (counterAll * (rectText.h + 40)) ,Scene::Size().x - 100,rectText.h + 40 };
			//csl.btnRectF = RectF{ 100 / 2,(300) + (counterAll * (rectText.h + 20)) ,Scene::Size().x - 100,rectText.h + 20 };

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
				SystemString ss;
				ss.TopMenuTitle = value[U"TopMenuTitle"].getString();
				ss.AppTitle = value[U"AppTitle"].getString();
				systemString = ss;
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
			getData().slice9Cy.draw(ttt.btnRectF.asRect());
			getData().fontLine(ttt.lang).draw(Arg::center = ttt.btnRectF.center(), Palette::Antiquewhite);
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

		System::Update();
	}
	void update() override {
		if (m_000Button.leftClicked() || m_001Button.leftClicked()) {
			if (m_000Button.leftClicked())
			{
				ini->write(U"data", U"winSize", 1600);
			}
			else
			{
				ini->write(U"data", U"winSize", 1200);
			}
			ini->write(U"data", U"winSizeCheck", checked0);
			ini->save(U"data.ini");

			changeScene(U"TitleScene");
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
class TitleScene : public App::Scene {
public:
	TitleScene(const InitData& init) : IScene(init) {
		INI ini = INI(U"data.ini");
		WriteIni(ini);
		int32 tempWinSize = Parse<int32>(ini[U"data.winSize"]);
		Size ss;
		if (tempWinSize == 1600)
		{
			ss = { WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000 };
			SetWindSize(WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000);
		}
		else
		{
			ss = { WINDOWSIZEWIDTH001, WINDOWSIZEHEIGHT001 };
			SetWindSize(WINDOWSIZEWIDTH001, WINDOWSIZEHEIGHT001);
		}

		EXITBTNPOLYGON = Shape2D::Cross(10, 5, Vec2{ Scene::Size().x - 10, Scene::Size().y - 10 }).asPolygon();
		EXITBTNRECT = Rect{ Arg::center(EXITBTNPOLYGON.centroid().asPoint()),20,20 };

		// シーンの拡大倍率を計算する
		const Size BaseSceneSize{ WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000 };
		SCALE = CalculateScale(BaseSceneSize, ss);
		OFFSET = CalculateOffset(BaseSceneSize, ss);
		m_gaussianClass->SetSize(Scene::Size());

		System::Update();

		// TOML ファイルからデータを読み込む
		const TOMLReader tomlConfig{ PATHBASE + PATH_DEFAULT_GAME + U"/config.toml" };

		if (not tomlConfig) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load `config.toml`" };
		}

		TitleMenuX = tomlConfig[U"config.TitleMenuX"].get<int32>();
		TitleMenuY = tomlConfig[U"config.TitleMenuY"].get<int32>();
		space = tomlConfig[U"config.TitleMenuSpace"].get<int32>();

		for (const auto& filePath : FileSystem::DirectoryContents(PATHBASE + PATH_DEFAULT_GAME + U"/000_SystemImage/000_TitleMenuImage/"))
		{
			String filename = FileSystem::FileName(filePath);
			TextureAsset::Register(filename, filePath);
		}
		easyRect = Rect(TitleMenuX, TitleMenuY, 160, 40);
		normalRect = Rect(TitleMenuX, (TitleMenuY + (40 * 1)) + space * 1, 160, 40);
		hardRect = Rect(TitleMenuX, (TitleMenuY + (40 * 2)) + space * 2, 160, 40);
		lunaRect = Rect(TitleMenuX, (TitleMenuY + (40 * 3)) + space * 3, 160, 40);
	}
	void update() override {
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

	void draw() const override {
		TextureAsset(U"0001_easy.png").draw(TitleMenuX, TitleMenuY);
		TextureAsset(U"0002_normal.png").draw(TitleMenuX, (TitleMenuY + (40 * 1)) + space * 1);
		TextureAsset(U"0003_hard.png").draw(TitleMenuX, (TitleMenuY + (40 * 2)) + space * 2);
		TextureAsset(U"0004_luna.png").draw(TitleMenuX, (TitleMenuY + (40 * 3)) + space * 3);
		easyRect.draw(ColorF{ 0, 0, 0, 0 });
		normalRect.draw(ColorF{ 0, 0, 0, 0 });
		hardRect.draw(ColorF{ 0, 0, 0, 0 });
		lunaRect.draw(ColorF{ 0, 0, 0, 0 });
	}
private:
	int32 TitleMenuX = 0;
	int32 TitleMenuY = 0;
	int32 space = 30;
	Rect easyRect;
	Rect normalRect;
	Rect hardRect;
	Rect lunaRect;
};
class ScenarioMenu : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	ScenarioMenu(const InitData& init)
		: IScene{ init }
	{
		for (const auto& filePath : FileSystem::DirectoryContents(PATHBASE + PATH_DEFAULT_GAME + U"/070_Scenario/InfoScenario/"))
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
				cs.btnRectF = {};
				arrayClassScenario.push_back(std::move(cs));
			}
		}

		for (const auto& filePath : FileSystem::DirectoryContents(PATHBASE + PATH_DEFAULT_GAME + U"/000_SystemImage/005_ScenarioBackImage/"))
		{
			String filename = FileSystem::FileName(filePath);
			if (filename = U"pre0.jpg")
			{
				TextureAsset::Register(filename, filePath);
			}
		}

		vbar001.emplace(SasaGUI::Orientation::Vertical);
		vbar002.emplace(SasaGUI::Orientation::Vertical);

		int32 Scenario1YStart = 64;
		Scenario2X = WINDOWSIZEWIDTH000 / 2;
		Scenario2Y = WINDOWSIZEHEIGHT000 / 2;
		ScenarioTextX = WINDOWSIZEWIDTH000 / 2;
		ScenarioTextY = 80;
		auto max_it = std::max_element(arrayClassScenario.begin(), arrayClassScenario.end(),
									   [](const ClassScenario& a, const ClassScenario& b) {
										   return a.ScenarioName.size() < b.ScenarioName.size();
									   });
		if (max_it != arrayClassScenario.end())
		{
			ClassScenario& scenario_with_longest_name = *max_it;
			RectF re = getData().fontMini(scenario_with_longest_name.ScenarioName).region();
			re.w = re.w + (32 * 2);
			re.h = WINDOWSIZEHEIGHT000 / 2 - Scenario1YStart;
			re.x = 64;
			re.y = Scenario1YStart;
			scenario_with_longest_name_x = (int32)re.x;
			scenario_with_longest_name_y = (int32)re.y;
			scenario_with_longest_name_w = (int32)re.w;
			scenario_with_longest_name_h = (int32)re.h;

			rectFScenario1X = re.asRect();
			rectFScenario2X = { (int32)re.x ,(int32)re.y + (int32)re.h + 32,(int32)re.w ,(int32)re.h - 32 };

			vbar001.value().updateLayout({
				(int32)(re.x + re.w + SasaGUI::ScrollBar::Thickness), (int32)re.y,
				SasaGUI::ScrollBar::Thickness,
				(int32)re.h
			});
			vbar001.value().updateConstraints(0.0, 2000.0, Scene::Height());

			vbar002.value().updateLayout({
				(int32)(re.x + re.w + SasaGUI::ScrollBar::Thickness), (int32)Scenario2Y + 32,
				SasaGUI::ScrollBar::Thickness,
				(int32)re.h
			});
			vbar002.value().updateConstraints(0.0, 2000.0, Scene::Height());

			rectFScenarioTextX = Rect{ (int32)(re.x + re.w + SasaGUI::ScrollBar::Thickness) + 32,(int32)re.y ,ScenarioTextX - 100,770 };
		}

		Scenario1 = RenderTexture{ Size{ scenario_with_longest_name_w, scenario_with_longest_name_h }, ColorF{ 0.5, 0.0 } };
		RenderWrite1();

		Scenario2 = RenderTexture{ Size{ scenario_with_longest_name_w, scenario_with_longest_name_h }, ColorF{ 0.5, 0.0 } };
		RenderWrite2();

		for (auto&& e : arrayClassScenario)
		{
			getData().fontLine.preload(e.Text);
		}
	}
	// 更新関数（オプション）
	void update() override
	{
		RenderWrite1();
		RenderWrite2();

		for (auto&& e : arrayClassScenario)
		{
			if (e.ButtonType == U"Scenario")
			{
				{
					const Transformer2D transformer{ Mat3x2::Identity(), Mat3x2::Translate(scenario_with_longest_name_x, scenario_with_longest_name_y) };

					if (e.btnRectF.mouseOver())
					{
						tempText = ClassStaticCommonMethod::MoldingScenarioText(e.Text);
					}

					if (e.btnRectF.leftClicked() == true)
					{
						getData().selectClassScenario = e;
						changeScene(U"SelectChar", 0.9s);
					}
				}
			}
			else
			{
				{
					const Transformer2D transformer{ Mat3x2::Identity(), Mat3x2::Translate(scenario_with_longest_name_x, rectFScenario2X.y + 32) };

					if (e.btnRectF.mouseOver())
					{
						tempText = ClassStaticCommonMethod::MoldingScenarioText(e.Text);
					}

					if (e.btnRectF.leftClicked() == true)
					{
						if (e.ButtonType == U"Mail")
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
		TextureAsset(U"pre0.jpg").resized(WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000).draw(Arg::center = Scene::Center());

		getData().slice9.draw(rectFScenario1X);
		getData().slice9.draw(rectFScenario2X);
		getData().slice9.draw(rectFScenarioTextX);
		getData().fontLine(tempText).draw(rectFScenarioTextX.stretched(-32), ColorF{ 0.85 });

		Scenario1.draw(scenario_with_longest_name_x, scenario_with_longest_name_y);
		Scenario2.draw(scenario_with_longest_name_x, rectFScenario2X.y + 32);

		vbar001.value().draw();
		vbar002.value().draw();
	}
	void RenderWrite1()
	{
		int32 counterScenario1 = 0;
		{
			const ScopedRenderTarget2D target{ Scenario1.clear(ColorF{0.5, 0.0}) };

			// 描画された最大のアルファ成分を保持するブレンドステート
			const ScopedRenderStates2D blend{ MakeBlendState() };

			int32 counterScenario1 = 0;
			for (auto&& e : arrayClassScenario | std::views::filter([](auto&& e) { return e.SortKey < 0; }))
			{
				RectF re = getData().fontMini(e.ScenarioName).region();
				re.h = re.h + 16 * 2;

				int32 yyy = 32 + (counterScenario1 * re.h) + (counterScenario1 * Scenario1YBetween) - vbar001.value().value();

				re.w = scenario_with_longest_name_w - 32;
				re.x = 16;
				re.y = yyy;

				e.btnRectF = re;

				getData().slice9.draw(e.btnRectF.asRect());
				getData().fontMini(e.ScenarioName).drawAt(e.btnRectF.center(), ColorF{ 0.85 });

				counterScenario1++;
			}
		}

	}
	void RenderWrite2()
	{
		{
			const ScopedRenderTarget2D target{ Scenario2.clear(ColorF{0.5, 0.0}) };

			// 描画された最大のアルファ成分を保持するブレンドステート
			const ScopedRenderStates2D blend{ MakeBlendState() };

			int32 counterScenario2 = 0;

			for (auto&& e : arrayClassScenario | std::views::filter([](auto&& e) { return e.SortKey >= 0; }))
			{
				RectF re = getData().fontMini(e.ScenarioName).region();
				re.h = re.h + 16 * 2;

				int32 yyy = 32 + (counterScenario2 * re.h) + (counterScenario2 * Scenario1YBetween) - vbar002.value().value();

				re.w = scenario_with_longest_name_w - 32;
				re.x = 16;
				re.y = yyy;

				e.btnRectF = re;

				getData().slice9.draw(e.btnRectF.asRect());
				getData().fontMini(e.ScenarioName).drawAt(e.btnRectF.center(), ColorF{ 0.85 });

				counterScenario2++;
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
	Optional<SasaGUI::ScrollBar> vbar001;
	Optional<SasaGUI::ScrollBar> vbar002;
	String tempText = U"";
	int32 Scenario1YBetween = 32;
	int32 Scenario1X = 0;
	int32 Scenario2X;
	int32 Scenario2Y;
	int32 ScenarioTextX;
	int32 ScenarioTextY;
	Rect rectFScenario1X = {};
	Rect rectFScenario2X = {};
	Rect rectFScenarioTextX = {};
	Array <ClassScenario> arrayClassScenario;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
	RenderTexture Scenario1 = {};
	RenderTexture Scenario2 = {};
	int32 scenario_with_longest_name_x = 0;
	int32 scenario_with_longest_name_y = 0;
	int32 scenario_with_longest_name_w = 0;
	int32 scenario_with_longest_name_h = 0;
};


void Main()
{
	// ウィンドウの枠を非表示にする
	Window::SetStyle(WindowStyle::Frameless);

	//中央に配置
	Window::Centering();

	// 背景の色を設定する | Set the background color
	Scene::SetBackground(ColorF(U"#0F040D"));

	Size tempSize = { INIT_WINDOW_SIZE_WIDTH,INIT_WINDOW_SIZE_HEIGHT };
	Window::Resize(tempSize);

	App manager;
	manager.add<TitleScene>(U"TitleScene");
	manager.add<WinSizeScene>(U"WinSizeScene");
	manager.add<SelectLang>(U"SelectLang");
	manager.add<ScenarioMenu>(U"ScenarioMenu");
	manager.init(U"SelectLang");

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

		if (not manager.update())
		{
			break;
		}

		m_gaussianClass->Show();

		//タイトル表示
		manager.get().get()->fontLine(systemString.AppTitle).draw(5, Scene::Size().y - 30, Palette::Black);

		if (EXITBTNRECT.leftClicked() == true)
		{
			break;
		}

		DragProcess(dragStart);
	}

	m_gaussianClass.reset();

}
