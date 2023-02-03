using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren._006_ClassStatic
{
    public static class ClassStaticStraregyAI
    {
        public static void ThinkingEasy(ClassGameStatus classGameStatus, ClassPower classPower)
        {
            if (classGameStatus == null) return;

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

                classPower.Money += countMoney;
            }

            ////状態によって隣国へ攻め入る
            ///状態をチェック
            switch (classPower.Fix)
            {
                case _010_Enum.FlagPowerFix.on:

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
                            var ch = classGameStatus.NowListSpot.Where(x => x.NameTag == itemListLinkSpot.Item2).FirstOrDefault();
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
                            var ch = classGameStatus.NowListSpot.Where(x => x.NameTag == itemListLinkSpot.Item1).FirstOrDefault();
                            if (ch == null)
                            {
                                continue;
                            }
                            //リストへ格納
                            spotOtherLand.Add(ch);
                            continue;
                        }
                    }

                    //隣接している他国の一覧を取得
                    List<ClassPower> adjacentPowers = new List<ClassPower>();
                    foreach (var item in spotOtherLand)
                    {
                        var result = classGameStatus.NowListPower
                                        .Where(x => x.NameTag == item.NameTag)
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
                        ////ターゲットとの国境都市が無い
                        //適当な都市で徴兵や内政
                        int cou = mySpot.Count();
                        int targetNum = random.Next(0, cou + 1);
                        var targetSpot = mySpot.ToList()[targetNum];

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
                                targetMySpot.Add(itemListLinkSpot.Item2);
                            }
                            if (targetLandString.Contains(itemListLinkSpot.Item2))
                            {
                                targetMySpot.Add(itemListLinkSpot.Item1);
                            }
                        }
                        targetMySpot = targetMySpot.Distinct().ToList();

                        int cou = targetMySpot.Count();
                        int targetNum = random.Next(0, cou + 1);
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

                    break;
                case _010_Enum.FlagPowerFix.off:
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

                    break;
                case _010_Enum.FlagPowerFix.home:
                    //攻め入るかチェック
                    //戦力差、アンチユニット・アンチスキルなどで計算

                    //攻め入る

                    //徴兵など次ターンの準備する

                    break;
                case _010_Enum.FlagPowerFix.hold:
                    //徴兵など次ターンの準備する

                    break;
                case _010_Enum.FlagPowerFix.warlike:
                    //Power構造体からdiplomacyを取得
                    //Power構造体からhomeを取得
                    //Power構造体からfixを取得
                    //現在のdiploを取得
                    //現在のenemy_powerを取得
                    //現在のleagueを取得

                    ///どの隣国を狙うかチェック
                    //現在のhomeを考慮
                    //現在のdiploを考慮
                    //現在のenemy_powerを考慮
                    //現在のleagueを考慮

                    //攻め入る

                    //徴兵など次ターンの準備する

                    break;
                case _010_Enum.FlagPowerFix.freeze:
                    break;
                default:
                    break;
            }

            return;
        }
    }
}
