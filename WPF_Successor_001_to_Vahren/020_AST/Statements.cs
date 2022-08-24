using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._015_Lexer;

namespace WPF_Successor_001_to_Vahren._020_AST
{
    public class LetStatement : IStatement
    {
        public Token? Token { get; set; }
        public Identifier? Name { get; set; }
        public IExpression? Value { get; set; }

        public string TokenLiteral()
        {
            if (this.Token == null)
            {
                return String.Empty;
            }
            else
            {
                return this.Token.Literal;
            }
        }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append(this.Token?.Literal ?? "");
            builder.Append(" ");
            builder.Append(this.Name?.ToCode() ?? "");
            builder.Append(" = ");
            builder.Append(this.Value?.ToCode() ?? "");
            builder.Append(";");
            return builder.ToString();
        }
    }
    public class ReturnStatement : IStatement
    {
        public Token? Token { get; set; }
        public IExpression? ReturnValue { get; set; }

        public string TokenLiteral()
        {
            if (this.Token == null)
            {
                return String.Empty;
            }
            else
            {
                return this.Token.Literal;
            }
        }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append(this.Token?.Literal ?? "");
            builder.Append(" ");
            builder.Append(this.ReturnValue?.ToCode() ?? "");
            builder.Append(";");
            return builder.ToString();
        }
    }
    public class ExpressionStatement : IStatement
    {
        public Token? Token { get; set; }
        public IExpression? Expression { get; set; }

        public string ToCode() => this.Expression?.ToCode() ?? "";

        public string TokenLiteral()
        {
            if (this.Token == null)
            {
                return String.Empty;
            }
            else
            {
                return this.Token.Literal;
            }
        }
    }
    public class BlockStatement : IStatement
    {
        public Token? Token { get; set; }
        public List<IStatement>? Statements { get; set; }

        public string ToCode()
        {
            if (this.Statements == null)
            {
                return String.Empty;
            }

            var builder = new StringBuilder();
            foreach (var statement in this.Statements)
            {
                builder.Append(statement.ToCode());
            }
            return builder.ToString();
        }

        public string TokenLiteral()
        {
            if (this.Token == null)
            {
                return String.Empty;
            }
            else
            {
                return this.Token.Literal;
            }
        }
    }
}
