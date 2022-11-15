using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassTestBattle
    {
        private string player = "kougeki";
        public string Player
        {
            get { return player; }
            set { player = value; }
        }

        private string map = "";
        public string Map
        {
            get { return map; }
            set { map = value; }
        }

        #region ListMember
        private List<(string, int)> _listMember = new List<(string, int)>();
        public List<(string, int)> ListMember
        {
            get { return _listMember; }
            set { _listMember = value; }
        }
        #endregion

        #region UnitGroup
        private List<ClassHorizontalUnit> _unitGroup = new List<ClassHorizontalUnit>();
        public List<ClassHorizontalUnit> UnitGroup
        {
            get { return _unitGroup; }
            set { _unitGroup = value; }
        }
        #endregion


    }
}
