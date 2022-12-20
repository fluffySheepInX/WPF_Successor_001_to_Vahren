﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassGameStatus
    {
        #region IDCount
        private long iDCount = 0;
        public long IDCount
        {
            get { return iDCount; }
            set 
            { 
                iDCount = value; 
            }
        }
        #endregion
        public void SetIDCount()
        {
            iDCount += 1;
        }
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
        #region ListObject
        private List<ClassObjectMapTip> _listObject = new List<ClassObjectMapTip>();
        public List<ClassObjectMapTip> ListObject
        {
            get { return _listObject; }
            set { _listObject = value; }
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
        private ClassBattle _classBattle = new ClassBattle();
        public ClassBattle ClassBattle
        {
            get { return _classBattle; }
            set { _classBattle = value; }
        }
        #endregion
        #region TaskBattleSkill
        private (Task, CancellationTokenSource) taskBattleSkill;
        public (Task, CancellationTokenSource) TaskBattleSkill
        {
            get { return taskBattleSkill; }
            set { taskBattleSkill = value; }
        }
        #endregion
        #region TaskBattleSkillDef
        private (Task, CancellationTokenSource) taskBattleSkillDef;
        public (Task, CancellationTokenSource) TaskBattleSkillDef
        {
            get { return taskBattleSkillDef; }
            set { taskBattleSkillDef = value; }
        }
        #endregion
        #region TaskBattleMoveAsync
        private (Task, CancellationTokenSource) taskBattleMoveAsync;
        public (Task, CancellationTokenSource) TaskBattleMoveAsync
        {
            get { return taskBattleMoveAsync; }
            set { taskBattleMoveAsync = value; }
        }
        #endregion
        #region TaskBattleMoveDefAsync
        private (Task, CancellationTokenSource) taskBattleMoveDefAsync;
        public (Task, CancellationTokenSource) TaskBattleMoveDefAsync
        {
            get { return taskBattleMoveDefAsync; }
            set { taskBattleMoveDefAsync = value; }
        }
        #endregion

        #region IsDrag
        private bool _isDrag = false;
        public bool IsDrag
        {
            get { return _isDrag; }
            set { _isDrag = value; }
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

        #region WindowStrategyMenu
        private UserControl005_StrategyMenu _windowStrategyMenu = new UserControl005_StrategyMenu();
        public UserControl005_StrategyMenu WindowStrategyMenu
        {
            get { return _windowStrategyMenu; }
            set { _windowStrategyMenu = value; }
        }
        #endregion

        #region CommonWindow
        private CommonWindow? commonWindow;
        public CommonWindow? CommonWindow
        {
            get { return commonWindow; }
            set { commonWindow = value; }
        }
        #endregion

        #region TextWindow
        private FrameworkElement? textWindow = null;
        public FrameworkElement? TextWindow
        {
            get { return textWindow; }
            set { textWindow = value; }
        }
        #endregion
    }
}
