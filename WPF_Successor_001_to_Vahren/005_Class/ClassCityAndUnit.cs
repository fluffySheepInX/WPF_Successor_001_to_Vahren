using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassCityAndUnit
    {
        #region ClassPowerAndCity
        private ClassPowerAndCity _classPowerAndCity = new ClassPowerAndCity();
        public ClassPowerAndCity ClassPowerAndCity
        {
            get { return _classPowerAndCity; }
            set { _classPowerAndCity = value; }
        }
        #endregion

        #region ClassUnit
        private ClassUnit _classUnit = new ClassUnit();
        public ClassUnit ClassUnit
        {
            get { return _classUnit; }
            set { _classUnit = value; }
        }
        #endregion
    }
}
