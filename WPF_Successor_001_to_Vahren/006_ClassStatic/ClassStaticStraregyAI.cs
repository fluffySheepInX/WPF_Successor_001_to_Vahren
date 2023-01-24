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

                    ////ランダム(補正有り)でターゲットとなる国を選ぶ
                    //友好度50のリストを作る
                    Dictionary<string, int> baseTargetPowerList = new Dictionary<string, int>();
                    foreach (var item in classGameStatus.NowListPower.Where(x => x.Index != classPower.Index))
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

                    //ランダムで値を取得して、絶対値と比較
                    Random random = new Random(DateTime.Now.Second);
                    List<ClassPower> targetPowers = new List<ClassPower>();
                    foreach (var item in absTargetPowerList)
                    {
                        //範囲内ならターゲットの国とする
                        if (item.Value < random.Next(1, 100 + 1) == true)
                        {
                            continue;
                        }
                        var tar = classGameStatus.ListPower.Where(x => x.NameTag == item.Key).FirstOrDefault();
                        if (tar == null) continue;
                        targetPowers.Add(tar);
                        //本来ならターゲットは複数あっても良いが、今は一つに絞る
                        break;//複数の時はこれを外す
                    }

                    //ターゲットとの国境都市を取得
                    List<ClassSpot> classSpot = new List<ClassSpot>();



                    //ターゲットとの国境都市があるかチェック
                    if (classSpot.Count == 0)
                    {
                        //ターゲットとの国境都市が無い
                    }
                    else
                    {
                        //ターゲットとの国境都市で徴兵

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
