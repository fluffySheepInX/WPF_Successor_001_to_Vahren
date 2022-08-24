using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._020_AST
{
    public interface INode
    {
        string TokenLiteral();
        string ToCode();
    }

    public interface IStatement : INode
    {
    }

    public interface IExpression : INode
    {
    }
}
