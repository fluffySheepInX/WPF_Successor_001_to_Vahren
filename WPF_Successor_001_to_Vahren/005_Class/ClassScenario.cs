using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._010_Enum;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassScenario
    {
        #region ScenarioName
        private string _scenarioName = string.Empty;
        public string ScenarioName
        {
            get { return _scenarioName; }
            set { _scenarioName = value; }
        }
        #endregion

        #region Sortkey
        private int sortkey;
        public int Sortkey
        {
            get { return sortkey; }
            set { sortkey = value; }
        }
        #endregion

        #region ScenarioIntroduce
        private string _scenarioIntroduce = String.Empty;
        public string ScenarioIntroduce
        {
            get { return _scenarioIntroduce; }
            set { _scenarioIntroduce = value; }
        }
        #endregion

        #region ScenarioImageBool
        private bool _scenarioImageBool = false;
        public bool ScenarioImageBool
        {
            get { return _scenarioImageBool; }
            set { _scenarioImageBool = value; }
        }
        #endregion

        #region ScenarioImage
        private string _scenario_image = string.Empty;

        public string ScenarioImage
        {
            get { return _scenario_image; }
            set { _scenario_image = value; }
        }
        #endregion

        #region NameMapImageFile
        private string map_image_name_file = string.Empty;

        public string NameMapImageFile
        {
            get { return map_image_name_file; }
            set { map_image_name_file = value; }
        }

        #endregion

        #region ButtonType
        private ButtonType _buttonType;

        public ButtonType ButtonType
        {
            get { return _buttonType; }
            set { _buttonType = value; }
        }
        #endregion

        #region Mail
        private string _mail = string.Empty;

        public string Mail
        {
            get { return _mail; }
            set { _mail = value; }
        }
        #endregion

        #region Internet
        private string _Internet = string.Empty;

        public string Internet
        {
            get { return _Internet; }
            set { _Internet = value; }
        }
        #endregion

        #region ListSpot
        private List<string> _listSpot = new List<string>();
        public List<string> DisplayListSpot
        {
            get { return _listSpot; }
            set { _listSpot = value; }
        }
        #endregion

        #region ListLinkSpot
        private List<(string, string)> _listLinkSpot = new List<(string, string)>();
        public List<(string, string)> ListLinkSpot
        {
            get { return _listLinkSpot; }
            set { _listLinkSpot = value; }
        }
        #endregion

        #region SpotCapacity
        private int _spotCapacity = 8;
        public int SpotCapacity
        {
            get { return _spotCapacity; }
            set { _spotCapacity = value; }
        }
        #endregion

        #region MemberCapacity
        private int _memberCapacity = 8;
        public int MemberCapacity
        {
            get { return _memberCapacity; }
            set { _memberCapacity = value; }
        }
        #endregion

        #region World
        private string _world = string.Empty;

        public string World
        {
            get { return _world; }
            set { _world = value; }
        }
        #endregion
    }
}
