using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._010_Enum;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassObjectMapTip
    {
        #region NameTag
        private string _nameTag = string.Empty;
        public string NameTag
        {
            get { return _nameTag; }
            set { _nameTag = value; }
        }
        #endregion

        #region Type
        private MapTipObjectType _type;
        public MapTipObjectType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        #endregion

        #region NoWall2
        private int _noWall2 = 0;
        public int NoWall2
        {
            get { return _noWall2; }
            set { _noWall2 = value; }
        }
        #endregion
    }
}
