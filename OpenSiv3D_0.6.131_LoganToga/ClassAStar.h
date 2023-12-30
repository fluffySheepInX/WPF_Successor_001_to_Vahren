# include "ClassMapBattle.h" 
# include "ClassHorizontalUnit.h" 
# include "ClassGameStatus.h" 

#pragma once
enum class AStarStatus {
	None
	,
	Open
	,
	Closed
};

int HeuristicMethod(int nowX, int nowY, int targetX, int targetY) {
	int x = std::abs(nowX - targetX);
	int y = std::abs(nowY - targetY);

	return (x > y) ? x : y;
}

class ClassAStar {
public:
	ClassAStar(int row, int col) : row(row), col(col) {}

	// AStarStatus のゲッターとセッター
	AStarStatus GetAStarStatus() const {
		return aStarStatus;
	}

	void SetAStarStatus(AStarStatus status) {
		aStarStatus = status;
	}

	// Row のゲッターとセッター
	int GetRow() const {
		return row;
	}

	void SetRow(int value) {
		row = value;
	}

	// Col のゲッターとセッター
	int GetCol() const {
		return col;
	}

	void SetCol(int value) {
		col = value;
	}

	// Cost のゲッターとセッター
	int GetCost() const {
		return cost;
	}

	void SetCost(int value) {
		cost = value;
	}

	// HCost のゲッターとセッター
	int GetHCost() const {
		return hCost;
	}

	void SetHCost(int value) {
		hCost = value;
	}

	// RefClassAStar のゲッターとセッター
	ClassAStar* GetRefClassAStar() const {
		return refClassAStar;
	}

	void SetRefClassAStar(ClassAStar* value) {
		refClassAStar = value;
	}

	// GetRoot メソッド
	void GetRoot(Array<Point>& target) {
		target.push_back(Point(row, col));
		if (refClassAStar != nullptr) {
			refClassAStar->GetRoot(target);
		}
	}

private:
	AStarStatus aStarStatus = AStarStatus::None;
	int row;
	int col;
	int cost = 0;
	int hCost = 0;
	ClassAStar* refClassAStar = nullptr;
};

class ClassAStarManager {
public:
	ClassAStarManager(int x, int y)
		: endX(x), endY(y), listClassAStar(Array<ClassAStar*>()) {}

	// Pool のゲッターとセッター
	const HashTable<String, ClassAStar>& GetPool() const {
		return pool;
	}

	void SetPool(const HashTable<String, ClassAStar>& value) {
		pool = value;
	}

	// ListClassAStar のゲッターとセッター
	Array<ClassAStar*>& GetListClassAStar() {
		return listClassAStar;
	}

	void SetListClassAStar(const Array<ClassAStar*>& value) {
		listClassAStar = value;
	}

	// EndX, EndY のゲッターとセッター
	int GetEndX() const {
		return endX;
	}

	void SetEndX(int value) {
		endX = value;
	}

	int GetEndY() const {
		return endY;
	}

	void SetEndY(int value) {
		endY = value;
	}

	ClassAStar CreateClassAStar(int x, int y) {
		ClassAStar re(x, y);
		re.SetHCost(HeuristicMethod(x, y, GetEndX(), GetEndY()));
		return re;
	}

	void RemoveClassAStar(const ClassAStar* classAStar) {
		// ポインタの配列から特定のオブジェクトへのポインタを削除
		listClassAStar.remove_if([&classAStar](const ClassAStar* item) {
			return item == classAStar;
		});
	}

	Optional<ClassAStar*> OpenOne(int x, int y, int cost, ClassAStar* parent) {
		if (x < 0 || y < 0) {
			return none;
		}

		String key = Format(x, U"/", y);
		if (auto it = pool.find(key); it != pool.end()) {
			return &(it->second);
		}

		ClassAStar* getClassAStar = new ClassAStar(CreateClassAStar(x, y));
		getClassAStar->SetAStarStatus(AStarStatus::Open);
		getClassAStar->SetCost(cost);
		getClassAStar->SetRefClassAStar(parent);

		if (parent != nullptr)
		{
			listClassAStar.push_back(getClassAStar);
		}
		pool.emplace(key, *getClassAStar);

		return getClassAStar;
	}

	void OpenAround(ClassAStar* parent,Array<Array<MapDetail>> mapData, Array<ClassHorizontalUnit> listClassHorizontalUnits, ClassGameStatus classGameStatus)
	{
		int32 x = parent->GetRow();
		int32 y = parent->GetCol();
		int32 cost = parent->GetCost();
		cost += 1;
		for (int j = -1; j < 2; j++)
		{
			for (int i = -1; i < 2; i++)
			{
				if (x + i < 0 || y + j < 0 || x + i >= mapData.size() || y + j >= mapData[x + i].size())
				{
					continue;
				}

				// GATE系の壊せるオブジェクトが存在するかチェック
				ClassUnit classUnitBuilding;
				classUnitBuilding.IsBuilding = false;
				for (auto bbb: listClassHorizontalUnits)
				{
					if (bbb.FlagBuilding == false)
					{
						continue;
					}
					for (auto aaa: bbb.ListClassUnit)
					{

						if (aaa.rowBuilding == x + i && aaa.colBuilding == y + j)
						{
							classUnitBuilding = aaa;
							classUnitBuilding.IsBuilding = true;
							classUnitBuilding.IsBuildingEnable = true;
							break;
						}
					}
				}

				//オブジェクトがあったらコンティニュー
				if (classUnitBuilding.IsBuilding == true)
				{
					switch (classUnitBuilding.mapTipObjectType)
					{
					case MapTipObjectType::WALL2:
						//ここに来ることは無い
						continue;
					case MapTipObjectType::GATE:
						if (classUnitBuilding.IsBuildingEnable == false)
						{
							OpenOne(x + i, y + j, cost, parent);
						}
						break;
					default:
						break;
					}
					continue;
				}

				if (mapData[x + i][y + j].building.size() <= 0)
				{
					OpenOne(x + i, y + j, cost, parent);
					continue;
				}

				// WALL2系の壊せないオブジェクトが存在するかチェック
				auto ob = std::find_if(classGameStatus.arrayClassObjectMapTip.begin(), classGameStatus.arrayClassObjectMapTip.end(),
									   [&](const auto& obj) {
											   return obj.nameTag == mapData[x + i][y + j].building[0].begin()->first;
									   });
				if (ob != classGameStatus.arrayClassObjectMapTip.end()) {
					switch (ob->type) {
					case MapTipObjectType::WALL2:
						break;
					case MapTipObjectType::GATE:
						OpenOne(x + i, y + j, cost, parent);
						break;
					default:
						break;
					}
				}
				else {
					OpenOne(x + i, y + j, cost, parent);
				}
			}
		}
	}

private:
	HashTable<String, ClassAStar> pool;
	Array<ClassAStar*> listClassAStar;
	int endX;
	int endY;
};
