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
        public static void Thinking(ClassGameStatus classGameStatus, ClassPower classPower)
        {
            if (classGameStatus == null) return;

            ////状態によって隣国へ攻め入る
            ///状態をチェック
            switch (classPower.Fix)
            {
                case _010_Enum.FlagPowerFix.on:
                    //交戦国をチェック

                    //国境都市を取得
                    ClassSpot classSpot;
                    //国境都市で徴兵


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
