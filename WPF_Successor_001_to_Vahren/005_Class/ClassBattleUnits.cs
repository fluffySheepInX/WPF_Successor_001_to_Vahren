using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using WPF_Successor_001_to_Vahren._010_Enum;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassBattle
    {
        #region BattleSpot
        // ヴァーレントゥーガの storeBattleSpot, storeAttackPower, storeDefensePower を参考にする
        // 直前に侵攻された「領地の識別子」
        private string _BattleSpot = string.Empty;
        public string BattleSpot
        {
            get { return _BattleSpot; }
            set { _BattleSpot = value; }
        }
        /// <summary>
        /// 出撃元領地
        /// </summary>
        private string _sortieSpot = string.Empty;
        /// <summary>
        /// 出撃元領地
        /// </summary>
        public string SortieSpot
        {
            get { return _sortieSpot; }
            set { _sortieSpot = value; }
        }

        // 直前の侵攻先の攻撃側ホストの「勢力の識別子」
        private string _AttackPower = string.Empty;
        public string AttackPower
        {
            get { return _AttackPower; }
            set { _AttackPower = value; }
        }
        // 直前の侵攻先の防衛側ホストの「勢力の識別子」
        private string _DefensePower = string.Empty;
        public string DefensePower
        {
            get { return _DefensePower; }
            set { _DefensePower = value; }
        }
        #endregion

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

        #region ClassMapBattle
        private ClassMapBattle? classMapBattle = null;
        public ClassMapBattle? ClassMapBattle
        {
            get { return classMapBattle; }
            set { classMapBattle = value; }
        }
        #endregion

        #region ListBuildingAlive
        private List<Rectangle> _listBuildingAlive = new List<Rectangle>();
        public List<Rectangle> ListBuildingAlive
        {
            get { return _listBuildingAlive; }
            set { _listBuildingAlive = value; }
        }
        #endregion

        #region BattleWhichIsThePlayer
        private BattleWhichIsThePlayer _battleWhichIsThePlayer;
        public BattleWhichIsThePlayer BattleWhichIsThePlayer
        {
            get { return _battleWhichIsThePlayer; }
            set { _battleWhichIsThePlayer = value; }
        }
        #endregion
    }
}
