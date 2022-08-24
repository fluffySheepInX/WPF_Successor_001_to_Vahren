using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassPowerAndCity
    {
        public ClassPowerAndCity(ClassPower classPower, ClassSpot classSpot)
        {
            this.ClassPower = classPower;
            this.ClassSpot = classSpot;
        }
        public ClassPowerAndCity()
        {
        }

        #region ClassPower
        private ClassPower _classPower = new ClassPower();
        public ClassPower ClassPower
        {
            get { return _classPower; }
            set { _classPower = value; }
        }
        #endregion

        #region ClassSpot
        private ClassSpot _classSpot = new ClassSpot();
        public ClassSpot ClassSpot
        {
            get { return _classSpot; }
            set { _classSpot = value; }
        }
        #endregion
    }
}
