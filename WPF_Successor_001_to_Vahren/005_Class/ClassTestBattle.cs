using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._010_Enum;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassTestBattle
    {
        private BattleWhichIsThePlayer player = BattleWhichIsThePlayer.Sortie;
        public BattleWhichIsThePlayer Player
        {
            get { return player; }
            set { player = value; }
        }

        private string map = "";
        public string Map
        {
            get { return map; }
            set { map = value; }
        }

        #region ListMember
        private List<(string, int)> _listMember = new List<(string, int)>();
        public List<(string, int)> ListMember
        {
            get { return _listMember; }
            set { _listMember = value; }
        }
        #endregion

        #region ListMemberBouei
        private List<(string, int)> _listMemberBouei = new List<(string, int)>();
        public List<(string, int)> ListMemberBouei
        {
            get { return _listMemberBouei; }
            set { _listMemberBouei = value; }
        }
        #endregion


    }
}
