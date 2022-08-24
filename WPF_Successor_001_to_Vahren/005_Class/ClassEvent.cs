using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._020_AST;

namespace WPF_Successor_001_to_Vahren._005_Class
{
    public class ClassEvent
    {
        #region Name
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        #endregion

        #region Root
        private Root root = new Root();
        public Root Root
        {
            get { return root; }
            set { root = value; }
        }
        #endregion

    }
}
