using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassDiplomacy
    {
        public ClassDiplomacy ShallowCopy()
        {
            return (ClassDiplomacy)MemberwiseClone();
        }
        public ClassDiplomacy DeepCopy()
        {
            ClassDiplomacy cp = ShallowCopy();
            // 配列など参照型のデータを新規作成して元の値をコピーする
            cp.Diplo = new List<(string, string, int)>(cp.Diplo);
            cp.League = new List<(string, string, int)>(cp.League);
            cp.EnemyPower = new List<(string, string, string, int)>(cp.EnemyPower);
            cp.OneWayLove = new List<(string, string, int)>(cp.OneWayLove);
            cp.Cold = new List<(string, string, int)>(cp.Cold);
            return cp;
        }

        #region Diplo
        private List<(string, string, int)> diplo = new List<(string, string, int)>();
        public List<(string, string, int)> Diplo
        {
            get { return diplo; }
            set { diplo = value; }
        }
        #endregion

        #region League
        private List<(string, string, int)> league = new List<(string, string, int)>();
        public List<(string, string, int)> League
        {
            get { return league; }
            set { league = value; }
        }
        #endregion
        #region EnemyPower
        private List<(string, string, string, int)> enemyPower = new List<(string, string, string, int)>();
        public List<(string, string, string, int)> EnemyPower
        {
            get { return enemyPower; }
            set { enemyPower = value; }
        }
        #endregion
        #region OneWayLove
        private List<(string, string, int)> oneWayLove = new List<(string, string, int)>();
        public List<(string, string, int)> OneWayLove
        {
            get { return oneWayLove; }
            set { oneWayLove = value; }
        }
        #endregion
        #region Cold
        private List<(string, string, int)> cold = new List<(string, string, int)>();
        public List<(string, string, int)> Cold
        {
            get { return cold; }
            set { cold = value; }
        }
        #endregion
    }
}
