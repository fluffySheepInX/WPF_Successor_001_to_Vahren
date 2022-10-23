using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._010_Enum;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassSkill
    {
        #region NameTag
        private string _nameTag = string.Empty;
        public string NameTag
        {
            get { return _nameTag; }
            set { _nameTag = value; }
        }
        #endregion

        #region FKey
        private (string, int) fkey;
        public (string, int) FKey
        {
            get { return fkey; }
            set { fkey = value; }
        }
        #endregion
        #region SortKey
        private int sortKey;
        public int SortKey
        {
            get { return sortKey; }
            set { sortKey = value; }
        }
        #endregion
        #region Func
        private SkillFunc func;
        public SkillFunc Func
        {
            get { return func; }
            set { func = value; }
        }
        #endregion
        #region Icon
        private List<string> icon = new List<string>();
        public List<string> Icon
        {
            get { return icon; }
            set { icon = value; }
        }
        #endregion
        #region Name
        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion
        #region Help
        private string help = String.Empty;
        public string Help
        {
            get { return help; }
            set { help = value; }
        }
        #endregion
        #region Center
        private string center = string.Empty;
        public string Center
        {
            get { return center; }
            set { center = value; }
        }
        #endregion
        #region Mp
        private double mp = 0;
        public double Mp
        {
            get { return mp; }
            set { mp = value; }
        }
        #endregion
        #region SlowPer
        private double slowPer = 0.75;
        public double SlowPer
        {
            get { return slowPer; }
            set { slowPer = value; }
        }
        #endregion
        #region SlowTime
        private int slowTime = 1;
        public int SlowTime
        {
            get { return slowTime; }
            set { slowTime = value; }
        }
        #endregion
        #region Sound
        private List<string> sound = new List<string>();
        public List<string> Sound
        {
            get { return sound; }
            set { sound = value; }
        }
        #endregion
        #region Image
        private string image = string.Empty;
        public string Image
        {
            get { return image; }
            set { image = value; }
        }
        #endregion
        #region Direct
        private string direct = string.Empty;
        public string Direct
        {
            get { return direct; }
            set { direct = value; }
        }
        #endregion
        #region W
        private int w;
        public int W
        {
            get { return w; }
            set { w = value; }
        }
        #endregion
        #region H
        private int h;
        public int H
        {
            get { return h; }
            set { h = value; }
        }
        #endregion
        #region A
        private int a;
        public int A
        {
            get { return a; }
            set { a = value; }
        }
        #endregion
        #region ForceFire
        private string forceFire = string.Empty;
        public string ForceFire
        {
            get { return forceFire; }
            set { forceFire = value; }
        }
        #endregion
        #region Attr
        private string attr = string.Empty;
        public string Attr
        {
            get { return attr; }
            set { attr = value; }
        }
        #endregion
        #region Str
        private (string,int) str;
        public (string, int) Str
        {
            get { return str; }
            set { str = value; }
        }
        #endregion
        #region Range
        private int range;
        public int Range
        {
            get { return range; }
            set { range = value; }
        }
        #endregion
        #region DamageRangeAdjust
        private int damageRangeAdjust;
        public int DamageRangeAdjust
        {
            get { return damageRangeAdjust; }
            set { damageRangeAdjust = value; }
        }
        #endregion
        #region RangeMin
        private int rangeMin;
        public int RangeMin
        {
            get { return rangeMin; }
            set { rangeMin = value; }
        }
        #endregion
        #region Speed
        private int speed;
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        #endregion
        #region GunDelay
        private (string,int) gunDelay;
        public (string, int) GunDelay
        {
            get { return gunDelay; }
            set { gunDelay = value; }
        }
        #endregion
        #region PairNext
        private string pairNext = string.Empty;
        public string PairNext
        {
            get { return pairNext; }
            set { pairNext = value; }
        }
        #endregion
        #region Next
        private string next = string.Empty;
        public string Next
        {
            get { return next; }
            set { next = value; }
        }
        #endregion
        #region RandomSpace
        private int randomSpace;
        public int RandomSpace
        {
            get { return randomSpace; }
            set { randomSpace = value; }
        }
        #endregion
        #region Offset
        private List<string> offset = new List<string>();
        public List<string> Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        #endregion
        #region Ray
        private List<int> ray = new List<int>();
        public List<int> Ray
        {
            get { return ray; }
            set { ray = value; }
        }
        #endregion
        #region ForceRay
        private string forceRay = string.Empty;
        public string ForceRay
        {
            get { return forceRay; }
            set { forceRay = value; }
        }
        #endregion
    }
}
