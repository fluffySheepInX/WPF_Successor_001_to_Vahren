using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;
using System.Windows;
using Point = System.Windows.Point;

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

        #region IsReader
        private bool _isReader = false;
        public bool IsReader
        {
            get { return _isReader; }
            set { _isReader = value; }
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
        private int _speed;
        public int Speed
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
        #region Skill
        private List<string> skill = new List<string>();

        public List<string> Skill
        {
            get { return skill; }
            set { skill = value; }
        }

        #endregion

        #region NowPosi
        private Point nowPosi;

        public Point NowPosi
        {
            get { return nowPosi; }
            set { nowPosi = value; }
        }
        #endregion
        #region OrderPosi
        private Point orderPosi;

        public Point OrderPosi
        {
            get { return orderPosi; }
            set { orderPosi = value; }
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
    }
}
