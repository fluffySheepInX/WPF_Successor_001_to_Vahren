# include <Siv3D.hpp> // Siv3D v0.6.14
# include "000_SystemString.h"
# include "001_GLOBAL.h"
# include "005_GameUIToolkit.h"
# include "100_StructEffect.h"
# include "101_StructHomography.h"
# include "102_StructGameData.h"
# include "150_EnumClassLanguageSteamFullSuport.h"
# include "151_EnumSelectCharStatus.h"
# include "158_EnumFormBuyDisplayStatus.h"
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
	Rect{ 0, 0, Scene::Size().x , 1 }.draw(Palette::Yellow);
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
	if (Rect{ 0, 0, WINDOWSIZEWIDTH000 / 2, 60 }.mouseOver() == true)
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
	if (Rect{ 0, 0, WINDOWSIZEWIDTH000 / 2, 60 }.leftClicked())
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

			csl.btnRectF = RectF{ 100 / 2,(300) + (counterAll * (rectText.h + 40)) ,WINDOWSIZEWIDTH000 / 2 - 100,rectText.h + 40 };
			//csl.btnRectF = RectF{ 100 / 2,(300) + (counterAll * (rectText.h + 20)) ,WINDOWSIZEWIDTH000 / 2 - 100,rectText.h + 20 };

			acsl.push_back(csl);
			counterCol++;
			counterAll++;
		}

		vbar001.emplace(SasaGUI::Orientation::Vertical);

		//仮置き
		language = LanguageSteamFullSuport::English;


		EXITBTNPOLYGON = Shape2D::Cross(10, 5, Vec2{ INIT_WINDOW_SIZE_WIDTH - 10, INIT_WINDOW_SIZE_HEIGHT - 10 }).asPolygon();
		EXITBTNRECT = Rect{ Arg::center(EXITBTNPOLYGON.centroid().asPoint()),20,20 };
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
				ss.configSave = value[U"configSave"].get<String>();
				ss.configLoad = value[U"configLoad"].get<String>();
				ss.selectScenario = value[U"selectScenario"].get<String>();
				ss.selectScenario2 = value[U"selectScenario2"].get<String>();
				ss.selectChara1 = value[U"selectChara1"].get<String>();
				ss.selectCard = value[U"selectCard"].get<String>();
				ss.DoYouWantToQuitTheGame = value[U"DoYouWantToQuitTheGame"].get<String>();
				ss.strategyMenu.push_back(value[U"strategyMenu000"].get<String>());
				ss.strategyMenu.push_back(value[U"strategyMenu001"].get<String>());
				ss.strategyMenu.push_back(value[U"strategyMenu002"].get<String>());
				ss.strategyMenu.push_back(value[U"strategyMenu003"].get<String>());
				ss.strategyMenu.push_back(value[U"strategyMenu004"].get<String>());
				ss.strategyMenu.push_back(value[U"strategyMenu005"].get<String>());
				ss.strategyMenu.push_back(value[U"strategyMenu006"].get<String>());
				ss.strategyMenu.push_back(value[U"strategyMenu007"].get<String>());
				ss.strategyMenu.push_back(value[U"strategyMenu008"].get<String>());
				ss.strategyMenu.push_back(value[U"strategyMenu009"].get<String>());
				//ss.strategyMenu000 = value[U"strategyMenu000"].get<String>();
				//ss.strategyMenu001 = value[U"strategyMenu001"].get<String>();
				//ss.strategyMenu002 = value[U"strategyMenu002"].get<String>();
				//ss.strategyMenu003 = value[U"strategyMenu003"].get<String>();
				//ss.strategyMenu004 = value[U"strategyMenu004"].get<String>();
				//ss.strategyMenu005 = value[U"strategyMenu005"].get<String>();
				//ss.strategyMenu006 = value[U"strategyMenu006"].get<String>();
				//ss.strategyMenu007 = value[U"strategyMenu007"].get<String>();
				//ss.strategyMenu008 = value[U"strategyMenu008"].get<String>();
				//ss.strategyMenu009 = value[U"strategyMenu009"].get<String>();
				ss.BattleMessage001 = value[U"BattleMessage001"].get<String>();
				ss.BuyMessage001 = value[U"BuyMessage001"].get<String>();
				ss.SelectCharMessage001 = value[U"SelectCharMessage001"].get<String>();
				ss.StorySkip = value[U"StorySkip"].get<String>();
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

		EXITBTNPOLYGON = Shape2D::Cross(10, 5, Vec2{ tempSize.x - 10, tempSize.y - 10 }).asPolygon();
		EXITBTNRECT = Rect{ Arg::center(EXITBTNPOLYGON.centroid().asPoint()),20,20 };

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

		EXITBTNPOLYGON = Shape2D::Cross(10, 5, Vec2{ WINDOWSIZEWIDTH000 - 10, Scene::Size().y - 10 }).asPolygon();
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
class SelectChar : public App::Scene
{
public:
	// コンストラクタ（必ず実装）
	SelectChar(const InitData& init)
		: IScene{ init }
	{
		for (const auto& filePath : FileSystem::DirectoryContents(PATHBASE + PATH_DEFAULT_GAME + U"/030_SelectCharaImage/"))
		{
			String filename = FileSystem::FileName(filePath);
			TextureAsset::Register(filename, filePath);
		}

		String sc = systemString.selectChara1;
		RectF re1 = getData().fontNormal(sc).region();
		re1.x = WINDOWSIZEWIDTH000 / 2 - (re1.w / 2);
		re1.y = basePointY;
		arrayRectFSystem.push_back(re1);
		Rect re2 = { 0,0,WINDOWSIZEWIDTH000,256 };
		re2.x = WINDOWSIZEWIDTH000 / 2 - (800);
		re2.y = (re1.h + basePointY + 20) + 450 + 20;
		arrayRectSystem.push_back(re2);

		int32 arrayPowerSize = getData().selectClassScenario.ArrayPower.size();
		int32 xxx = 0;
		xxx = ((arrayPowerSize * 169) / 2);
		int32 counter = 0;
		for (auto ttt : getData().selectClassScenario.ArrayPower)
		{
			for (auto&& e : getData().classGameStatus.arrayClassPower | std::views::filter([&](auto&& e) { return e.PowerTag == ttt; }))
			{
				RectF rrr = {};
				rrr = { (WINDOWSIZEWIDTH000 / 2 - xxx) + counter * 169,re1.h + basePointY + 20,169,450 };
				e.RectF = rrr;
			}
			counter++;
		}

		//Scene::SetBackground(Color{ 126,87,194,255 });
	}
	// 更新関数（オプション）
	void update() override
	{
		switch (selectCharStatus)
		{
		case SelectCharStatus::SelectChar:
		{
			for (const auto ttt : getData().classGameStatus.arrayClassPower)
			{
				if (ttt.RectF.leftClicked() == true)
				{
					// TOML ファイルからデータを読み込む
					const TOMLReader tomlInfoProcess{ PATHBASE + PATH_DEFAULT_GAME + U"/070_Scenario/InfoProcess/" + ttt.PowerTag + U".toml" };

					if (not tomlInfoProcess) // もし読み込みに失敗したら
					{
						selectCharStatus = SelectCharStatus::Message;
						Message001 = true;
						break;
					}

					for (const auto& table : tomlInfoProcess[U"Process"].tableArrayView()) {
						String map = table[U"map"].get<String>();
						getData().classGameStatus.arrayInfoProcessSelectCharaMap = map.split(U',');
						//for (auto& map : getData().classGameStatus.arrayInfoProcessSelectCharaMap)
						//{
						//	String ene = table[map].get<String>();
						//	getData().classGameStatus.arrayInfoProcessSelectCharaEnemyUnit = ene.split(U',');
						//}
					}
					getData().classGameStatus.nowPowerTag = ttt.PowerTag;
					getData().NovelPower = ttt.PowerTag;
					getData().NovelNumber = 0;

					for (auto& aaa : getData().classGameStatus.arrayClassPower)
					{
						if (aaa.PowerTag == ttt.PowerTag)
						{
							getData().selectClassPower = aaa;
							getData().Money = aaa.Money;
						}
					}

					changeScene(U"Novel", 0.9s);
				}
			}
		}
		break;
		case SelectCharStatus::Message:
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
						selectCharStatus = SelectCharStatus::SelectChar;
					}
				}
			}
		}
		break;
		case SelectCharStatus::Event:
			break;
		default:
			break;
		}
	}
	// 描画関数（オプション）
	void draw() const override
	{
		TextureAsset(getData().selectClassScenario.SelectCharaFrameImageLeft).draw();
		TextureAsset(getData().selectClassScenario.SelectCharaFrameImageRight)
			.draw(WINDOWSIZEWIDTH000 - TextureAsset(getData().selectClassScenario.SelectCharaFrameImageRight).width(), 0);

		arrayRectFSystem[0].draw();
		getData().fontMini(systemString.selectChara1).draw(arrayRectFSystem[0], ColorF{ 0.25 });
		arrayRectFSystem[0].drawFrame(3, 0, Palette::Orange);

		getData().slice9.draw(arrayRectSystem[0]);

		for (const auto ttt : getData().classGameStatus.arrayClassPower)
		{
			ttt.RectF(TextureAsset(ttt.Image).resized(169, 450)).draw();
			if (ttt.RectF.mouseOver() == true)
			{
				getData().fontLine(ttt.Text).draw(arrayRectSystem[0].stretched(-10), ColorF{ 0.85 });
			}
		}

		switch (selectCharStatus)
		{
		case SelectCharStatus::SelectChar:
			break;
		case SelectCharStatus::Message:
			sceneMessageBoxImpl.show(systemString.SelectCharMessage001);
			break;
		case SelectCharStatus::Event:
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
	int32 basePointY = 50;
	/// @brief classConfigString.selectChara1の枠　など
	Array<Rect> arrayRectSystem;
	/// @brief mouseOver時のテキストエリア　など
	Array<RectF> arrayRectFSystem;
	bool Message001 = false;
	SelectCharStatus selectCharStatus = SelectCharStatus::SelectChar;
	s3dx::SceneMessageBoxImpl sceneMessageBoxImpl;
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
		String path = PATHBASE + PATH_DEFAULT_GAME + U"/070_Scenario/InfoStory/" + np + U"+" + Format(nn) + U".csv";
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

		rectText = { 50,WINDOWSIZEHEIGHT000 - (256 + 32),WINDOWSIZEWIDTH000 - 100,256 };
		rectHelp = { 70,WINDOWSIZEHEIGHT000 - 325 - 70,400,70 };
		rectFace = { WINDOWSIZEWIDTH000 - 100 - 206 - 50,WINDOWSIZEHEIGHT000 - 325 + 50,206,206 };
		rectSkip = { WINDOWSIZEWIDTH000 - 400 - 50,WINDOWSIZEHEIGHT000 - 325 - 70,400,70 };
		stopwatch = Stopwatch{ StartImmediately::Yes };
	}
	// 更新関数（オプション）
	void update() override
	{
		if (csv[nowRow][7].length() != length)
		{
			length = stopwatch.sF() / 0.05;
		}
		if (csv[nowRow][0].substr(0, 3) == U"end" || rectSkip.leftClicked() == true)
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

		if (MouseL.down() == true && rectText.mouseOver() == true)
		{
			if (csv[nowRow][7].length() == length)
			{
				stopwatch.restart();
				length = 0;
				nowRow++;
			}
			else
			{
				length = csv[nowRow][7].length();
			}
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
			TextureAsset(csv[nowRow][4]).resized(WINDOWSIZEWIDTH000, WINDOWSIZEHEIGHT000).drawAt(Scene::Center());
		}

		getData().slice9.draw(rectText);
		getData().slice9.draw(rectSkip);
		getData().fontLine(systemString.StorySkip).draw(rectSkip.stretched(-10), ColorF{ 0.85 });

		if (csv[nowRow][3] != U"-1")
		{
			rectFace(TextureAsset(csv[nowRow][3])).draw();
		}
		if (csv[nowRow][0].c_str()[0] != '#')
		{
			getData().fontLine(csv[nowRow][7].substr(0, length)).draw(rectText.stretched(-10), ColorF{ 0.85 });
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
			getData().fontLine(he).draw(rectHelp.stretched(-10), ColorF{ 0.85 });
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
	Rect rectText = {};
	Rect rectFace = {};
	Rect rectHelp = {};
	Rect rectSkip = {};
	int32 nowRow = 0;
	int32 length = 0;
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
		for (auto index : Range(0, getData().classGameStatus.NumMenus - 1)) {
			if (getData().classGameStatus.strategyMenus[index]) {
				htMenuBtn.push_back(std::make_tuple(index, Rect{ 32,32 + (index * 64),300,64 }));
			}
		}

		//初期化
		{
			for (auto ttt : htMenuBtnDisplay)
				ttt.second = false;

			int32 counter = 0;
			for (auto& ttt : getData().classGameStatus.arrayClassUnit)
			{
				ttt.rectExecuteBtnStrategyMenu = Rect{ 448,548 + (counter * 64),300,64 };
				counter++;
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
				if (std::get<1>(ttt).mouseOver())
				{
					for (auto&& [i, re] : htMenuBtnDisplay)
						htMenuBtnDisplay[i] = false;
					htMenuBtnDisplay[std::get<0>(ttt)] = true;
				}
				switch (std::get<0>(ttt))
				{
				case 9:
				{
					if (std::get<1>(ttt).leftClicked() == true)
					{
						//バトル前準備
						processBeforeBattle();
						changeScene(U"Battle", 0.9s);
					}
				}
				break;
				default:
					break;
				}
			}

			//trueなら表示
			if (std::any_of(htMenuBtnDisplay.begin(), htMenuBtnDisplay.end(),
				[](const auto& ttt) { return ttt.second; }))
			{
				rectExecuteBtn = Rect{ 432, 516, 500, 500 };
			}

			//徴兵処理
			conscriptionUnit();
			//ユニット表示エリアでスクロールした時、位置を調整する
			fixDisplayUnit();
			//ユニットをクリック時に、その他のユニットの対象フラグを初期化する
			resetFlagsUnit();
			//スクロールバー関係
			processBar();
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

		for (auto&& [i, ttt] : Indexed(htMenuBtn))
		{
			getData().slice9.draw(std::get<1>(ttt));
			getData().fontLine(systemString.strategyMenu[i]).draw(std::get<1>(ttt).stretched(-10));
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
						getData().fontLine(nowHtRectPlusUnit.Name).draw(nowHtRectPlusUnit.rectExecuteBtnStrategyMenu.stretched(-10));
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
			sceneMessageBoxImpl.show(systemString.BuyMessage001);
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
	/// @brief ユニットをクリック時に、その他のユニットの対象フラグを初期化する
	void resetFlagsUnit()
	{
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
					break;
				}
			}
		}
	}
	/// @brief スクロールバー関係
	void processBar()
	{
		if (arrayRectMenuBack[1].mouseOver() == true)
			vbar001.value().scroll(Mouse::Wheel() * 60);
		if (rectExecuteBtn.mouseOver() == true)
			vbar002.value().scroll(Mouse::Wheel() * 60);
		vbar001.value().update();
		vbar002.value().update();
	}
	/// @brief ユニット表示エリアでスクロールした時、位置を調整する
	void fixDisplayUnit()
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
	/// @brief 徴兵処理
	void conscriptionUnit()
	{
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
	}
	/// @brief バトル前準備
	void processBeforeBattle()
	{
		String targetMap = getData().classGameStatus.arrayInfoProcessSelectCharaMap[getData().Wave];

		const TOMLReader tomlMap{ PATHBASE + PATH_DEFAULT_GAME + U"/016_BattleMap/" + targetMap };
		if (not tomlMap) // もし読み込みに失敗したら
			throw Error{ U"Failed to load `tomlMap`" };

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
		const TOMLReader tomlInfoProcess{ PATHBASE + PATH_DEFAULT_GAME + U"/070_Scenario/InfoProcess/" + getData().classGameStatus.nowPowerTag + U".toml" };
		if (not tomlInfoProcess) // もし読み込みに失敗したら
			throw Error{ U"Failed to load `tomlInfoProcess`" };

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

	}
	/// @brief 左上メニュー、画面上部ユニット群の強制表示枠
	Array <Rect> arrayRectMenuBack;
	/// @brief 画面下部の詳細実行枠
	Rect rectExecuteBtn{ 0,0,0,0 };
	Array<std::tuple<int32, Rect>> htMenuBtn;
	HashTable<int32, bool> htMenuBtnDisplay;
	std::unique_ptr<IFade> m_fadeInFunction = randomFade();
	std::unique_ptr<IFade> m_fadeOutFunction = randomFade();
	Optional<SasaGUI::ScrollBar> vbar001;
	Optional<SasaGUI::ScrollBar> vbar002;
	bool Message001 = false;
	FormBuyDisplayStatus formBuyDisplayStatus = FormBuyDisplayStatus::Normal;
	s3dx::SceneMessageBoxImpl sceneMessageBoxImpl;

};


void Init(App& manager)
{
	for (const auto& filePath : FileSystem::DirectoryContents(PATHBASE + PATH_DEFAULT_GAME + U"/010_FaceImage/"))
	{
		String filename = FileSystem::FileName(filePath);
		TextureAsset::Register(filename, filePath);
	}
	for (const auto& filePath : FileSystem::DirectoryContents(PATHBASE + PATH_DEFAULT_GAME + U"/006_CardImage/"))
	{
		String filename = FileSystem::FileName(filePath);
		TextureAsset::Register(filename, filePath);
	}
	for (const auto& filePath : FileSystem::DirectoryContents(PATHBASE + PATH_DEFAULT_GAME + U"/005_BackgroundImage/"))
	{
		String filename = FileSystem::FileName(filePath);
		TextureAsset::Register(filename, filePath);
	}
	for (const auto& filePath : FileSystem::DirectoryContents(PATHBASE + PATH_DEFAULT_GAME + U"/040_ChipImage/"))
	{
		String filename = FileSystem::FileName(filePath);
		TextureAsset::Register(filename, filePath);
	}
	for (const auto& filePath : FileSystem::DirectoryContents(PATHBASE + PATH_DEFAULT_GAME + U"/070_Scenario/InfoPower/"))
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
	// Unit.jsonからデータを読み込む
	{
		const JSON jsonUnit = JSON::Load(PATHBASE + PATH_DEFAULT_GAME + U"/070_Scenario/InfoUnit/Unit.json");

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
			cu.Defense = Parse<int32>(value[U"defense"].getString());
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
		const JSON skillData = JSON::Load(PATHBASE + PATH_DEFAULT_GAME + +U"/070_Scenario/InfoSkill/skill.json");

		if (not skillData) // もし読み込みに失敗したら
		{
			throw Error{ U"Failed to load `skill.json`" };
		}

		Array<ClassSkill> arrayClassSkill;
		for (const auto& [key, value] : skillData[U"Skill"]) {
			ClassSkill cu;
			{
				if (value[U"func"].get<String>() == U"missile")
				{
					cu.SkillType = SkillType::missile;
				}
			}
			{
				if (value[U"MoveType"].get<String>() == U"throw")
				{
					cu.MoveType = MoveType::thr;
				}
				if (value[U"MoveType"].get<String>() == U"circle")
				{
					cu.MoveType = MoveType::circle;
				}
				if (value[U"MoveType"].get<String>() == U"swing")
				{
					cu.MoveType = MoveType::swing;
				}
			}
			{
				if (value.hasElement(U"Easing") == true)
				{
					if (value[U"Easing"].get<String>() == U"easeOutExpo")
					{
						cu.Easing = SkillEasing::easeOutExpo;
					}
				}
			}
			if (value.hasElement(U"EasingRatio") == true)
			{
				cu.EasingRatio = Parse<int32>(value[U"EasingRatio"].get<String>());
			}
			if (value.hasElement(U"slow_per") == true)
			{
				cu.slowPer = Parse<int32>(value[U"slow_per"].get<String>());
			}
			else
			{
				cu.slowPer = none;
			}
			if (value.hasElement(U"slow_time") == true)
			{
				cu.slowTime = Parse<int32>(value[U"slow_time"].get<String>());
			}
			else
			{
				cu.slowTime = none;
			}
			if (value.hasElement(U"center") == true)
			{
				if (value[U"center"].get<String>() == U"on")
				{
					cu.SkillCenter = SkillCenter::on;
				}
				else if (value[U"center"].get<String>() == U"off")
				{
					cu.SkillCenter = SkillCenter::off;
				}
				else if (value[U"center"].get<String>() == U"end")
				{
					cu.SkillCenter = SkillCenter::end;
				}
			}
			if (value.hasElement(U"bom") == true)
			{
				if (value[U"bom"].get<String>() == U"on")
				{
					cu.SkillBomb = SkillBomb::on;
				}
				else if (value[U"bom"].get<String>() == U"off")
				{
					cu.SkillBomb = SkillBomb::off;
				}
			}
			if (value.hasElement(U"height") == true)
			{
				cu.height = Parse<int32>(value[U"height"].get<String>());
			}
			if (value.hasElement(U"radius") == true)
			{
				cu.radius = Parse<double>(value[U"radius"].get<String>());
			}
			if (value.hasElement(U"rush") == true)
			{
				cu.rush = Parse<int32>(value[U"rush"].get<String>());
			}
			cu.image = value[U"image"].get<String>();
			if (value.hasElement(U"d360") == true)
			{
				if (value[U"d360"].get<String>() == U"on")
				{
					cu.SkillD360 = SkillD360::on;
					TextureAsset::Register(cu.image + U".png", U"001_Warehouse/001_DefaultGame/042_ChipImageSkillEffect/" + cu.image + U".png");
				}
			}
			else
			{
				TextureAsset::Register(cu.image + U"NW.png", U"001_Warehouse/001_DefaultGame/042_ChipImageSkillEffect/" + cu.image + U"NW.png");
				TextureAsset::Register(cu.image + U"N.png", U"001_Warehouse/001_DefaultGame/042_ChipImageSkillEffect/" + cu.image + U"N.png");
			}
			if (value.hasElement(U"force_ray") == true)
			{
				if (value[U"force_ray"].get<String>() == U"on")
				{
					cu.SkillForceRay = SkillForceRay::on;
				}
				else
				{
					cu.SkillForceRay = SkillForceRay::off;
				}
			}
			cu.nameTag = value[U"name_tag"].get<String>();
			cu.name = value[U"name"].get<String>();
			cu.range = Parse<int32>(value[U"range"].get<String>());
			cu.w = Parse<int32>(value[U"w"].get<String>());
			cu.h = Parse<int32>(value[U"h"].get<String>());
			cu.str = Parse<int32>(value[U"str"].get<String>());
			if (value[U"str_kind"].get<String>() == U"attack")
			{
				cu.SkillStrKind = SkillStrKind::attack;
			}
			else if (value[U"str_kind"].get<String>() == U"off")
			{
				cu.SkillStrKind = SkillStrKind::attack;
			}
			else if (value[U"str_kind"].get<String>() == U"end")
			{
				cu.SkillStrKind = SkillStrKind::attack;
			}

			cu.speed = Parse<double>(value[U"speed"].get<String>());

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

}

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
	manager.add<SelectChar>(U"SelectChar");
	manager.add<Novel>(U"Novel");
	manager.add<Buy>(U"Buy");
	manager.init(U"SelectLang");

	// 関数を格納するArrayを定義する
	Array<std::function<void()>> functions;
	functions.push_back(DrawUnder000);
	functions.push_back(DrawUnder001);
	m_gaussianClass = std::make_unique<GaussianClass>(Scene::Size(), functions);

	Init(manager);

	Optional<std::pair<Point, Point>> dragStart;

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
