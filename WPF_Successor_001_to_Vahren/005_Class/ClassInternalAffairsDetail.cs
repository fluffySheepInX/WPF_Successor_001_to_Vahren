using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassInternalAffairsDetail
    {
        public string NameTag { get; set; } = string.Empty;

        // 画面上に表示される名前
        public string Title { get; set; } = string.Empty;

        // 100px*512pxで画面上に表示
        public string Image { get; set; } = string.Empty;

        // help
        public string Help { get; set; } = "-1";
        // cost
        public int Cost { get; set; } = -1;

        // 前提とする内政（これが実行されてなければ、この内政は実行不可能
        public string Premise { get; set; } = "-1";
        // 前提とする内政の必要回数（5なら5回実行が必要
        public int PremiseNumber { get; set; } = -1;

        // 収入補正
        // %分、上昇する
        // -1だと実行されない
        public double IncomeCorrection { get; set; } = -1;

        // 収入
        // 数値分、上昇する
        // -1だと実行されない
        public int Income { get; set; } = 1000;

        // スキル付与(出撃時と防衛時、全部隊に
        // スキルのタグを指定する
        // -1だと実行されない
        public string Skill { get; set; } = "-1";

        // ユニット追加(出撃時と防衛時、操作不可、自動で移動と攻撃
        public string Unit { get; set; } = "-1";

        // 上昇させる能力値
        // -1だと実行されない
        public string Ability { get; set; } = "-1";

        // 能力値上昇をさせる時(出撃時と防衛時どちらか
        // -1だと実行されない
        public string AbilityTiming { get; set; } = "-1";

        // 能力値上昇値
        // -1だと実行されない
        public int AbilityUp { get; set; } = -1;
    }
}
