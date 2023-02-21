using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren._006_ClassStatic
{
    public static class ClassStaticStraregyAI
    {

        // COM勢力の手順を終わる
        public static void ThinkingEnd(ClassGameStatus classGameStatus, ClassPower classPower, MainWindow mainWindow)
        {
            //お金増やす
            {
                var listSpotMoney = classGameStatus.NowListSpot
                                .Where(x => x.PowerNameTag == classPower.NameTag);
                int countMoney = 0;
                int addMoney = 0; // 維持費と財政値による増減
                foreach (var itemSpot in listSpotMoney)
                {
                    countMoney += itemSpot.Gain;
                    foreach (var itemTroop in itemSpot.UnitGroup)
                    {
                        foreach (var itemUnit in itemTroop.ListClassUnit)
                        {
                            // 維持費の分だけ減らして、財政値の分だけ増やす
                            addMoney -= itemUnit.Cost;
                            addMoney += itemUnit.Finance;
                        }
                    }
                }
                countMoney *= (int)(classGameStatus.ClassContext.GainPer * 0.01);
                countMoney += addMoney;

                // どの勢力の処理中か分かるようにする（実験用なので後でコメントアウトすること）
                //MessageBox.Show(classPower.Name + "のお金が " + countMoney + " 増えたよ！");

                classPower.Money += countMoney;
            }
        }

        // COM勢力の戦闘処理に続ける場合は true を返すこと
        public static bool ThinkingEasy(ClassGameStatus classGameStatus, ClassPower classPower, MainWindow mainWindow)
        {
            if (classGameStatus == null) return false;

            ////状態によって隣国へ攻め入る
            ///状態をチェック
            switch (classPower.Fix)
            {
                case _010_Enum.FlagPowerFix.on:

                    {
                        ////他国との国境都市を取得
                        //自国領土を取得
                        List<ClassSpot> mySpot = new List<ClassSpot>();
                        foreach (var item in classGameStatus.NowListSpot)
                        {
                            if (item.PowerNameTag == classPower.NameTag)
                            {
                                mySpot.Add(item);
                            }
                        }

                        //自国領土と接触している他国領土のタグを取得
                        List<ClassSpot> spotOtherLand = new List<ClassSpot>();
                        foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                        {
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item1.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item2
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item2.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item1
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                        }

                        spotOtherLand = spotOtherLand.Distinct().ToList();

                        //隣接している他国の一覧を取得
                        List<ClassPower> adjacentPowers = new List<ClassPower>();
                        foreach (var item in spotOtherLand)
                        {
                            var result = classGameStatus.NowListPower
                                            .Where(x => x != classPower)
                                            .Where(x => x.NameTag == item.PowerNameTag)
                                            .FirstOrDefault();
                            if (result != null)
                            {
                                adjacentPowers.Add(result);
                            }
                        }

                        ////ランダム(補正有り)でターゲットとなる国を選ぶ
                        //友好度50のリストを作る
                        Dictionary<string, int> baseTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in adjacentPowers)
                        {
                            baseTargetPowerList.Add(item.NameTag, 50);
                        }

                        //友好度50のリストを本来のデータで上書きする
                        foreach (var itemBaseTargetPowerList in baseTargetPowerList.ToList())
                        {
                            foreach (var item in classGameStatus.ClassDiplomacy.Diplo)
                            {
                                if (itemBaseTargetPowerList.Key == item.Item1 && item.Item2 == classPower.NameTag)
                                {
                                    baseTargetPowerList[itemBaseTargetPowerList.Key] = item.Item3;
                                    continue;
                                }
                                if (itemBaseTargetPowerList.Key == item.Item2 && item.Item1 == classPower.NameTag)
                                {
                                    baseTargetPowerList[item.Item1] = item.Item3;
                                    continue;
                                }
                            }
                        }

                        //-100して絶対値を取る
                        //友好度100なら0
                        //友好度0なら100
                        Dictionary<string, int> absTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in baseTargetPowerList)
                        {
                            absTargetPowerList.Add(item.Key, Math.Abs(item.Value - 100));
                        }

                        ////ランダムで値を取得して、絶対値と比較
                        //範囲内ならターゲットの国とする
                        Random random = new Random(DateTime.Now.Second);
                        List<ClassPower> targetPowers = new List<ClassPower>();
                        foreach (var item in absTargetPowerList)
                        {
                            if (item.Value < random.Next(1, 100 + 1) == true)
                            {
                                //範囲外
                                continue;
                            }
                            //範囲内
                            var tar = classGameStatus.ListPower.Where(x => x.NameTag == item.Key).FirstOrDefault();
                            if (tar == null) continue;
                            targetPowers.Add(tar);
                            //本来ならターゲットは複数あっても良いが、今は一つに絞る
                            break;//複数の時はこれを外す
                        }

                        //この時点でターゲット勢力無しでかつ、勢力が存在する場合、
                        //ランダムな勢力をターゲット勢力にする
                        if (targetPowers.Count == 0 && absTargetPowerList.Count != 0)
                        {
                            Random randomTwo = new Random(DateTime.Now.Second);
                            var abc = absTargetPowerList.OrderBy(x => randomTwo.Next()).FirstOrDefault();
                            var ch = classGameStatus.ListPower.Where(x => x.NameTag == abc.Key).FirstOrDefault();
                            if (ch != null)
                            {
                                targetPowers.Add(ch);
                            }
                        }

                        if (targetPowers.Count == 0)
                        {
                            ////ターゲットが無い
                            //適当な都市で徴兵や内政
                            int cou = mySpot.Count();
                            int targetNum = random.Next(0, cou);
                            var targetSpot = mySpot.ToList()[targetNum];

                            ////徴兵・内政
                            //空都市かチェック
                            if (targetSpot.UnitGroup.Count == 0)
                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = 0;
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        != classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    }

                                    counterUnitGroup++;
                                }
                            }
                            else
                            {
                                //同系統徴兵
                                foreach (var itemUnitGroup in targetSpot.UnitGroup)
                                {
                                    var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                    if (unitBase == null)
                                    {
                                        continue;
                                    }

                                    while (classPower.Money - unitBase.Cost > 0
                                        && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                        classPower.Money = classPower.Money - unitBase.Cost;
                                    }
                                }

                                {
                                    var unitBase = classGameStatus.ListUnit
                                                    .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                    .ToList();
                                    int targetNumunitBase = random.Next(0, unitBase.Count());

                                    int counterUnitGroup = targetSpot.UnitGroup.Count();
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                            && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                    {
                                        targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                        while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                            && targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                            != classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                        {
                                            targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                            targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                            classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                        }

                                        counterUnitGroup++;
                                    }
                                }

                            }
                        }
                        else
                        {
                            ////ターゲットとの国境都市で徴兵や内政
                            //本来は乱数で決めちゃダメ
                            //ターゲットとの国境都市を乱数で取得

                            //ターゲットとの国境都市(他国)を取得
                            var targetLand = spotOtherLand.Where(x => x.PowerNameTag == targetPowers[0].NameTag);
                            if (targetLand.Count() == 0)
                            {
                                break;
                            }

                            List<string> targetLandString = new List<string>();
                            foreach (var itemLand in targetLand)
                            {
                                targetLandString.Add(itemLand.NameTag);
                            }

                            //ターゲットとの国境都市(自国)を取得
                            List<string> targetMySpots = new List<string>();
                            foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                            {
                                if (targetLandString.Contains(itemListLinkSpot.Item1))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item2).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpots.Add(itemListLinkSpot.Item2);
                                    }
                                }
                                if (targetLandString.Contains(itemListLinkSpot.Item2))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item1).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpots.Add(itemListLinkSpot.Item1);
                                    }
                                }
                            }
                            targetMySpots = targetMySpots.Distinct().ToList();

                            int cou = targetMySpots.Count();
                            int targetNum = random.Next(0, cou);
                            var ch = classGameStatus.NowListSpot.Where(x => x.NameTag == targetMySpots[targetNum]).FirstOrDefault();
                            if (ch == null)
                            {
                                break;
                            }

                            var targetSpot = ch;

                            //他都市からユニット移動
                            Random randomOne = new Random(DateTime.Now.Second);
                            int sc = classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity;
                            var moveUnitSpot = mySpot.Where(x => x != targetSpot).OrderBy(x => randomOne.Next());
                            foreach (var itemMoveUnitSpot in moveUnitSpot)
                            {
                                if (sc - targetSpot.UnitGroup.Count <= 0)
                                {
                                    break;
                                }
                                List<ClassHorizontalUnit> lisHo = new List<ClassHorizontalUnit>();
                                foreach (var itemMoveUnitSpotUnitGroup in itemMoveUnitSpot.UnitGroup)
                                {
                                    if (sc - targetSpot.UnitGroup.Count <= 0)
                                    {
                                        break;
                                    }
                                    targetSpot.UnitGroup.Add(itemMoveUnitSpotUnitGroup);
                                    lisHo.Add(itemMoveUnitSpotUnitGroup);
                                }

                                //元都市から削除
                                foreach (var itemLisHo in lisHo)
                                {
                                    itemMoveUnitSpot.UnitGroup.Remove(itemLisHo);
                                }

                            }

                            ////徴兵・内政
                            //同系統徴兵
                            foreach (var itemUnitGroup in targetSpot.UnitGroup)
                            {
                                var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                if (unitBase == null)
                                {
                                    continue;
                                }

                                while (classPower.Money - unitBase.Cost > 0
                                    && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                {
                                    itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                    classPower.Money = classPower.Money - unitBase.Cost;
                                }
                            }

                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = targetSpot.UnitGroup.Count();
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        != classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    }

                                    counterUnitGroup++;
                                }
                            }

                        }
                    }

                    break;
                case _010_Enum.FlagPowerFix.off:

                    {
                        //Power構造体からdiplomacyを取得
                        //Power構造体からhomeを取得
                        //Power構造体からfixを取得
                        //現在のdiploを取得
                        //現在のenemy_powerを取得
                        //現在のleagueを取得

                        ///どの隣国を狙うかチェック
                        //ランダム(補正有り)でターゲットとなる国を選ぶ
                        //現在のhomeを考慮
                        //現在のdiploを考慮
                        //現在のenemy_powerを考慮
                        //現在のleagueを考慮

                        //攻め入るかチェック
                        //戦力差、アンチユニット・アンチスキルなどで計算

                        //攻め入る

                        //徴兵など次ターンの準備する
                    }

                    break;
                case _010_Enum.FlagPowerFix.home:

                    {
                        ////攻め入るかチェック
                        //自国領土を取得
                        List<ClassSpot> mySpot = new List<ClassSpot>();
                        foreach (var item in classGameStatus.NowListSpot)
                        {
                            if (item.PowerNameTag == classPower.NameTag)
                            {
                                mySpot.Add(item);
                            }
                        }

                        //自国領土と接触している他国領土のタグを取得
                        List<ClassSpot> spotOtherLand = new List<ClassSpot>();
                        foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                        {
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item1.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item2
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item2.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item1
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                        }

                        spotOtherLand = spotOtherLand.Distinct().ToList();

                        //隣接している他国の一覧を取得
                        List<ClassPower> adjacentPowers = new List<ClassPower>();
                        foreach (var item in spotOtherLand)
                        {
                            var result = classGameStatus.NowListPower
                                            .Where(x => x != classPower)
                                            .Where(x => x.NameTag == item.PowerNameTag)
                                            .FirstOrDefault();
                            if (result != null)
                            {
                                adjacentPowers.Add(result);
                            }
                        }

                        //自国領土と接触している他国領土にhomeがあるかチェック
                        var intersect = spotOtherLand
                                        .Select(x => x.NameTag)
                                        .Intersect(classPower.ListHome)
                                        .FirstOrDefault();

                        if (intersect == null || intersect == string.Empty)
                        {
                            break;
                        }

                        //戦力差、アンチユニット・アンチスキルなどで計算

                        ////homeと隣接してる自都市から兵を出す
                        //ターゲットとの国境都市(自国)を取得
                        List<string> targetMySpots = new List<string>();
                        foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                        {
                            if (intersect.Contains(itemListLinkSpot.Item1))
                            {
                                var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item2).FirstOrDefault();
                                if (msB != null)
                                {
                                    targetMySpots.Add(itemListLinkSpot.Item2);
                                }
                            }
                            if (intersect.Contains(itemListLinkSpot.Item2))
                            {
                                var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item1).FirstOrDefault();
                                if (msB != null)
                                {
                                    targetMySpots.Add(itemListLinkSpot.Item1);
                                }
                            }
                        }
                        targetMySpots = targetMySpots
                                        .Distinct()
                                        .ToList();

                        if (targetMySpots.Count == 0)
                        {
                            break;
                        }

                        var selectSpot = mySpot.Where(x => x.NameTag == targetMySpots[0]).FirstOrDefault();
                        if (selectSpot == null)
                        {
                            break;
                        }

                        foreach (var item in selectSpot.UnitGroup.Where(x => x.FlagDisplay == true))
                        {
                            //出撃クラスにunit追加
                            mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Add(item);

                            item.FlagDisplay = false;
                        }

                        //他国都市の最初の一つを取得
                        var defSpot = spotOtherLand.Where(x => x.NameTag == intersect).FirstOrDefault();
                        if (defSpot == null)
                        {
                            break;
                        }
                        //防衛ユニット設定
                        foreach (var item in defSpot.UnitGroup)
                        {
                            if (mainWindow.ClassGameStatus.ClassBattle.DefUnitGroup.Count()
                                < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].WarCapacity)
                            {
                                mainWindow.ClassGameStatus.ClassBattle.DefUnitGroup.Add(item);
                            }
                            else
                            {
                                break;
                            }
                        }

                        // 攻め込む都市がプレイヤー都市かどうかチェック
                        {
                            var getPo = classGameStatus.NowListPower.Where(x => x.NameTag == defSpot.PowerNameTag).FirstOrDefault();
                            if (getPo == null)
                            {
                                mainWindow.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer = _010_Enum.BattleWhichIsThePlayer.None;
                            }
                            else
                            {
                                if (getPo.NameTag == classGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
                                {
                                    mainWindow.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer = _010_Enum.BattleWhichIsThePlayer.Def;
                                }
                                else
                                {
                                    mainWindow.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer = _010_Enum.BattleWhichIsThePlayer.None;
                                }
                            }
                        }

                        //map設定
                        var extractMap = mainWindow
                                        .ClassGameStatus
                                        .ListClassMapBattle
                                        .Where(x => x.TagName == defSpot.Map)
                                        .FirstOrDefault();
                        if (extractMap != null)
                        {
                            mainWindow.ClassGameStatus.ClassBattle.ClassMapBattle = extractMap;

                            Application.Current.Dispatcher.Invoke(new Func<bool>(() =>
                            {
                                ClassStaticBattle.AddBuilding(mainWindow.ClassGameStatus);

                                return true;
                            }));
                        }

                        // 現在のマップ表示位置を記録しておく
                        var worldMap = classGameStatus.WorldMap;
                        if (worldMap != null)
                        {
                            classGameStatus.Camera = new Point(Canvas.GetLeft(worldMap), Canvas.GetTop(worldMap));
                        }

                        // 戦闘後に防衛側の情報を参照できるよう記録しておく
                        ClassPowerAndCity classPowerAndCity;
                        if (defSpot.PowerNameTag == string.Empty)
                        {
                            // 中立領地
                            classPowerAndCity = new ClassPowerAndCity(new ClassPower(), defSpot);
                        }
                        else
                        {
                            // 勢力の領地
                            var getPo = classGameStatus.NowListPower.Where(x => x.NameTag == defSpot.PowerNameTag).FirstOrDefault();
                            if (getPo != null)
                            {
                                classPowerAndCity = new ClassPowerAndCity(getPo, defSpot);
                            }
                            else
                            {
                                classPowerAndCity = new ClassPowerAndCity(new ClassPower(), defSpot);
                            }
                        }
                        Application.Current.Properties["defensePowerAndCity"] = classPowerAndCity;

                        // 攻め込むのは次の関数で実行する（全ての準備を終えておくこと）
                        return true;
                        // breakで抜けると return false になるので、return true で強制的に出る。
                        // 戦闘しない場合は、break で抜けるか、return false で終わること。
                    }

                case _010_Enum.FlagPowerFix.hold:

                    {
                        ////他国との国境都市を取得
                        //自国領土を取得
                        List<ClassSpot> mySpot = new List<ClassSpot>();
                        foreach (var item in classGameStatus.NowListSpot)
                        {
                            if (item.PowerNameTag == classPower.NameTag)
                            {
                                mySpot.Add(item);
                            }
                        }

                        //自国領土と接触している他国領土のタグを取得
                        List<ClassSpot> spotOtherLand = new List<ClassSpot>();
                        foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                        {
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item1.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item2
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item2.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item1
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                        }

                        spotOtherLand = spotOtherLand.Distinct().ToList();

                        //隣接している他国の一覧を取得
                        List<ClassPower> adjacentPowers = new List<ClassPower>();
                        foreach (var item in spotOtherLand)
                        {
                            var result = classGameStatus.NowListPower
                                            .Where(x => x != classPower)
                                            .Where(x => x.NameTag == item.PowerNameTag)
                                            .FirstOrDefault();
                            if (result != null)
                            {
                                adjacentPowers.Add(result);
                            }
                        }

                        ////ランダム(補正有り)でターゲットとなる国を選ぶ
                        //友好度50のリストを作る
                        Dictionary<string, int> baseTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in adjacentPowers)
                        {
                            baseTargetPowerList.Add(item.NameTag, 50);
                        }

                        //友好度50のリストを本来のデータで上書きする
                        foreach (var itemBaseTargetPowerList in baseTargetPowerList.ToList())
                        {
                            foreach (var item in classGameStatus.ClassDiplomacy.Diplo)
                            {
                                if (itemBaseTargetPowerList.Key == item.Item1 && item.Item2 == classPower.NameTag)
                                {
                                    baseTargetPowerList[itemBaseTargetPowerList.Key] = item.Item3;
                                    continue;
                                }
                                if (itemBaseTargetPowerList.Key == item.Item2 && item.Item1 == classPower.NameTag)
                                {
                                    baseTargetPowerList[item.Item1] = item.Item3;
                                    continue;
                                }
                            }
                        }

                        //-100して絶対値を取る
                        //友好度100なら0
                        //友好度0なら100
                        Dictionary<string, int> absTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in baseTargetPowerList)
                        {
                            absTargetPowerList.Add(item.Key, Math.Abs(item.Value - 100));
                        }

                        ////ランダムで値を取得して、絶対値と比較
                        //範囲内ならターゲットの国とする
                        Random random = new Random(DateTime.Now.Second);
                        List<ClassPower> targetPowers = new List<ClassPower>();
                        foreach (var item in absTargetPowerList)
                        {
                            if (item.Value < random.Next(1, 100 + 1) == true)
                            {
                                //範囲外
                                continue;
                            }
                            //範囲内
                            var tar = classGameStatus.ListPower.Where(x => x.NameTag == item.Key).FirstOrDefault();
                            if (tar == null) continue;
                            targetPowers.Add(tar);
                            //本来ならターゲットは複数あっても良いが、今は一つに絞る
                            break;//複数の時はこれを外す
                        }

                        if (targetPowers.Count == 0)
                        {
                            ////ターゲットが無い
                            //適当な都市で徴兵や内政
                            int cou = mySpot.Count();
                            int targetNum = random.Next(0, cou);
                            var targetSpot = mySpot.ToList()[targetNum];

                            ////徴兵・内政
                            //空都市かチェック
                            if (targetSpot.UnitGroup.Count == 0)
                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = 0;
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        != classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    }

                                    counterUnitGroup++;
                                }
                            }
                            else
                            {
                                //同系統徴兵
                                foreach (var itemUnitGroup in targetSpot.UnitGroup)
                                {
                                    var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                    if (unitBase == null)
                                    {
                                        continue;
                                    }

                                    while (classPower.Money - unitBase.Cost > 0
                                        && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                        classPower.Money = classPower.Money - unitBase.Cost;
                                    }
                                }
                            }
                        }
                        else
                        {
                            ////ターゲットとの国境都市で徴兵や内政
                            //本来は乱数で決めちゃダメ
                            //ターゲットとの国境都市を乱数で取得

                            //ターゲットとの国境都市(他国)を取得
                            var targetLand = spotOtherLand.Where(x => x.PowerNameTag == targetPowers[0].NameTag);
                            if (targetLand.Count() == 0)
                            {
                                break;
                            }

                            List<string> targetLandString = new List<string>();
                            foreach (var itemLand in targetLand)
                            {
                                targetLandString.Add(itemLand.NameTag);
                            }

                            //ターゲットとの国境都市(自国)を取得
                            List<string> targetMySpot = new List<string>();
                            foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                            {
                                if (targetLandString.Contains(itemListLinkSpot.Item1))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item2).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpot.Add(itemListLinkSpot.Item2);
                                    }
                                }
                                if (targetLandString.Contains(itemListLinkSpot.Item2))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item1).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpot.Add(itemListLinkSpot.Item1);
                                    }
                                }
                            }
                            targetMySpot = targetMySpot.Distinct().ToList();

                            int cou = targetMySpot.Count();
                            int targetNum = random.Next(0, cou);
                            var ch = classGameStatus.NowListSpot.Where(x => x.NameTag == targetMySpot[targetNum]).FirstOrDefault();
                            if (ch == null)
                            {
                                break;
                            }

                            var targetSpot = ch;

                            ////徴兵・内政
                            //同系統徴兵
                            foreach (var itemUnitGroup in targetSpot.UnitGroup)
                            {
                                var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                if (unitBase == null)
                                {
                                    continue;
                                }

                                while (classPower.Money - unitBase.Cost > 0
                                    && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                {
                                    itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                    classPower.Money = classPower.Money - unitBase.Cost;
                                }
                            }
                        }
                        //その他準備
                    }

                    break;
                case _010_Enum.FlagPowerFix.warlike:

                    {
                        ////他国との国境都市を取得
                        //自国領土を取得
                        List<ClassSpot> mySpot = new List<ClassSpot>();
                        foreach (var item in classGameStatus.NowListSpot)
                        {
                            if (item.PowerNameTag == classPower.NameTag)
                            {
                                mySpot.Add(item);
                            }
                        }

                        //自国領土と接触している他国領土のタグを取得
                        List<ClassSpot> spotOtherLand = new List<ClassSpot>();
                        foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                        {
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item1.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var abc = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item2
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (abc == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(abc);
                                continue;
                            }
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item2.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var abc = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item1
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (abc == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(abc);
                                continue;
                            }
                        }

                        spotOtherLand = spotOtherLand.Distinct().ToList();

                        //隣接している他国の一覧を取得
                        List<ClassPower> adjacentPowers = new List<ClassPower>();
                        foreach (var item in spotOtherLand)
                        {
                            var result = classGameStatus.NowListPower
                                            .Where(x => x != classPower)
                                            .Where(x => x.NameTag == item.PowerNameTag)
                                            .FirstOrDefault();
                            if (result != null)
                            {
                                adjacentPowers.Add(result);
                            }
                        }

                        ////ランダム(補正有り)でターゲットとなる国を選ぶ
                        //友好度50のリストを作る
                        Dictionary<string, int> baseTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in adjacentPowers)
                        {
                            baseTargetPowerList.Add(item.NameTag, 50);
                        }

                        //友好度50のリストを本来のデータで上書きする
                        foreach (var itemBaseTargetPowerList in baseTargetPowerList.ToList())
                        {
                            foreach (var item in classGameStatus.ClassDiplomacy.Diplo)
                            {
                                if (itemBaseTargetPowerList.Key == item.Item1 && item.Item2 == classPower.NameTag)
                                {
                                    baseTargetPowerList[itemBaseTargetPowerList.Key] = item.Item3;
                                    continue;
                                }
                                if (itemBaseTargetPowerList.Key == item.Item2 && item.Item1 == classPower.NameTag)
                                {
                                    baseTargetPowerList[item.Item1] = item.Item3;
                                    continue;
                                }
                            }
                        }

                        //-100して絶対値を取る
                        //友好度100なら0
                        //友好度0なら100
                        Dictionary<string, int> absTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in baseTargetPowerList)
                        {
                            absTargetPowerList.Add(item.Key, Math.Abs(item.Value - 100));
                        }

                        ////ランダムで値を取得して、絶対値と比較
                        //範囲内ならターゲットの国とする
                        Random random = new Random(DateTime.Now.Second);
                        List<ClassPower> targetPowers = new List<ClassPower>();
                        foreach (var item in absTargetPowerList)
                        {
                            if (item.Value < random.Next(1, 100 + 1) == true)
                            {
                                //範囲外
                                continue;
                            }
                            //範囲内
                            var tar = classGameStatus.ListPower.Where(x => x.NameTag == item.Key).FirstOrDefault();
                            if (tar == null) continue;
                            targetPowers.Add(tar);
                            //本来ならターゲットは複数あっても良いが、今は一つに絞る
                            break;//複数の時はこれを外す
                        }

                        //この時点でターゲット勢力無しでかつ、勢力が存在する場合、
                        //ランダムな勢力をターゲット勢力にする
                        if (targetPowers.Count == 0 && absTargetPowerList.Count != 0)
                        {
                            Random randomTwo = new Random(DateTime.Now.Second);
                            var abc = absTargetPowerList.OrderBy(x => randomTwo.Next()).FirstOrDefault();
                            var aaaa = classGameStatus.ListPower.Where(x => x.NameTag == abc.Key).FirstOrDefault();
                            if (aaaa != null)
                            {
                                targetPowers.Add(aaaa);
                            }
                        }

                        //ターゲット勢力がいない
                        if (targetPowers.Count == 0)
                        {
                            ////ターゲットが無い
                            //適当な都市で徴兵や内政
                            int abc = random.Next(0, mySpot.Count());
                            var targetSpotWarLike = mySpot.ToList()[abc];

                            ////徴兵・内政
                            //空都市かチェック
                            if (targetSpotWarLike.UnitGroup.Count == 0)
                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = 0;
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpotWarLike.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpotWarLike.UnitGroup.Add(new ClassHorizontalUnit());
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpotWarLike.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        != classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        targetSpotWarLike.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpotWarLike.UnitGroup[counterUnitGroup].Spot = targetSpotWarLike;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    }

                                    counterUnitGroup++;
                                }
                            }
                            else
                            {
                                //同系統徴兵
                                foreach (var itemUnitGroup in targetSpotWarLike.UnitGroup)
                                {
                                    var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                    if (unitBase == null)
                                    {
                                        continue;
                                    }

                                    while (classPower.Money - unitBase.Cost > 0
                                        && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                        classPower.Money = classPower.Money - unitBase.Cost;
                                    }
                                }

                                {
                                    var unitBase = classGameStatus.ListUnit
                                                    .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                    .ToList();
                                    int targetNumunitBase = random.Next(0, unitBase.Count());

                                    int counterUnitGroup = targetSpotWarLike.UnitGroup.Count();
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                            && targetSpotWarLike.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                    {
                                        targetSpotWarLike.UnitGroup.Add(new ClassHorizontalUnit());
                                        while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                            && targetSpotWarLike.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                            != classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                        {
                                            targetSpotWarLike.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                            targetSpotWarLike.UnitGroup[counterUnitGroup].Spot = targetSpotWarLike;
                                            classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                        }

                                        counterUnitGroup++;
                                    }
                                }

                            }

                            return false;
                        }

                        ////ターゲット勢力がいる（隣接
                        //ターゲットとの国境都市(他国)を取得
                        var targetLand = spotOtherLand.Where(x => x.PowerNameTag == targetPowers[0].NameTag);
                        if (targetLand.Count() == 0)
                        {
                            break;
                        }
                        List<string> targetLandString = new List<string>();
                        foreach (var itemLand in targetLand)
                        {
                            targetLandString.Add(itemLand.NameTag);
                        }

                        //ターゲットとの国境都市(自国)を取得
                        List<string> targetMySpots = new List<string>();
                        foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                        {
                            if (targetLandString.Contains(itemListLinkSpot.Item1))
                            {
                                var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item2).FirstOrDefault();
                                if (msB != null)
                                {
                                    targetMySpots.Add(itemListLinkSpot.Item2);
                                }
                            }
                            if (targetLandString.Contains(itemListLinkSpot.Item2))
                            {
                                var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item1).FirstOrDefault();
                                if (msB != null)
                                {
                                    targetMySpots.Add(itemListLinkSpot.Item1);
                                }
                            }
                        }
                        targetMySpots = targetMySpots.Distinct().ToList();

                        int cou = targetMySpots.Count();
                        int targetNum = random.Next(0, cou);
                        var ch = classGameStatus.NowListSpot.Where(x => x.NameTag == targetMySpots[targetNum]).FirstOrDefault();
                        if (ch == null)
                        {
                            break;
                        }

                        var targetSpot = ch;

                        //出撃
                        foreach (var item in targetSpot.UnitGroup.Where(x => x.FlagDisplay == true))
                        {
                            //出撃クラスにunit追加
                            mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Add(item);

                            item.FlagDisplay = false;
                        }

                        //他国都市の最初の一つを取得
                        var defSpot = spotOtherLand.Where(x => x.NameTag == targetLandString[0]).FirstOrDefault();
                        if (defSpot == null)
                        {
                            break;
                        }

                        //防衛ユニット設定
                        foreach (var item in defSpot.UnitGroup)
                        {
                            if (mainWindow.ClassGameStatus.ClassBattle.DefUnitGroup.Count()
                                < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].WarCapacity)
                            {
                                mainWindow.ClassGameStatus.ClassBattle.DefUnitGroup.Add(item);
                            }
                            else
                            {
                                break;
                            }
                        }

                        // 攻め込む都市がプレイヤー都市かどうかチェック
                        {
                            var getPo = classGameStatus.NowListPower.Where(x => x.NameTag == defSpot.PowerNameTag).FirstOrDefault();
                            if (getPo == null)
                            {
                                mainWindow.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer = _010_Enum.BattleWhichIsThePlayer.None;
                            }
                            else
                            {
                                if (getPo.NameTag == classGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
                                {
                                    mainWindow.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer = _010_Enum.BattleWhichIsThePlayer.Def;
                                }
                                else
                                {
                                    mainWindow.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer = _010_Enum.BattleWhichIsThePlayer.None;
                                }
                            }
                        }

                        //map設定
                        var extractMap = mainWindow
                                        .ClassGameStatus
                                        .ListClassMapBattle
                                        .Where(x => x.TagName == defSpot.Map)
                                        .FirstOrDefault();
                        if (extractMap != null)
                        {
                            mainWindow.ClassGameStatus.ClassBattle.ClassMapBattle = extractMap;

                            Application.Current.Dispatcher.Invoke(new Func<bool>(() =>
                            {
                                ClassStaticBattle.AddBuilding(mainWindow.ClassGameStatus);

                                return true;
                            }));
                        }

                        // 現在のマップ表示位置を記録しておく
                        var worldMap = classGameStatus.WorldMap;
                        if (worldMap != null)
                        {
                            classGameStatus.Camera = new Point(Canvas.GetLeft(worldMap), Canvas.GetTop(worldMap));
                        }

                        // 戦闘後に防衛側の情報を参照できるよう記録しておく
                        ClassPowerAndCity classPowerAndCity;
                        if (defSpot.PowerNameTag == string.Empty)
                        {
                            // 中立領地
                            classPowerAndCity = new ClassPowerAndCity(new ClassPower(), defSpot);
                        }
                        else
                        {
                            // 勢力の領地
                            var getPo = classGameStatus.NowListPower.Where(x => x.NameTag == defSpot.PowerNameTag).FirstOrDefault();
                            if (getPo != null)
                            {
                                classPowerAndCity = new ClassPowerAndCity(getPo, defSpot);
                            }
                            else
                            {
                                classPowerAndCity = new ClassPowerAndCity(new ClassPower(), defSpot);
                            }
                        }
                        Application.Current.Properties["defensePowerAndCity"] = classPowerAndCity;

                        // 攻め込むのは次の関数で実行する（全ての準備を終えておくこと）
                        return true;
                        // breakで抜けると return false になるので、return true で強制的に出る。
                        // 戦闘しない場合は、break で抜けるか、return false で終わること。
                    }

                case _010_Enum.FlagPowerFix.freeze:
                    break;
                default:
                    break;
            }

            return false;
        }

        // 戦闘を開始する
        public static void StartBattle(MainWindow mainWindow)
        {
            //攻め入る
            mainWindow.IsBattle = true;
            Application.Current.Dispatcher.Invoke(new Func<bool>(() =>
            {
                mainWindow.SetBattleMap();

                return true;
            }));
        }

        // 戦闘終了後に徴兵・再配置・別の戦闘などを行う
        // COM勢力の戦闘処理に続ける場合は true を返すこと
        public static bool AfterBattle(ClassGameStatus classGameStatus, ClassPower classPower, MainWindow mainWindow)
        {
            switch (classPower.Fix)
            {
                case _010_Enum.FlagPowerFix.on:

                    //やらない

                    break;
                case _010_Enum.FlagPowerFix.off:

                    {
                        ////他国との国境都市を取得
                        //自国領土を取得
                        List<ClassSpot> mySpot = new List<ClassSpot>();
                        foreach (var item in classGameStatus.NowListSpot)
                        {
                            if (item.PowerNameTag == classPower.NameTag)
                            {
                                mySpot.Add(item);
                            }
                        }

                        //自国領土と接触している他国領土のタグを取得
                        List<ClassSpot> spotOtherLand = new List<ClassSpot>();
                        foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                        {
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item1.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item2
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item2.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item1
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                        }

                        spotOtherLand = spotOtherLand.Distinct().ToList();

                        //隣接している他国の一覧を取得
                        List<ClassPower> adjacentPowers = new List<ClassPower>();
                        foreach (var item in spotOtherLand)
                        {
                            var result = classGameStatus.NowListPower
                                            .Where(x => x != classPower)
                                            .Where(x => x.NameTag == item.PowerNameTag)
                                            .FirstOrDefault();
                            if (result != null)
                            {
                                adjacentPowers.Add(result);
                            }
                        }

                        ////ランダム(補正有り)でターゲットとなる国を選ぶ
                        //友好度50のリストを作る
                        Dictionary<string, int> baseTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in adjacentPowers)
                        {
                            baseTargetPowerList.Add(item.NameTag, 50);
                        }

                        //友好度50のリストを本来のデータで上書きする
                        foreach (var itemBaseTargetPowerList in baseTargetPowerList.ToList())
                        {
                            foreach (var item in classGameStatus.ClassDiplomacy.Diplo)
                            {
                                if (itemBaseTargetPowerList.Key == item.Item1 && item.Item2 == classPower.NameTag)
                                {
                                    baseTargetPowerList[itemBaseTargetPowerList.Key] = item.Item3;
                                    continue;
                                }
                                if (itemBaseTargetPowerList.Key == item.Item2 && item.Item1 == classPower.NameTag)
                                {
                                    baseTargetPowerList[item.Item1] = item.Item3;
                                    continue;
                                }
                            }
                        }

                        //-100して絶対値を取る
                        //友好度100なら0
                        //友好度0なら100
                        Dictionary<string, int> absTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in baseTargetPowerList)
                        {
                            absTargetPowerList.Add(item.Key, Math.Abs(item.Value - 100));
                        }

                        ////ランダムで値を取得して、絶対値と比較
                        //範囲内ならターゲットの国とする
                        Random random = new Random(DateTime.Now.Second);
                        List<ClassPower> targetPowers = new List<ClassPower>();
                        foreach (var item in absTargetPowerList)
                        {
                            if (item.Value < random.Next(1, 100 + 1) == true)
                            {
                                //範囲外
                                continue;
                            }
                            //範囲内
                            var tar = classGameStatus.ListPower.Where(x => x.NameTag == item.Key).FirstOrDefault();
                            if (tar == null) continue;
                            targetPowers.Add(tar);
                            //本来ならターゲットは複数あっても良いが、今は一つに絞る
                            break;//複数の時はこれを外す
                        }

                        if (targetPowers.Count == 0 && absTargetPowerList.Count != 0)
                        {
                            Random randomTwo = new Random(DateTime.Now.Second);
                            var abc = absTargetPowerList.OrderBy(x => randomTwo.Next()).FirstOrDefault();
                            var ch = classGameStatus.ListPower.Where(x => x.NameTag == abc.Key).FirstOrDefault();
                            if (ch != null)
                            {
                                targetPowers.Add(ch);
                            }
                        }

                        if (targetPowers.Count == 0)
                        {
                            ////ターゲットが無い
                            //適当な都市で徴兵や内政
                            int cou = mySpot.Count();
                            int targetNum = random.Next(0, cou);
                            var targetSpot = mySpot.ToList()[targetNum];

                            ////徴兵・内政
                            //空都市かチェック
                            if (targetSpot.UnitGroup.Count == 0)
                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = 0;
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                    targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                    targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                    classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    if (targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        == classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        counterUnitGroup++;
                                    }
                                }
                            }
                            else
                            {
                                //同系統徴兵
                                foreach (var itemUnitGroup in targetSpot.UnitGroup)
                                {
                                    var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                    if (unitBase == null)
                                    {
                                        continue;
                                    }

                                    while (classPower.Money - unitBase.Cost > 0
                                        && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                        classPower.Money = classPower.Money - unitBase.Cost;
                                    }
                                }

                                {
                                    var unitBase = classGameStatus.ListUnit
                                                    .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                    .ToList();
                                    int targetNumunitBase = random.Next(0, unitBase.Count());

                                    int counterUnitGroup = targetSpot.UnitGroup.Count();
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                            && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                    {
                                        targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                        targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                        if (targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                            == classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                        {
                                            counterUnitGroup++;
                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            ////ターゲットとの国境都市で徴兵や内政
                            //本来は乱数で決めちゃダメ
                            //ターゲットとの国境都市を乱数で取得

                            //ターゲットとの国境都市(他国)を取得
                            var targetLand = spotOtherLand.Where(x => x.PowerNameTag == targetPowers[0].NameTag);
                            if (targetLand.Count() == 0)
                            {
                                break;
                            }

                            List<string> targetLandString = new List<string>();
                            foreach (var itemLand in targetLand)
                            {
                                targetLandString.Add(itemLand.NameTag);
                            }

                            //ターゲットとの国境都市(自国)を取得
                            List<string> targetMySpots = new List<string>();
                            foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                            {
                                if (targetLandString.Contains(itemListLinkSpot.Item1))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item2).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpots.Add(itemListLinkSpot.Item2);
                                    }
                                }
                                if (targetLandString.Contains(itemListLinkSpot.Item2))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item1).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpots.Add(itemListLinkSpot.Item1);
                                    }
                                }
                            }
                            targetMySpots = targetMySpots.Distinct().ToList();

                            int cou = targetMySpots.Count();
                            int targetNum = random.Next(0, cou);
                            var ch = classGameStatus.NowListSpot.Where(x => x.NameTag == targetMySpots[targetNum]).FirstOrDefault();
                            if (ch == null)
                            {
                                break;
                            }

                            var targetSpot = ch;

                            //他都市からユニット移動
                            Random randomOne = new Random(DateTime.Now.Second);
                            int sc = classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity;
                            var moveUnitSpot = mySpot.Where(x => x != targetSpot).OrderBy(x => randomOne.Next());
                            foreach (var itemMoveUnitSpot in moveUnitSpot)
                            {
                                if (sc - targetSpot.UnitGroup.Count <= 0)
                                {
                                    break;
                                }
                                List<ClassHorizontalUnit> lisHo = new List<ClassHorizontalUnit>();
                                foreach (var itemMoveUnitSpotUnitGroup in itemMoveUnitSpot.UnitGroup)
                                {
                                    if (sc - targetSpot.UnitGroup.Count <= 0)
                                    {
                                        break;
                                    }
                                    targetSpot.UnitGroup.Add(itemMoveUnitSpotUnitGroup);
                                    lisHo.Add(itemMoveUnitSpotUnitGroup);
                                }

                                //元都市から削除
                                foreach (var itemLisHo in lisHo)
                                {
                                    itemMoveUnitSpot.UnitGroup.Remove(itemLisHo);
                                }

                            }

                            ////徴兵・内政
                            //同系統徴兵
                            foreach (var itemUnitGroup in targetSpot.UnitGroup)
                            {
                                var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                if (unitBase == null)
                                {
                                    continue;
                                }

                                while (classPower.Money - unitBase.Cost > 0
                                    && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                {
                                    itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                    classPower.Money = classPower.Money - unitBase.Cost;
                                }
                            }

                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = targetSpot.UnitGroup.Count();
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        != classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    }
                                    counterUnitGroup++;
                                }
                            }

                        }
                    }


                    break;
                case _010_Enum.FlagPowerFix.home:

                    {
                        ////他国との国境都市を取得
                        //自国領土を取得
                        List<ClassSpot> mySpot = new List<ClassSpot>();
                        foreach (var item in classGameStatus.NowListSpot)
                        {
                            if (item.PowerNameTag == classPower.NameTag)
                            {
                                mySpot.Add(item);
                            }
                        }

                        //自国領土と接触している他国領土のタグを取得
                        List<ClassSpot> spotOtherLand = new List<ClassSpot>();
                        foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                        {
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item1.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item2
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item2.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item1
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                        }

                        spotOtherLand = spotOtherLand.Distinct().ToList();

                        //隣接している他国の一覧を取得
                        List<ClassPower> adjacentPowers = new List<ClassPower>();
                        foreach (var item in spotOtherLand)
                        {
                            var result = classGameStatus.NowListPower
                                            .Where(x => x != classPower)
                                            .Where(x => x.NameTag == item.PowerNameTag)
                                            .FirstOrDefault();
                            if (result != null)
                            {
                                adjacentPowers.Add(result);
                            }
                        }

                        ////ランダム(補正有り)でターゲットとなる国を選ぶ
                        //友好度50のリストを作る
                        Dictionary<string, int> baseTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in adjacentPowers)
                        {
                            baseTargetPowerList.Add(item.NameTag, 50);
                        }

                        //友好度50のリストを本来のデータで上書きする
                        foreach (var itemBaseTargetPowerList in baseTargetPowerList.ToList())
                        {
                            foreach (var item in classGameStatus.ClassDiplomacy.Diplo)
                            {
                                if (itemBaseTargetPowerList.Key == item.Item1 && item.Item2 == classPower.NameTag)
                                {
                                    baseTargetPowerList[itemBaseTargetPowerList.Key] = item.Item3;
                                    continue;
                                }
                                if (itemBaseTargetPowerList.Key == item.Item2 && item.Item1 == classPower.NameTag)
                                {
                                    baseTargetPowerList[item.Item1] = item.Item3;
                                    continue;
                                }
                            }
                        }

                        //-100して絶対値を取る
                        //友好度100なら0
                        //友好度0なら100
                        Dictionary<string, int> absTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in baseTargetPowerList)
                        {
                            absTargetPowerList.Add(item.Key, Math.Abs(item.Value - 100));
                        }

                        ////ランダムで値を取得して、絶対値と比較
                        //範囲内ならターゲットの国とする
                        Random random = new Random(DateTime.Now.Second);
                        List<ClassPower> targetPowers = new List<ClassPower>();
                        foreach (var item in absTargetPowerList)
                        {
                            if (item.Value < random.Next(1, 100 + 1) == true)
                            {
                                //範囲外
                                continue;
                            }
                            //範囲内
                            var tar = classGameStatus.ListPower.Where(x => x.NameTag == item.Key).FirstOrDefault();
                            if (tar == null) continue;
                            targetPowers.Add(tar);
                            //本来ならターゲットは複数あっても良いが、今は一つに絞る
                            break;//複数の時はこれを外す
                        }

                        if (targetPowers.Count == 0 && absTargetPowerList.Count != 0)
                        {
                            Random randomTwo = new Random(DateTime.Now.Second);
                            var abc = absTargetPowerList.OrderBy(x => randomTwo.Next()).FirstOrDefault();
                            var ch = classGameStatus.ListPower.Where(x => x.NameTag == abc.Key).FirstOrDefault();
                            if (ch != null)
                            {
                                targetPowers.Add(ch);
                            }
                        }

                        if (targetPowers.Count == 0)
                        {
                            ////ターゲットが無い
                            //適当な都市で徴兵や内政
                            int cou = mySpot.Count();
                            int targetNum = random.Next(0, cou);
                            var targetSpot = mySpot.ToList()[targetNum];

                            ////徴兵・内政
                            //空都市かチェック
                            if (targetSpot.UnitGroup.Count == 0)
                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = 0;
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                    targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                    targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                    classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    if (targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        == classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        counterUnitGroup++;
                                    }
                                }
                            }
                            else
                            {
                                //同系統徴兵
                                foreach (var itemUnitGroup in targetSpot.UnitGroup)
                                {
                                    var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                    if (unitBase == null)
                                    {
                                        continue;
                                    }

                                    while (classPower.Money - unitBase.Cost > 0
                                        && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                        classPower.Money = classPower.Money - unitBase.Cost;
                                    }
                                }

                                {
                                    var unitBase = classGameStatus.ListUnit
                                                    .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                    .ToList();
                                    int targetNumunitBase = random.Next(0, unitBase.Count());

                                    int counterUnitGroup = targetSpot.UnitGroup.Count();
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                            && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                    {
                                        targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                        targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                        if (targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                            == classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                        {
                                            counterUnitGroup++;
                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            ////ターゲットとの国境都市で徴兵や内政
                            //本来は乱数で決めちゃダメ
                            //ターゲットとの国境都市を乱数で取得

                            //ターゲットとの国境都市(他国)を取得
                            var targetLand = spotOtherLand.Where(x => x.PowerNameTag == targetPowers[0].NameTag);
                            if (targetLand.Count() == 0)
                            {
                                break;
                            }

                            List<string> targetLandString = new List<string>();
                            foreach (var itemLand in targetLand)
                            {
                                targetLandString.Add(itemLand.NameTag);
                            }

                            //ターゲットとの国境都市(自国)を取得
                            List<string> targetMySpots = new List<string>();
                            foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                            {
                                if (targetLandString.Contains(itemListLinkSpot.Item1))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item2).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpots.Add(itemListLinkSpot.Item2);
                                    }
                                }
                                if (targetLandString.Contains(itemListLinkSpot.Item2))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item1).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpots.Add(itemListLinkSpot.Item1);
                                    }
                                }
                            }
                            targetMySpots = targetMySpots.Distinct().ToList();

                            int cou = targetMySpots.Count();
                            int targetNum = random.Next(0, cou);
                            var ch = classGameStatus.NowListSpot.Where(x => x.NameTag == targetMySpots[targetNum]).FirstOrDefault();
                            if (ch == null)
                            {
                                break;
                            }

                            var targetSpot = ch;

                            //他都市からユニット移動
                            Random randomOne = new Random(DateTime.Now.Second);
                            int sc = classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity;
                            var moveUnitSpot = mySpot.Where(x => x != targetSpot).OrderBy(x => randomOne.Next());
                            foreach (var itemMoveUnitSpot in moveUnitSpot)
                            {
                                if (sc - targetSpot.UnitGroup.Count <= 0)
                                {
                                    break;
                                }
                                List<ClassHorizontalUnit> lisHo = new List<ClassHorizontalUnit>();
                                foreach (var itemMoveUnitSpotUnitGroup in itemMoveUnitSpot.UnitGroup)
                                {
                                    if (sc - targetSpot.UnitGroup.Count <= 0)
                                    {
                                        break;
                                    }
                                    targetSpot.UnitGroup.Add(itemMoveUnitSpotUnitGroup);
                                    lisHo.Add(itemMoveUnitSpotUnitGroup);
                                }

                                //元都市から削除
                                foreach (var itemLisHo in lisHo)
                                {
                                    itemMoveUnitSpot.UnitGroup.Remove(itemLisHo);
                                }

                            }

                            ////徴兵・内政
                            //同系統徴兵
                            foreach (var itemUnitGroup in targetSpot.UnitGroup)
                            {
                                var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                if (unitBase == null)
                                {
                                    continue;
                                }

                                while (classPower.Money - unitBase.Cost > 0
                                    && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                {
                                    itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                    classPower.Money = classPower.Money - unitBase.Cost;
                                }
                            }

                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = targetSpot.UnitGroup.Count();
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        != classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    }
                                    counterUnitGroup++;
                                }
                            }

                        }
                    }

                    break;
                case _010_Enum.FlagPowerFix.hold:

                    //やらない

                    break;
                case _010_Enum.FlagPowerFix.warlike:

                    {
                        ////他国との国境都市を取得
                        //自国領土を取得
                        List<ClassSpot> mySpot = new List<ClassSpot>();
                        foreach (var item in classGameStatus.NowListSpot)
                        {
                            if (item.PowerNameTag == classPower.NameTag)
                            {
                                mySpot.Add(item);
                            }
                        }

                        //自国領土と接触している他国領土のタグを取得
                        List<ClassSpot> spotOtherLand = new List<ClassSpot>();
                        foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                        {
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item1.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item2
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                            //自国領土かチェック
                            if (mySpot.Where(x => itemListLinkSpot.Item2.Contains(x.NameTag)).Count() == 1)
                            {
                                ////リンクしている領土が他国の領土かチェック
                                //リンクしている領土を取得
                                var ch = classGameStatus.NowListSpot
                                            .Where(x => x.NameTag == itemListLinkSpot.Item1
                                                    && x.PowerNameTag != classPower.NameTag)
                                            .FirstOrDefault();
                                if (ch == null)
                                {
                                    continue;
                                }
                                //リストへ格納
                                spotOtherLand.Add(ch);
                                continue;
                            }
                        }

                        spotOtherLand = spotOtherLand.Distinct().ToList();

                        //隣接している他国の一覧を取得
                        List<ClassPower> adjacentPowers = new List<ClassPower>();
                        foreach (var item in spotOtherLand)
                        {
                            var result = classGameStatus.NowListPower
                                            .Where(x => x != classPower)
                                            .Where(x => x.NameTag == item.PowerNameTag)
                                            .FirstOrDefault();
                            if (result != null)
                            {
                                adjacentPowers.Add(result);
                            }
                        }

                        ////ランダム(補正有り)でターゲットとなる国を選ぶ
                        //友好度50のリストを作る
                        Dictionary<string, int> baseTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in adjacentPowers)
                        {
                            baseTargetPowerList.Add(item.NameTag, 50);
                        }

                        //友好度50のリストを本来のデータで上書きする
                        foreach (var itemBaseTargetPowerList in baseTargetPowerList.ToList())
                        {
                            foreach (var item in classGameStatus.ClassDiplomacy.Diplo)
                            {
                                if (itemBaseTargetPowerList.Key == item.Item1 && item.Item2 == classPower.NameTag)
                                {
                                    baseTargetPowerList[itemBaseTargetPowerList.Key] = item.Item3;
                                    continue;
                                }
                                if (itemBaseTargetPowerList.Key == item.Item2 && item.Item1 == classPower.NameTag)
                                {
                                    baseTargetPowerList[item.Item1] = item.Item3;
                                    continue;
                                }
                            }
                        }

                        //-100して絶対値を取る
                        //友好度100なら0
                        //友好度0なら100
                        Dictionary<string, int> absTargetPowerList = new Dictionary<string, int>();
                        foreach (var item in baseTargetPowerList)
                        {
                            absTargetPowerList.Add(item.Key, Math.Abs(item.Value - 100));
                        }

                        ////ランダムで値を取得して、絶対値と比較
                        //範囲内ならターゲットの国とする
                        Random random = new Random(DateTime.Now.Second);
                        List<ClassPower> targetPowers = new List<ClassPower>();
                        foreach (var item in absTargetPowerList)
                        {
                            if (item.Value < random.Next(1, 100 + 1) == true)
                            {
                                //範囲外
                                continue;
                            }
                            //範囲内
                            var tar = classGameStatus.ListPower.Where(x => x.NameTag == item.Key).FirstOrDefault();
                            if (tar == null) continue;
                            targetPowers.Add(tar);
                            //本来ならターゲットは複数あっても良いが、今は一つに絞る
                            break;//複数の時はこれを外す
                        }

                        if (targetPowers.Count == 0 && absTargetPowerList.Count != 0)
                        {
                            Random randomTwo = new Random(DateTime.Now.Second);
                            var abc = absTargetPowerList.OrderBy(x => randomTwo.Next()).FirstOrDefault();
                            var ch = classGameStatus.ListPower.Where(x => x.NameTag == abc.Key).FirstOrDefault();
                            if (ch != null)
                            {
                                targetPowers.Add(ch);
                            }
                        }

                        if (targetPowers.Count == 0)
                        {
                            ////ターゲットが無い
                            //適当な都市で徴兵や内政
                            int cou = mySpot.Count();
                            int targetNum = random.Next(0, cou);
                            var targetSpot = mySpot.ToList()[targetNum];

                            ////徴兵・内政
                            //空都市かチェック
                            if (targetSpot.UnitGroup.Count == 0)
                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = 0;
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                    targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                    targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                    classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    if (targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        == classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        counterUnitGroup++;
                                    }
                                }
                            }
                            else
                            {
                                //同系統徴兵
                                foreach (var itemUnitGroup in targetSpot.UnitGroup)
                                {
                                    var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                    if (unitBase == null)
                                    {
                                        continue;
                                    }

                                    while (classPower.Money - unitBase.Cost > 0
                                        && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                        classPower.Money = classPower.Money - unitBase.Cost;
                                    }
                                }

                                {
                                    var unitBase = classGameStatus.ListUnit
                                                    .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                    .ToList();
                                    int targetNumunitBase = random.Next(0, unitBase.Count());

                                    int counterUnitGroup = targetSpot.UnitGroup.Count();
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                            && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                    {
                                        targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                        targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                        if (targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                            == classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                        {
                                            counterUnitGroup++;
                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            ////ターゲットとの国境都市で徴兵や内政
                            //本来は乱数で決めちゃダメ
                            //ターゲットとの国境都市を乱数で取得

                            //ターゲットとの国境都市(他国)を取得
                            var targetLand = spotOtherLand.Where(x => x.PowerNameTag == targetPowers[0].NameTag);
                            if (targetLand.Count() == 0)
                            {
                                break;
                            }

                            List<string> targetLandString = new List<string>();
                            foreach (var itemLand in targetLand)
                            {
                                targetLandString.Add(itemLand.NameTag);
                            }

                            //ターゲットとの国境都市(自国)を取得
                            List<string> targetMySpots = new List<string>();
                            foreach (var itemListLinkSpot in classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].ListLinkSpot)
                            {
                                if (targetLandString.Contains(itemListLinkSpot.Item1))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item2).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpots.Add(itemListLinkSpot.Item2);
                                    }
                                }
                                if (targetLandString.Contains(itemListLinkSpot.Item2))
                                {
                                    var msB = mySpot.Where(x => x.NameTag == itemListLinkSpot.Item1).FirstOrDefault();
                                    if (msB != null)
                                    {
                                        targetMySpots.Add(itemListLinkSpot.Item1);
                                    }
                                }
                            }
                            targetMySpots = targetMySpots.Distinct().ToList();

                            int cou = targetMySpots.Count();
                            int targetNum = random.Next(0, cou);
                            var ch = classGameStatus.NowListSpot.Where(x => x.NameTag == targetMySpots[targetNum]).FirstOrDefault();
                            if (ch == null)
                            {
                                break;
                            }

                            var targetSpot = ch;

                            //他都市からユニット移動
                            Random randomOne = new Random(DateTime.Now.Second);
                            int sc = classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity;
                            var moveUnitSpot = mySpot.Where(x => x != targetSpot).OrderBy(x => randomOne.Next());
                            foreach (var itemMoveUnitSpot in moveUnitSpot)
                            {
                                if (sc - targetSpot.UnitGroup.Count <= 0)
                                {
                                    break;
                                }
                                List<ClassHorizontalUnit> lisHo = new List<ClassHorizontalUnit>();
                                foreach (var itemMoveUnitSpotUnitGroup in itemMoveUnitSpot.UnitGroup)
                                {
                                    if (sc - targetSpot.UnitGroup.Count <= 0)
                                    {
                                        break;
                                    }
                                    targetSpot.UnitGroup.Add(itemMoveUnitSpotUnitGroup);
                                    lisHo.Add(itemMoveUnitSpotUnitGroup);
                                }

                                //元都市から削除
                                foreach (var itemLisHo in lisHo)
                                {
                                    itemMoveUnitSpot.UnitGroup.Remove(itemLisHo);
                                }

                            }

                            ////徴兵・内政
                            //同系統徴兵
                            foreach (var itemUnitGroup in targetSpot.UnitGroup)
                            {
                                var unitBase = classGameStatus.ListUnit.Where(x => x.NameTag == itemUnitGroup.ListClassUnit[0].Friend).FirstOrDefault();
                                if (unitBase == null)
                                {
                                    continue;
                                }

                                while (classPower.Money - unitBase.Cost > 0
                                    && itemUnitGroup.ListClassUnit.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                {
                                    itemUnitGroup.ListClassUnit.Add(unitBase.DeepCopy());
                                    classPower.Money = classPower.Money - unitBase.Cost;
                                }
                            }

                            {
                                var unitBase = classGameStatus.ListUnit
                                                .Where(x => classPower.ListCommonConscription.Contains(x.NameTag))
                                                .ToList();
                                int targetNumunitBase = random.Next(0, unitBase.Count());

                                int counterUnitGroup = targetSpot.UnitGroup.Count();
                                while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup.Count() < classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].SpotCapacity)
                                {
                                    targetSpot.UnitGroup.Add(new ClassHorizontalUnit());
                                    while (classPower.Money - unitBase[targetNumunitBase].Cost > 0
                                        && targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Count()
                                        != classGameStatus.ListClassScenarioInfo[classGameStatus.NumberScenarioSelection].MemberCapacity)
                                    {
                                        targetSpot.UnitGroup[counterUnitGroup].ListClassUnit.Add(unitBase[targetNumunitBase].DeepCopy());
                                        targetSpot.UnitGroup[counterUnitGroup].Spot = targetSpot;
                                        classPower.Money = classPower.Money - unitBase[targetNumunitBase].Cost;
                                    }
                                    counterUnitGroup++;
                                }
                            }

                        }
                    }

                    break;
                case _010_Enum.FlagPowerFix.freeze:
                    break;
                default:
                    break;
            }
            // 今は何もしないで終わる
            return false;
        }

    }
}
