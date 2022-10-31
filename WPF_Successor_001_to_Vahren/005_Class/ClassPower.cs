using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassPower
    {
        #region Index
        private int _index;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
        #endregion
        #region IsDownfall
        private bool isDownfall = false;
        public bool IsDownfall
        {
            get { return isDownfall; }
            set { isDownfall = value; }
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
        #region Money
        private int _money;
        public int Money
        {
            get { return _money; }
            set { _money = value; }
        }
        #endregion
        #region FlagPath
        private string _flagPath = string.Empty;
        public string FlagPath
        {
            get { return _flagPath; }
            set { _flagPath = value; }
        }
        #endregion
        #region MasterTag
        private string _masterTag = string.Empty;
        public string MasterTag
        {
            get { return _masterTag; }
            set { _masterTag = value; }
        }
        #endregion
        #region ListHome
        /// <summary>
        /// 勢力のホーム領地を示します。COMの思考に影響します。列挙された領地方面の攻略、奪還を優先するようになります。
        /// </summary>
        private List<string> _listHome = new List<string>();
        public List<string> ListHome
        {
            get { return _listHome; }
            set { _listHome = value; }
        }
        #endregion
        #region Head
        private string _head = string.Empty;
        public string Head
        {
            get { return _head; }
            set { _head = value; }
        }
        #endregion
        #region Diff
        private string _diff = string.Empty;
        public string Diff
        {
            get { return _diff; }
            set { _diff = value; }
        }
        #endregion
        #region Text
        private string _text = string.Empty;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        #endregion
        #region ListMember
        /// <summary>
        /// 開始時の領地。
        /// </summary>
        private List<string> _listMember = new List<string>();
        public List<string> ListMember
        {
            get { return _listMember; }
            set { _listMember = value; }
        }
        #endregion
        #region ListNowMember
        /// <summary>
        /// 現在の領地。
        /// </summary>
        private List<string> listNowMember = new List<string>();
        public List<string> ListNowMember
        {
            get { return listNowMember; }
            set { listNowMember = value; }
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
        #region ListCommonConscription
        private List<string> _listCommonConscription = new List<string>();
        public List<string> ListCommonConscription
        {
            get { return _listCommonConscription; }
            set { _listCommonConscription = value; }
        }
        #endregion

    }
}
