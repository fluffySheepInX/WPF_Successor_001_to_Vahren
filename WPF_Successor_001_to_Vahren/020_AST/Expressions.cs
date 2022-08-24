using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._015_Lexer;

namespace WPF_Successor_001_to_Vahren._020_AST
{
    public class Identifier : IExpression
    {
        public Token Token { get; set; }
        public string Value { get; set; }

        public Identifier(Token token, string value)
        {
            this.Token = token;
            this.Value = value;
        }

        public string TokenLiteral() => this.Token.Literal;

        public string ToCode()
        {
            return this.Value;
        }
    }

    public class IntegerLiteral : IExpression
    {
        public Token? Token { get; set; }
        public int Value { get; set; }

        public string ToCode()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
        public string TokenLiteral()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
    }

    public class PrefixExpression : IExpression
    {
        public Token? Token { get; set; }
        public string Operator { get; set; } = String.Empty;
        public IExpression? Right { get; set; }

        public string ToCode()
        {
            if (this.Right != null)
            {
                return $"({this.Operator}{this.Right.ToCode()})";
            }
            else
            {
                return String.Empty;
            }
        }
        public string TokenLiteral()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
    }

    public class InfixExpression : IExpression
    {
        public Token? Token { get; set; }
        public IExpression? Left { get; set; }
        public string Operator { get; set; } = String.Empty;
        public IExpression? Right { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("(");

            if (this.Left != null)
            {
                builder.Append(this.Left.ToCode());
            }
            builder.Append(" ");
            builder.Append(this.Operator);
            builder.Append(" ");
            if (this.Right != null)
            {
                builder.Append(this.Right.ToCode());
            }
            builder.Append(")");
            return builder.ToString();
        }

        public string TokenLiteral()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
    }

    public class BooleanLiteral : IExpression
    {
        public Token? Token { get; set; }
        public bool Value { get; set; }

        public string ToCode()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
        public string TokenLiteral()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
    }

    public class IfExpression : IExpression
    {
        public Token? Token { get; set; }
        public IExpression? Condition { get; set; }
        public BlockStatement? Consequence { get; set; }
        public BlockStatement? Alternative { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("if");
            if (this.Condition == null)
            {
                builder.Append(String.Empty);
            }
            else
            {
                builder.Append(this.Condition.ToCode());
            }
            builder.Append(" ");
            if (this.Consequence == null)
            {
                builder.Append(String.Empty);
            }
            else
            {
                builder.Append(this.Consequence.ToCode());
            }

            if (this.Alternative != null)
            {
                builder.Append("else ");
                builder.Append(this.Alternative.ToCode());
            }

            return builder.ToString();
        }

        public string TokenLiteral()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
    }

    public class FunctionLiteral : IExpression
    {
        public Token? Token { get; set; }
        public List<Identifier>? Parameters { get; set; }
        public BlockStatement? Body { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("fn");
            builder.Append("(");

            if (this.Parameters == null)
            {
                // 何もせず
            }
            else
            {
                var parameters = this.Parameters.Select(p => p.ToCode());
                builder.Append(string.Join(", ", parameters));
            }
            builder.Append(")");
            if (this.Body == null)
            {
                // 何もせず
            }
            else
            {
                builder.Append(this.Body.ToCode());
            }
            return builder.ToString();
        }

        public string TokenLiteral()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
    }

    /// <summary>
    /// MSGやダイアログなど
    /// </summary>
    public class SystemFunctionLiteral : IExpression
    {
        public Token Token { get; set; } = new Token(TokenType.INITD, "");
        public List<Identifier> Parameters { get; set; } = new List<Identifier>();

        public string ToCode()
        {
            var builder = new StringBuilder();
            builder.Append("SystemFn");
            builder.Append("(");

            if (this.Parameters == null)
            {
                // 何もせず
            }
            else
            {
                var parameters = this.Parameters.Select(p => p.ToCode());
                builder.Append(string.Join(", ", parameters));
            }
            builder.Append(")");
            return builder.ToString();
        }

        public string TokenLiteral()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
    }

    public class CallExpression : IExpression
    {
        public Token? Token { get; set; }
        public IExpression? Function { get; set; }
        public List<IExpression>? Arguments { get; set; }

        public string ToCode()
        {
            var builder = new StringBuilder();
            if (this.Function != null)
            {
                builder.Append(this.Function.ToCode());
            }
            builder.Append("(");
            if (this.Arguments != null)
            {
                var args = this.Arguments.Select(a => a.ToCode());
                builder.Append(string.Join(", ", args));
            }
            builder.Append(")");
            return builder.ToString();
        }

        public string TokenLiteral()
        {
            if (this.Token != null)
            {
                return this.Token.Literal;
            }
            return String.Empty;
        }
    }
}
