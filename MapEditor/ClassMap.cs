using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapEditor
{
    public class ClassMap
    {
        public ClassMap(string field, List<string> build, int flag, string unit, string direction, string formation)
        {
            this.field = field;
            this.build = build;
            this.flag = flag;
            this.unit = unit;
            this.direction = direction;
            this.formation = formation;
        }
        public ClassMap() { }
        /// <summary>
        /// 床画像
        /// </summary>
        public string field { get; set; } = string.Empty;
        /// <summary>
        /// 城壁や矢倉など
        /// </summary>
        public List<string> build { get; set; } = new List<string>();
        /// <summary>
        /// 部隊チップの種別
        /// flag = 0 なら「ユニットの識別名」として扱う。
        /// flag = 1 なら「@文字変数」として扱う。
        /// flag = 2 なら「特殊な文字列」として扱う。
        /// 
        /// ユニットの識別名 同名のunit/class構造体ユニットが配置されます
        /// 
        /// @文字変数 @が接頭辞の文字列は文字変数と見なされます。代入スクリプトで防衛施設を変化できます
        /// 
        /// 特殊な文字列
        /// 「@」 防衛部隊の位置。
        /// 「@@」 侵攻部隊の位置。
        /// 「@ESC@」 城兵の退却位置になります。
        /// 
        /// </summary>
        public int flag { get; set; } = 0;
        /// <summary>
        /// 部隊
        /// </summary>
        public string unit { get; set; } = string.Empty;
        /// <summary>
        /// 方向
        /// </summary>
        public string direction { get; set; } = string.Empty;
        /// <summary>
        /// 陣形
        /// </summary>
        public string formation { get; set; } = string.Empty;

    }
}
