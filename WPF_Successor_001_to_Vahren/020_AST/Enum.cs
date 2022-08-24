using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._020_AST
{
    public enum Precedence
    {
        LOWEST = 1,
        EQUALS,      // ==
        ANDAND,      // &&
        OROR,      // ||
        LESSGREATER, // >, <
        SUM,         // +
        PRODUCT,     // *
        PREFIX,      // -x, !x
        CALL,        // myFunction(x)
    }
}
