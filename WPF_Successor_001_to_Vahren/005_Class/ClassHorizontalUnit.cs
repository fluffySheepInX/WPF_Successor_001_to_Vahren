using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassHorizontalUnit
    {
        #region FlagDisplay
        private bool _flagDisplay = true;
        public bool FlagDisplay
        {
            get { return _flagDisplay; }
            set { _flagDisplay = value; }
        }
        #endregion

        #region ListClassUnit
        private List<ClassUnit> _listClassUnit = new List<ClassUnit>();

        public List<ClassUnit> ListClassUnit
        {
            get { return _listClassUnit; }
            set { _listClassUnit = value; }
        }

        #region Spot
        private ClassSpot _spot = new ClassSpot();

        public ClassSpot Spot
        {
            get { return _spot; }
            set { _spot = value; }
        }

        #endregion

        #endregion
    }
}
