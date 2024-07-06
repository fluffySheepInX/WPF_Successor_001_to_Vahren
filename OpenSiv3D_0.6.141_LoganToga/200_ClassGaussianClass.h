#pragma once
class GaussianClass
{
public:
	GaussianClass()
	{
	}
	GaussianClass(const Point& size, const Array<std::function<void()>>& draws)
	{
		sceneSize = size;
		functions = draws;
		gaussianA1 = RenderTexture{ sceneSize };
		gaussianB1 = RenderTexture{ sceneSize };
		gaussianA4 = RenderTexture{ sceneSize / 4 };
		gaussianB4 = RenderTexture{ sceneSize / 4 };
		gaussianA8 = RenderTexture{ sceneSize / 8 };
		gaussianB8 = RenderTexture{ sceneSize / 8 };
	}
	void Show() {
		show();
	}
	void SetSize(const Point& size)
	{
		sceneSize = size;
		gaussianA1 = RenderTexture{ sceneSize };
		gaussianB1 = RenderTexture{ sceneSize };
		gaussianA4 = RenderTexture{ sceneSize / 4 };
		gaussianB4 = RenderTexture{ sceneSize / 4 };
		gaussianA8 = RenderTexture{ sceneSize / 8 };
		gaussianB8 = RenderTexture{ sceneSize / 8 };
	}
private:
	// 関数を格納するArrayを定義する
	Array<std::function<void()>> functions;
	//ガウスぼかし
	Size sceneSize = { 0,0 };
	RenderTexture gaussianA1 = {}, gaussianB1 = {};
	RenderTexture gaussianA4 = {}, gaussianB4 = {};
	RenderTexture gaussianA8 = {}, gaussianB8 = {};
	double a1 = 0.0, a4 = 0.0, a8 = 0.5;

	void show() {
		process();
	}

	void process()
	{
		for (auto aaa : functions)
		{
			aaa();
		}
		// ガウスぼかし用テクスチャにもう一度シーンを描く
		{
			const ScopedRenderTarget2D target{ gaussianA1.clear(ColorF{ 0.0 }) };
			const ScopedRenderStates2D blend{ BlendState::Additive };
			for (auto aaa : functions)
			{
				aaa();
			}
		}

		// オリジナルサイズのガウスぼかし (A1)
		// A1 を 1/4 サイズにしてガウスぼかし (A4)
		// A4 を 1/2 サイズにしてガウスぼかし (A8)
		Shader::GaussianBlur(gaussianA1, gaussianB1, gaussianA1);
		Shader::Downsample(gaussianA1, gaussianA4);
		Shader::GaussianBlur(gaussianA4, gaussianB4, gaussianA4);
		Shader::Downsample(gaussianA4, gaussianA8);
		Shader::GaussianBlur(gaussianA8, gaussianB8, gaussianA8);

		{
			const ScopedRenderStates2D blend{ BlendState::Additive };

			if (a1)
			{
				gaussianA1.resized(sceneSize).draw(ColorF{ a1 });
			}

			if (a4)
			{
				gaussianA4.resized(sceneSize).draw(ColorF{ a4 });
			}

			if (a8)
			{
				gaussianA8.resized(sceneSize).draw(ColorF{ a8 });
			}
		}
	}
};
