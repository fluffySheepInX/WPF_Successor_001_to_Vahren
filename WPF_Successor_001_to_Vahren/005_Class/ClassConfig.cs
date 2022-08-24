using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassConfigGameTitle
    {
        public List<DirectoryInfo> DirectoryGameTitle { get; set; }

        public ClassConfigGameTitle()
        {
            DirectoryGameTitle = new List<DirectoryInfo>();
        }

        #region WindowSelectionPowerLeftTop
        private Point _windowSelectionPowerLeftTop = new Point() { X = 600, Y = 500 };
        public Point WindowSelectionPowerLeftTop
        {
            get { return _windowSelectionPowerLeftTop; }
            set { _windowSelectionPowerLeftTop = value; }
        }
        #endregion
        #region WindowSelectionPowerUnit
        private Point _windowSelectionPowerUnit = new Point() { X = 600, Y = 400 };
        public Point WindowSelectionPowerUnit
        {
            get { return _windowSelectionPowerUnit; }
            set { _windowSelectionPowerUnit = value; }
        }
        #endregion
        #region WindowSelectionPowerImage
        private Point _windowSelectionPowerImage = new Point() { X = 800, Y = 900+5 };
        public Point WindowSelectionPowerImage
        {
            get { return _windowSelectionPowerImage; }
            set { _windowSelectionPowerImage = value; }
        }
        #endregion
    }
}
