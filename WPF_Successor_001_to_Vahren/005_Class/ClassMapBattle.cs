using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassMapBattle
    {
        #region Name
        private string name = string.Empty;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        #endregion

        #region TagName
        private string tagName = string.Empty;

        public string TagName
        {
            get { return tagName; }
            set { tagName = value; }
        }
        #endregion

        #region List<List<MapDetail>> MapData
        private List<List<MapDetail>> mapData = new List<List<MapDetail>>();

        public List<List<MapDetail>> MapData
        {
            get { return mapData; }
            set { mapData = value; }
        }

        #endregion

        #region FrontPosi
        private Point? frontPosi = null;
        public Point? FrontPosi
        {
            get { return frontPosi; }
            set { frontPosi = value; }
        }
        #endregion

        #region DefensePosi
        private Point? defensePosi = null;
        public Point? DefensePosi
        {
            get { return defensePosi; }
            set { defensePosi = value; }
        }
        #endregion

        #region NeutralPosi
        private Point? neutralPosi = null;
        public Point? NeutralPosi
        {
            get { return neutralPosi; }
            set { neutralPosi = value; }
        }
        #endregion
    }

    public class MapDetail
    {
        #region Tip
        private string tip = string.Empty;
        public string Tip
        {
            get { return tip; }
            set { tip = value; }
        }
        #endregion

        #region Building
        private string building = string.Empty;

        public string Building
        {
            get { return building; }
            set { building = value; }
        }
        #endregion
    }
}
