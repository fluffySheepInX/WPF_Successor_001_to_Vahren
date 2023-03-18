using System;
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
        private double gain_per = 200;
		public double GainPer
        {
			get { return gain_per; }
			set { gain_per = value; }
		}
        #endregion

        #region NeutralMax
        private int neutral_max;
		public int NeutralMax
        {
			get { return neutral_max; }
			set { neutral_max = value; }
		}
        #endregion

        #region NeutralMin
        private int neutral_min;
		public int NeutralMin
        {
			get { return neutral_min; }
			set { neutral_min = value; }
		}
        #endregion

        #region NeutralMemberMax
        private int neutral_member_max;
		public int NeutralMemberMax
        {
			get { return neutral_member_max; }
			set { neutral_member_max = value; }
		}
        #endregion

        #region neutralMemberMin
        private int neutral_member_min;
		public int neutralMemberMin
        {
			get { return neutral_member_min; }
			set { neutral_member_min = value; }
		}
        #endregion

        #region enemyTurnSkip
        private bool enemy_turn_skip;
		public bool enemyTurnSkip
        {
			get { return enemy_turn_skip; }
			set { enemy_turn_skip = value; }
		}
		#endregion
	}
}
