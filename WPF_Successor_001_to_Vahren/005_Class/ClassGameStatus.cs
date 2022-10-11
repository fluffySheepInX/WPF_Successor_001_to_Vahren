using System;
using System.Collections.Generic;
using System.Windows;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassGameStatus
    {
        #region Camera
        private Point _camera;
        public Point Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }
        #endregion

        #region SelectionCityPoint
        private Point _selectionCityPoint;
        public Point SelectionCityPoint
        {
            get { return _selectionCityPoint; }
            set { _selectionCityPoint = value; }
        }
        #endregion

        #region SelectionPowerAndCity
        private ClassPowerAndCity _selectionPowerAndCity = new ClassPowerAndCity();
        public ClassPowerAndCity SelectionPowerAndCity
        {
            get { return _selectionPowerAndCity; }
            set { _selectionPowerAndCity = value; }
        }
        #endregion

        #region GridCityWidthAndHeight
        private Point _gridCityWidthAndHeight = new Point(100,100);
        public Point GridCityWidthAndHeight
        {
            get { return _gridCityWidthAndHeight; }
            set { _gridCityWidthAndHeight = value; }
        }
        #endregion

        #region ListClassMapBattle
        private List<ClassMapBattle> listClassMapBattle = new List<ClassMapBattle>();

        public List<ClassMapBattle> ListClassMapBattle
        {
            get { return listClassMapBattle; }
            set { listClassMapBattle = value; }
        }

        #endregion

        #region ListSpot
        private List<ClassSpot> _listSpot = new List<ClassSpot>();
        public List<ClassSpot> AllListSpot
        {
            get { return _listSpot; }
            set { _listSpot = value; }
        }
        #endregion
        #region ListPower
        private List<ClassPower> _listPower = new List<ClassPower>();
        public List<ClassPower> ListPower
        {
            get { return _listPower; }
            set { _listPower = value; }
        }
        #endregion
        #region ListUnit
        private List<ClassUnit> _listUnit = new List<ClassUnit>();
        public List<ClassUnit> ListUnit
        {
            get { return _listUnit; }
            set { _listUnit = value; }
        }
        #endregion
        #region ListSkill
        private List<ClassSkill> _listSkill = new List<ClassSkill>();
        public List<ClassSkill> ListSkill
        {
            get { return _listSkill; }
            set { _listSkill = value; }
        }
        #endregion

        #region ListEvent
        private List<ClassEvent> listEvent = new List<ClassEvent>();
        public List<ClassEvent> ListEvent
        {
            get { return listEvent; }
            set { listEvent = value; }
        }
        #endregion

        #region NowTurn
        private int nowTurn = -1;
        public int NowTurn
        {
            get { return nowTurn; }
            set { nowTurn = value; }
        }
        #endregion

        #region NowCountPower
        private int nowCountPower = -1;
        public int NowCountPower
        {
            get { return nowCountPower; }
            set { nowCountPower = value; }
        }
        #endregion

        #region NowCountSpot
        private int nowCountSpot = -1;
        public int NowCountSelectionPowerSpot
        {
            get { return nowCountSpot; }
            set { nowCountSpot = value; }
        }
        #endregion

        #region ClassBattleUnits
        private ClassBattle _classBattleUnits = new ClassBattle();
        public ClassBattle ClassBattleUnits
        {
            get { return _classBattleUnits; }
            set { _classBattleUnits = value; }
        }
        #endregion

        #region IsMouse
        private bool _isMouse;
        public bool IsMouse
        {
            get { return _isMouse; }
            set { _isMouse = value; }
        }
        #endregion
        #region StartPoint
        private Point _startPoint;
        public Point StartPoint
        {
            get { return _startPoint; }
            set { _startPoint = value; }
        }
        #endregion
        #region CurrentPoint
        private Point _currentPoint;
        public Point CurrentPoint
        {
            get { return _currentPoint; }
            set { _currentPoint = value; }
        }
        #endregion
        #region StartPointBattle
        private Point _startPointBattle;
        public Point StartPointBattle
        {
            get { return _startPointBattle; }
            set { _startPointBattle = value; }
        }
        #endregion
        #region CurrentPointBattle
        private Point _currentPointBattle;
        public Point CurrentPointBattle
        {
            get { return _currentPointBattle; }
            set { _currentPointBattle = value; }
        }
        #endregion


        #region WindowStrategyMenu
        private Page005_StrategyMenu _windowStrategyMenu = new Page005_StrategyMenu();

        public Page005_StrategyMenu WindowStrategyMenu
        {
            get { return _windowStrategyMenu; }
            set { _windowStrategyMenu = value; }
        }

        #endregion

    }
}
