﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassContext
    {
        #region TitleName
        private string title_name = string.Empty;
		public string TitleName
        {
			get { return title_name; }
			set { title_name = value; }
		}
        #endregion

        #region TitleMenuSpace
        private int title_menu_space = 50;
		public int TitleMenuSpace
        {
			get { return title_menu_space; }
			set { title_menu_space = value; }
		}
        #endregion

        #region GainPer
        private int gain_per = 200;
		public int GainPer
        {
			get { return gain_per; }
			set { gain_per = value; }
		}
		#endregion
	}
}
