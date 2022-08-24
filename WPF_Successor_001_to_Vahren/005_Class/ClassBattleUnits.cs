using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassBattleUnits
    {
        #region SortieUnitGroup
        private List<ClassHorizontalUnit> _sortieUnitGroup = new List<ClassHorizontalUnit>();
        public List<ClassHorizontalUnit> SortieUnitGroup
        {
            get { return _sortieUnitGroup; }
            set { _sortieUnitGroup = value; }
        }
        #endregion

        #region DefUnitGroup
        private List<ClassHorizontalUnit> _defUnitGroup = new List<ClassHorizontalUnit>();
        public List<ClassHorizontalUnit> DefUnitGroup
        {
            get { return _defUnitGroup; }
            set { _defUnitGroup = value; }
        }
        #endregion

        #region NeutralUnitGroup
        /// <summary>
        /// 中立勢力を出したい時用に
        /// </summary>
        private List<ClassHorizontalUnit> _neutralUnitGroup = new List<ClassHorizontalUnit>();
        /// <summary>
        /// 中立勢力を出したい時用に
        /// </summary>
        public List<ClassHorizontalUnit> NeutralUnitGroup
        {
            get { return _neutralUnitGroup; }
            set { _neutralUnitGroup = value; }
        }
        #endregion

    }
}
