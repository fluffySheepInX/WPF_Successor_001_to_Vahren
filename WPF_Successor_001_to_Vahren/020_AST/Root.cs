using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._020_AST
{
    public class Root : INode
    {
        public Root()
        {
            Statements = new List<IStatement>();
        }

        public List<IStatement> Statements { get; set; }

        public string TokenLiteral()
        {
            var target = this.Statements.FirstOrDefault();
            if (target != null)
            {
                return target.TokenLiteral();
            }
            else
            {
                return String.Empty;
            }
        }

        public string ToCode()
        {
            var builder = new StringBuilder();
            foreach (var ast in this.Statements)
            {
                builder.AppendLine(ast.ToCode());
            }
            return builder.ToString().TrimEnd();
        }
    }
}
