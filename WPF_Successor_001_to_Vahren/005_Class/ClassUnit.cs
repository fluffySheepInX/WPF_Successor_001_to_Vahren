using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;
using System.Windows;
using Point = System.Windows.Point;
using WPF_Successor_001_to_Vahren._006_ClassStatic;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassUnit
    {
        public ClassUnit ShallowCopy()
        {
            return (ClassUnit)MemberwiseClone();
        }
        public ClassUnit DeepCopy()
        {
            ClassUnit cu = ShallowCopy();
            if (cu.Formation != null)
            {
                var fo = new ClassFormation();
                fo.Formation = cu.Formation.Formation;
                fo.Id = cu.Formation.Id;
                cu.Formation = fo;
            }
            // 配列など参照型のデータを新規作成して元の値をコピーする
            cu.SkillName = new List<string>(cu.SkillName);
            cu.Skill = new List<ClassSkill>(cu.Skill);
            if (cu.OrderPosiSkill != null)
            {
                var ops = new Dictionary<int, Point>(cu.OrderPosiSkill);
                cu.OrderPosiSkill = ops;
            }
            if (cu.VecMoveSkill != null)
            {
                var vms = new Dictionary<int, Point>(cu.VecMoveSkill);
                cu.VecMoveSkill = vms;
            }
            if (cu.NowPosiSkill != null)
            {
                var nps = new Dictionary<int, Point>(cu.NowPosiSkill);
                cu.NowPosiSkill = nps;
            }
            return cu;
        }
        #region Formation
        private ClassFormation _formation = new ClassFormation();

        public ClassFormation Formation
        {
            get { return _formation; }
            set { _formation = value; }
        }

        #endregion
        #region ID
        private long id;

        public long ID
        {
            get { return id; }
            set { id = value; }
        }

        #endregion
        #region IsLeader
        private bool _isLeader = false;
        public bool IsLeader
        {
            get { return _isLeader; }
            set { _isLeader = value; }
        }
        #endregion

        #region IsSelect
        private bool _isSelect = false;
        public bool IsSelect
        {
            get { return _isSelect; }
            set { _isSelect = value; }
        }
        #endregion

        #region IsDone
        private bool _isDone = false;
        public bool IsDone
        {
            get { return _isDone; }
            set { _isDone = value; }
        }
        #endregion

        #region NameTag
        private string _nameTag = string.Empty;
        public string NameTag
        {
            get { return _nameTag; }
            set { _nameTag = value; }
        }
        #endregion
        #region Name
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion
        #region Help
        private string _help = string.Empty;
        public string Help
        {
            get { return _help; }
            set { _help = value; }
        }
        #endregion
        #region Text
        private string text = string.Empty;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        #endregion
        #region Race
        private string _race = string.Empty;
        public string Race
        {
            get { return _race; }
            set { _race = value; }
        }
        #endregion
        #region Class
        private string _class = string.Empty;
        public string Class
        {
            get { return _class; }
            set { _class = value; }
        }
        #endregion
        #region Image
        private string _image = string.Empty;
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }
        #endregion
        #region Dead
        private string _dead = string.Empty;
        public string Dead
        {
            get { return _dead; }
            set { _dead = value; }
        }
        #endregion
        #region Retreat
        private string _retreat = string.Empty;
        public string Retreat
        {
            get { return _retreat; }
            set { _retreat = value; }
        }
        #endregion
        #region Join
        private string _join = string.Empty;
        public string Join
        {
            get { return _join; }
            set { _join = value; }
        }
        #endregion
        #region Face
        private string _face = string.Empty;
        public string Face
        {
            get { return _face; }
            set { _face = value; }
        }
        #endregion
        #region Voice_type
        private string _voice_type = string.Empty;
        public string Voice_type
        {
            get { return _voice_type; }
            set { _voice_type = value; }
        }
        #endregion
        #region Gender
        private _010_Enum.Gender gender = _010_Enum.Gender.Neuter;
        public _010_Enum.Gender Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        #endregion
        #region Talent
        private string _talent = string.Empty;
        public string Talent
        {
            get { return _talent; }
            set { _talent = value; }
        }
        #endregion
        #region Friend
        private string _friend = string.Empty;
        public string Friend
        {
            get { return _friend; }
            set { _friend = value; }
        }
        #endregion
        #region Merce
        private string _merce = string.Empty;
        public string Merce
        {
            get { return _merce; }
            set { _merce = value; }
        }
        #endregion
        #region Staff
        private string _staff = string.Empty;
        public string Staff
        {
            get { return _staff; }
            set { _staff = value; }
        }
        #endregion
        #region InitMember
        private string _initMember = string.Empty;
        public string InitMember
        {
            get { return _initMember; }
            set { _initMember = value; }
        }
        #endregion
        #region Enemy
        private string _enemy = string.Empty;
        public string Enemy
        {
            get { return _enemy; }
            set { _enemy = value; }
        }
        #endregion
        #region Level
        private int _level;
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        #endregion
        #region Level_max
        private int _level_max;
        public int Level_max
        {
            get { return _level_max; }
            set { _level_max = value; }
        }
        #endregion
        #region Price
        private int _price;
        public int Price
        {
            get { return _price; }
            set { _price = value; }
        }
        #endregion
        #region Cost
        private int _cost;
        public int Cost
        {
            get { return _cost; }
            set { _cost = value; }
        }
        #endregion
        #region Medical
        private int _medical;
        public int Medical
        {
            get { return _medical; }
            set { _medical = value; }
        }
        #endregion
        #region HasExp
        private int _hasExp;
        public int HasExp
        {
            get { return _hasExp; }
            set { _hasExp = value; }
        }
        #endregion
        #region Hp
        private int _hp;
        public int Hp
        {
            get { return _hp; }
            set { _hp = value; }
        }
        #endregion
        #region Mp
        private int _mp;
        public int Mp
        {
            get { return _mp; }
            set { _mp = value; }
        }
        #endregion
        #region Attack
        private int _attack;
        public int Attack
        {
            get { return _attack; }
            set { _attack = value; }
        }
        #endregion
        #region Defense
        private int _defense;
        public int Defense
        {
            get { return _defense; }
            set { _defense = value; }
        }
        #endregion
        #region Magic
        private int _magic;
        public int Magic
        {
            get { return _magic; }
            set { _magic = value; }
        }
        #endregion
        #region MagDef
        private int _magDef;
        public int MagDef
        {
            get { return _magDef; }
            set { _magDef = value; }
        }
        #endregion
        #region Speed
        private double _speed;
        public double Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        #endregion
        #region Dext
        private int _dext;
        public int Dext
        {
            get { return _dext; }
            set { _dext = value; }
        }
        #endregion
        #region Move
        private int _move;
        public int Move
        {
            get { return _move; }
            set { _move = value; }
        }
        #endregion
        #region Hprec
        private int _hprec;
        public int Hprec
        {
            get { return _hprec; }
            set { _hprec = value; }
        }
        #endregion
        #region Mprec
        private int _mprec;
        public int Mprec
        {
            get { return _mprec; }
            set { _mprec = value; }
        }
        #endregion
        #region Exp
        private int _exp;
        public int Exp
        {
            get { return _exp; }
            set { _exp = value; }
        }
        #endregion
        #region Exp_mul
        private int _exp_mul;
        public int Exp_mul
        {
            get { return _exp_mul; }
            set { _exp_mul = value; }
        }
        #endregion
        #region Heal_max
        private int _heal_max;
        public int Heal_max
        {
            get { return _heal_max; }
            set { _heal_max = value; }
        }
        #endregion
        #region Summon_max
        private int _summon_max;
        public int Summon_max
        {
            get { return _summon_max; }
            set { _summon_max = value; }
        }
        #endregion
        #region No_knock
        private int _no_knock;
        public int No_knock
        {
            get { return _no_knock; }
            set { _no_knock = value; }
        }
        #endregion
        #region Loyal
        private int _loyal;
        public int Loyal
        {
            get { return _loyal; }
            set { _loyal = value; }
        }
        #endregion
        #region Alive_per
        private int _alive_per;
        public int Alive_per
        {
            get { return _alive_per; }
            set { _alive_per = value; }
        }
        #endregion
        #region Escape_range
        private int _escape_range;
        public int Escape_range
        {
            get { return _escape_range; }
            set { _escape_range = value; }
        }
        #endregion
        #region SkillName
        private List<string> skillName = new List<string>();

        public List<string> SkillName
        {
            get { return skillName; }
            set { skillName = value; }
        }
        #endregion
        #region Skill
        private List<ClassSkill> skill = new List<ClassSkill>();

        public List<ClassSkill> Skill
        {
            get { return skill; }
            set { skill = value; }
        }
        #endregion
        #region Finance
        private int _finance;
        public int Finance
        {
            get { return _finance; }
            set { _finance = value; }
        }
        #endregion
        #region MoveType
        private string _moveType = string.Empty;
        public string MoveType
        {
            get { return _moveType; }
            set { _moveType = value; }
        }
        #endregion

        #region FlagMove
        private bool flagMove;

        public bool FlagMove
        {
            get { return flagMove; }
            set { flagMove = value; }
        }
        #endregion
        #region FlagMoving
        private bool flagMoving;
        public bool FlagMoving
        {
            get { return flagMoving; }
            set { flagMoving = value; }
        }
        #endregion
        #region FlagMoveDispose
        private bool flagMoveDispose = false;

        public bool FlagMoveDispose
        {
            get { return flagMoveDispose; }
            set { flagMoveDispose = value; }
        }
        #endregion
        //#region FlagMoveDisposeOK
        //private bool flagMoveDisposeOK = false;

        //public bool FlagMoveDisposeOK
        //{
        //    get { return flagMoveDisposeOK; }
        //    set { flagMoveDisposeOK = value; }
        //}
        //#endregion
        #region NowPosiLeft
        private Point nowPosiLeft;

        public Point NowPosiLeft
        {
            get { return nowPosiLeft; }
            set { nowPosiLeft = value; }
        }
        #endregion
        public Point GetNowPosiCenter()
        {
            return new Point(NowPosiLeft.X + (ClassStaticBattle.yokoUnit / 2), NowPosiLeft.Y + (ClassStaticBattle.TakasaUnit / 2));
        }
        #region OrderPosiLeft
        private Point orderPosiLeft;

        public Point OrderPosiLeft
        {
            get { return orderPosiLeft; }
            set { orderPosiLeft = value; }
        }
        #endregion
        #region NowPosiCenter
        public Point NowPosiCenter
        {
            get { return new Point(nowPosiLeft.X + (ClassStaticBattle.yokoUnit / 2), nowPosiLeft.Y + (ClassStaticBattle.TakasaUnit / 2)); }
        }
        #endregion
        #region OrderPosiCenter
        public Point OrderPosiCenter
        {
            get { return new Point(orderPosiLeft.X + (ClassStaticBattle.yokoUnit / 2), orderPosiLeft.Y + (ClassStaticBattle.TakasaUnit / 2)); }
        }
        #endregion
        #region VecMove
        private Point vecMove;
        public Point VecMove
        {
            get { return vecMove; }
            set { vecMove = value; }
        }
        #endregion

        #region FlagMoveSkill
        private bool flagMoveSkill;

        public bool FlagMoveSkill
        {
            get { return flagMoveSkill; }
            set { flagMoveSkill = value; }
        }

        #endregion
        #region FlagMovingSkill
        private bool flagMovingSkill;
        public bool FlagMovingSkill
        {
            get { return flagMovingSkill; }
            set { flagMovingSkill = value; }
        }
        #endregion
        #region NowPosi
        private Dictionary<int, Point> nowPosiSkill = new Dictionary<int, Point>();

        public Dictionary<int, Point> NowPosiSkill
        {
            get { return nowPosiSkill; }
            set { nowPosiSkill = value; }
        }
        #endregion
        #region OrderPosiSkill
        private Dictionary<int, Point> orderPosiSkill = new Dictionary<int, Point>();

        public Dictionary<int, Point> OrderPosiSkill
        {
            get { return orderPosiSkill; }
            set { orderPosiSkill = value; }
        }
        #endregion
        #region VecMoveSkill
        private Dictionary<int, Point> vecMoveSkill = new Dictionary<int, Point>();
        public Dictionary<int, Point> VecMoveSkill
        {
            get { return vecMoveSkill; }
            set { vecMoveSkill = value; }
        }
        #endregion

    }
}
