using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassGameStatus
    {
        #region NowSituation
        private _010_Enum.Situation _nowSituation = _010_Enum.Situation.Title;
        public _010_Enum.Situation NowSituation
        {
            get
            {
                return _nowSituation;
            }
            set
            {
                _nowSituation = value;
            }
        }
        #endregion

        #region IDCount_セットで使うこと！
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
        #endregion
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

        #region NumberScenarioSelection
        private int _numberScenarioSelection;
        public int NumberScenarioSelection
        {
            get { return _numberScenarioSelection; }
            set { _numberScenarioSelection = value; }
        }
        #endregion

        #region ファイルから読み込んだオリジナルのデータ
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
        #region ClassContext
        private ClassContext classContext = new ClassContext();
        public ClassContext ClassContext
        {
            get { return classContext; }
            set { classContext = value; }
        }
        #endregion
        #region ClassDiplomacy
        private ClassDiplomacy classDiplomacy = new ClassDiplomacy();
        public ClassDiplomacy ClassDiplomacy
        {
            get { return classDiplomacy; }
            set { classDiplomacy = value; }
        }
        #endregion
        #region ListClassScenario
        private List<ClassScenarioInfo> _listClassScenarioInfo = new List<ClassScenarioInfo>();
        public List<ClassScenarioInfo> ListClassScenarioInfo
        {
            get { return _listClassScenarioInfo; }
            set { _listClassScenarioInfo = value; }
        }
        #endregion
        #endregion

        #region 元データからシナリオ用にコピーして使うデータ
        #region NowListSpot
        private List<ClassSpot> _nowlistSpot = new List<ClassSpot>();
        public List<ClassSpot> NowListSpot
        {
            get { return _nowlistSpot; }
            set { _nowlistSpot = value; }
        }
        #endregion
        #region NowListPower
        private List<ClassPower> _nowlistPower = new List<ClassPower>();
        public List<ClassPower> NowListPower
        {
            get { return _nowlistPower; }
            set { _nowlistPower = value; }
        }
        #endregion
        #region NowListUnit
        private List<ClassUnit> _nowlistUnit = new List<ClassUnit>();
        public List<ClassUnit> NowListUnit
        {
            get { return _nowlistUnit; }
            set { _nowlistUnit = value; }
        }
        #endregion
        #region NowClassDiplomacy
        private ClassDiplomacy nowClassDiplomacy = new ClassDiplomacy();
        public ClassDiplomacy NowClassDiplomacy
        {
            get { return nowClassDiplomacy; }
            set { nowClassDiplomacy = value; }
        }
        #endregion
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
        private List<(Task, CancellationTokenSource)> taskBattleMoveAsync = new List<(Task, CancellationTokenSource)>();
        public List<(Task, CancellationTokenSource)> TaskBattleMoveAsync
        {
            get { return taskBattleMoveAsync; }
            set { taskBattleMoveAsync = value; }
        }
        #endregion
        #region TaskBattleMoveDefAsync
        private List<(Task, CancellationTokenSource)> taskBattleMoveDefAsync = new List<(Task, CancellationTokenSource)>();
        public List<(Task, CancellationTokenSource)> TaskBattleMoveDefAsync
        {
            get { return taskBattleMoveDefAsync; }
            set { taskBattleMoveDefAsync = value; }
        }
        #endregion

        #region AiRoot
        private Dictionary<long, List<Point>> aiRoot = new Dictionary<long, List<Point>>();
        public Dictionary<long,List<Point>> AiRoot
        {
            get { return aiRoot; }
            set { aiRoot = value; }
        }
        #endregion

        #region 戦闘に関する数字
        #region BattleThread
        private int battleThread = 1;
        public int BattleThread
        {
            get { return battleThread; }
            set { battleThread = value; }
        }
        #endregion
        #region NumberSleep
        private int numberSleep = (int)(Math.Floor(((double)1 / 60) * 1000));
        public int NumberSleep
        {
            get { return numberSleep; }
            set { numberSleep = value; }
        }
        #endregion
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

        #region WorldMap
        private UserControl060_WorldMap? _worldMap = null;
        public UserControl060_WorldMap? WorldMap
        {
            get { return _worldMap; }
            set { _worldMap = value; }
        }
        #endregion

        #region WindowStrategyMenu
        private UserControl005_StrategyMenu? _windowStrategyMenu = null;
        public UserControl005_StrategyMenu? WindowStrategyMenu
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
