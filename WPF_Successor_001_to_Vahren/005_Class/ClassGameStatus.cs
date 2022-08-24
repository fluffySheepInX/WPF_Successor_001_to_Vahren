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

        #region ListEvent
        private List<ClassEvent> listEvent = new List<ClassEvent>();
        public List<ClassEvent> ListEvent
        {
            get { return listEvent; }
            set { listEvent = value; }
        }
        #endregion

        #region ClassBattleUnits
        private ClassBattleUnits _classBattleUnits = new ClassBattleUnits();
        public ClassBattleUnits ClassBattleUnits
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
