#pragma once
class PauseWindow {
public:
	void draw(Font gd) const {
		// ウィンドウの矩形を定義
		const Rect window(Arg::center = Scene::Center(), 300, 200);
		window.draw(Palette::Darkgray); // ウィンドウ背景を描画
		gd(U"Paused").drawAt(window.center()); // ウィンドウ中央に「Paused」のテキストを描画

		// OKボタン
		const Rect okButton(window.x + 50, window.y + 120, 200, 40);
		if (okButton.leftClicked()) {
			// モーダルウィンドウを閉じる処理を記述
			// ここにモーダルウィンドウを閉じるロジックを追加する
		}
		okButton.draw(Palette::Skyblue); // OKボタンの背景を描画
		gd(U"OK").drawAt(okButton.center()); // OKボタンのテキストを描画
	}
};
