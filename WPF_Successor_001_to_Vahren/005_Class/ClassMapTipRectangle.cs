using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Point = System.Windows.Point;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassMapTipRectangle
    {
        #region TipName
        private string tipName = string.Empty;
		public string TipName
		{
			get { return tipName; }
			set { tipName = value; }
		}
        #endregion

        #region LogicalXY
        private Thickness logicalXY;
		public Thickness LogicalXY
        {
			get { return logicalXY; }
			set { logicalXY = value; }
		}
		#endregion

		#region TipXY
		private Point tipXY;
		public Point TipXY
        {
			get { return tipXY; }
			set { tipXY = value; }
		}
		#endregion
	}
}
