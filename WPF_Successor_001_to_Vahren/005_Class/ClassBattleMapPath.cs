using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassBattleMapPath
    {
        public ClassBattleMapPath() { }

        #region KougekiOrBouei
        private string kougekiOrBouei = string.Empty;
        public string KougekiOrBouei
        {
            get { return kougekiOrBouei; }
            set { kougekiOrBouei = value; }
        }
        #endregion

        #region Col
        private int col = -1;
        public int Col
        {
            get { return col; }
            set { col = value; }
        }
        #endregion

        #region Row
        private int row = -1;
        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        #endregion
    }
}
