#pragma once
class MapCreator
{
public:
	////マップ関係
	Array<Array<double>> map;
	// 各タイルのテクスチャ
	HashTable<String, Texture> mapTextures;
	// マップの一辺のタイル数
	int32 N = 80;
	// タイルの種類
	Grid<int32> grid;
	// タイルの一辺の長さ（ピクセル）
	Vec2 TileOffset{ 50, 25 };
	// タイルの厚み（ピクセル）
	int32 TileThickness = 15;
	/// @brief タイルのインデックスから、タイルの底辺中央の座標を計算します。
	/// @param index タイルのインデックス
	/// @param N マップの一辺のタイル数
	/// @return タイルの底辺中央の座標
	Vec2 ToTileBottomCenter(const Point& index, const int32 N)
	{
		const int32 i = index.manhattanLength();
		const int32 xi = (i < (N - 1)) ? 0 : (i - (N - 1));
		const int32 yi = (i < (N - 1)) ? i : (N - 1);
		const int32 k = (index.manhattanDistanceFrom(Point{ xi, yi }) / 2);
		const double posX = ((i < (N - 1)) ? (i * -TileOffset.x) : ((i - 2 * N + 2) * TileOffset.x));
		const double posY = (i * TileOffset.y);
		return{ (posX + TileOffset.x * 2 * k), posY };
	}
	/// @brief タイルのインデックスから、タイルの四角形を計算します。
	/// @param index タイルのインデックス
	/// @param N マップの一辺のタイル数
	/// @return タイルの四角形
	Quad ToTile(const Point& index, const int32 N)
	{
		const Vec2 bottomCenter = ToTileBottomCenter(index, N);

		return Quad{
			bottomCenter.movedBy(0, -TileThickness).movedBy(0, -TileOffset.y * 2),
			bottomCenter.movedBy(0, -TileThickness).movedBy(TileOffset.x, -TileOffset.y),
			bottomCenter.movedBy(0, -TileThickness),
			bottomCenter.movedBy(0, -TileThickness).movedBy(-TileOffset.x, -TileOffset.y)
		};
	}
	/// @brief 指定した列のタイルによって構成される四角形を計算します。
	/// @param x 列インデックス
	/// @param N マップの一辺のタイル数
	/// @return 指定した列のタイルによって構成される四角形
	Quad ToColumnQuad(const int32 x, const int32 N)
	{
		return{
			ToTileBottomCenter(Point{ x, 0 }, N).movedBy(0, -TileThickness).movedBy(0, -TileOffset.y * 2),
			ToTileBottomCenter(Point{ x, 0 }, N).movedBy(0, -TileThickness).movedBy(TileOffset.x, -TileOffset.y),
			ToTileBottomCenter(Point{ x, (N - 1) }, N).movedBy(0, -TileThickness).movedBy(0, 0),
			ToTileBottomCenter(Point{ x, (N - 1) }, N).movedBy(0, -TileThickness).movedBy(-TileOffset.x, -TileOffset.y)
		};
	}
	/// @brief 指定した行のタイルによって構成される四角形を計算します。
	/// @param y 行インデックス
	/// @param N マップの一辺のタイル数
	/// @return 指定した行のタイルによって構成される四角形
	Quad ToRowQuad(const int32 y, const int32 N)
	{
		return{
			ToTileBottomCenter(Point{ 0, y }, N).movedBy(0, -TileThickness).movedBy(-TileOffset.x, -TileOffset.y),
			ToTileBottomCenter(Point{ 0, y }, N).movedBy(0, -TileThickness).movedBy(0, -TileOffset.y * 2),
			ToTileBottomCenter(Point{ (N - 1), y }, N).movedBy(0, -TileThickness).movedBy(TileOffset.x, -TileOffset.y),
			ToTileBottomCenter(Point{ (N - 1), y }, N).movedBy(0, -TileThickness).movedBy(0, 0)
		};
	}
	/// @brief 各列のタイルによって構成される四角形の配列を作成します。
	/// @param N マップの一辺のタイル数
	/// @return 各列のタイルによって構成される四角形の配列
	Array<Quad> MakeColumnQuads(const int32 N)
	{
		Array<Quad> quads;

		for (int32 x = 0; x < N; ++x)
		{
			quads << ToColumnQuad(x, N);
		}

		return quads;
	}
	/// @brief 各行のタイルによって構成される四角形の配列を作成します。
	/// @param N マップの一辺のタイル数
	/// @return 各行のタイルによって構成される四角形の配列
	Array<Quad> MakeRowQuads(const int32 N)
	{
		Array<Quad> quads;

		for (int32 y = 0; y < N; ++y)
		{
			quads << ToRowQuad(y, N);
		}

		return quads;
	}
	/// @brief 指定した座標にあるタイルのインデックスを返します。
	/// @param pos 座標
	/// @param columnQuads 各列のタイルによって構成される四角形の配列
	/// @param rowQuads 各行のタイルによって構成される四角形の配列
	/// @return タイルのインデックス。指定した座標にタイルが無い場合は none
	Optional<Point> ToIndex(const Vec2& pos, const Array<Quad>& columnQuadsToIndex, const Array<Quad>& rowQuads)
	{
		int32 x = -1, y = -1;

		// タイルの列インデックスを調べる
		for (int32 i = 0; i < columnQuadsToIndex.size(); ++i)
		{
			if (columnQuadsToIndex[i].intersects(pos))
			{
				x = i;
				break;
			}
		}

		// タイルの行インデックスを調べる
		for (int32 i = 0; i < rowQuads.size(); ++i)
		{
			if (rowQuads[i].intersects(pos))
			{
				y = i;
				break;
			}
		}

		// インデックスが -1 の場合、タイル上にはない
		if ((x == -1) || (y == -1))
		{
			return none;
		}

		return Point{ x, y };
	}
private:

};
