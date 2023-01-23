using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassDiplomacy
    {
        #region Diplo
        private (string, string, int) diplo;
        public (string, string, int) Diplo
        {
            get { return diplo; }
            set { diplo = value; }
        }
        #endregion

        #region League
        private (string, string, int) league;
        public (string, string, int) League
        {
            get { return league; }
            set { league = value; }
        }
        #endregion
        #region EnemyPower
        private (string, string, string, int) enemyPower;
        public (string, string, string, int) EnemyPower
        {
            get { return enemyPower; }
            set { enemyPower = value; }
        }
        #endregion
        #region OneWayLove
        private (string, string, int) oneWayLove;
        public (string, string, int) OneWayLove
        {
            get { return oneWayLove; }
            set { oneWayLove = value; }
        }
        #endregion
        #region Cold
        private (string, string, int) cold;
        public (string, string, int) Cold
        {
            get { return cold; }
            set { cold = value; }
        }
        #endregion
    }
}
