using System.Collections.Generic;
using System.Windows.Controls;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassSpot
    {
        public ClassSpot ShallowCopy()
        {
            return (ClassSpot)MemberwiseClone();
        }
        public ClassSpot DeepCopy()
        {
            ClassSpot cs = ShallowCopy();
            // 配列など参照型のデータを新規作成して元の値をコピーする
            cs.ListMember = new List<(string, int)>(cs.ListMember);
            cs.ListWanderingMonster = new List<(string, int)>(cs.ListWanderingMonster);
            cs.ListMonster = new List<(string, int)>(cs.ListMonster);
            // 部隊（ユニット）データの初期値は存在しないから初期化するだけ
            cs.UnitGroup = new List<ClassHorizontalUnit>();
            return cs;
        }

        #region Index
        private int _index;
        public int Index
        {
            get { return _index; }
            set { _index = value; }
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

        #region ImagePath
        private string _imagePath = string.Empty;
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }
        #endregion

        #region Image
        private Image _image = new Image();
        public Image Image
        {
            get { return _image; }
            set { _image = value; }
        }
        #endregion

        #region X
        private int _x;
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }
        #endregion

        #region Y
        private int _y;
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }
        #endregion

        #region Map
        private string _map = string.Empty;
        public string Map
        {
            get { return _map; }
            set { _map = value; }
        }
        #endregion

        // 経済値
        #region Gain
        private int _gain = 0;
        public int Gain
        {
            get { return _gain; }
            set { _gain = value; }
        }
        #endregion

        // 城壁値
        #region Castle
        private int _castle = 0;
        public int Castle
        {
            get { return _castle; }
            set { _castle = value; }
        }
        #endregion

        // 部隊駐留数
        #region Capacity
        private int _capacity = 0;
        public int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }
        #endregion

        // フレーバーテキスト
        #region Text
        private string text = string.Empty;
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        #endregion

        //選択したスポットがどこの勢力に属するかはClassPowerAndCityで見る

        #region PowerNameTag
        private string _powerNameTag = string.Empty;
        public string PowerNameTag
        {
            get { return _powerNameTag; }
            set { _powerNameTag = value; }
        }
        #endregion

        #region ListMember
        /// <summary>
        /// ListMember内部
        /// </summary>
        private List<(string, int)> _listMember = new List<(string, int)>();
        /// <summary>
        /// 初期メンバー（ユニット）配置用
        /// </summary>
        public List<(string, int)> ListMember
        {
            get { return _listMember; }
            set { _listMember = value; }
        }
        #endregion

        #region ListWanderingMonster
        private List<(string, int)> _listWanderingMonster = new List<(string, int)>();
        public List<(string, int)> ListWanderingMonster
        {
            get { return _listWanderingMonster; }
            set { _listWanderingMonster = value; }
        }
        #endregion

        #region ListMonster
        private List<(string, int)> _listMonster = new List<(string, int)>();
        public List<(string, int)> ListMonster
        {
            get { return _listMonster; }
            set { _listMonster = value; }
        }
        #endregion

        #region UnitGroup
        /// <summary>
        /// UnitGroup内部
        /// </summary>
        private List<ClassHorizontalUnit> _unitGroup = new List<ClassHorizontalUnit>();
        /// <summary>
        /// ユニットの実データ。
        /// unitの所属情報を書き換え等、こちらでやる
        /// </summary>
        public List<ClassHorizontalUnit> UnitGroup
        {
            get { return _unitGroup; }
            set { _unitGroup = value; }
        }
        #endregion
    }
}
