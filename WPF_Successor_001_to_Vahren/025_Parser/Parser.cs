using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using WPF_Successor_001_to_Vahren._015_Lexer;
using WPF_Successor_001_to_Vahren._020_AST;

namespace WPF_Successor_001_to_Vahren._025_Parser
{
    using PrefixParseFn = Func<IExpression>;
    using InfixParseFn = Func<IExpression, IExpression>;

    public class Parser
    {
        public Dictionary<TokenType, PrefixParseFn>? PrefixParseFns { get; set; }
        public Dictionary<TokenType, InfixParseFn>? InfixParseFns { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
        public Token CurrentToken { get; set; }
        public Token NextToken { get; set; }
        public Lexer Lexer { get; }

        public Parser(Lexer lexer)
        {
            this.Lexer = lexer;
            this.CurrentToken = this.Lexer.NextToken();
            this.NextToken = this.Lexer.NextToken();
            this.RegisterPrefixParseFns();
            this.RegisterInfixParseFns();
        }

        public Dictionary<TokenType, Precedence> Precedences { get; set; } =
        new Dictionary<TokenType, Precedence>()
        {
                { TokenType.EQ, Precedence.EQUALS },
                { TokenType.NOT_EQ, Precedence.EQUALS },
                { TokenType.AndAnd, Precedence.ANDAND },
                { TokenType.OrOr, Precedence.OROR },
                { TokenType.LT, Precedence.LESSGREATER },
                { TokenType.GT, Precedence.LESSGREATER },
                { TokenType.PLUS, Precedence.SUM },
                { TokenType.MINUS, Precedence.SUM },
                { TokenType.SLASH, Precedence.PRODUCT },
                { TokenType.ASTERISK, Precedence.PRODUCT },
                { TokenType.LPAREN, Precedence.CALL },
        };
        public Precedence CurrentPrecedence
        {
            get
            {
                if (this.Precedences.TryGetValue(this.CurrentToken.Type, out var p))
                {
                    return p;
                }
                return Precedence.LOWEST;
            }
        }
        public Precedence NextPrecedence
        {
            get
            {
                if (this.Precedences.TryGetValue(this.NextToken.Type, out var p))
                {
                    return p;
                }
                return Precedence.LOWEST;
            }
        }

        private void RegisterPrefixParseFns()
        {
            this.PrefixParseFns = new Dictionary<TokenType, PrefixParseFn>();
            this.PrefixParseFns.Add(TokenType.IDENT, this.ParseIdentifier);
            this.PrefixParseFns.Add(TokenType.INT, this.ParseIntegerLiteral);
            this.PrefixParseFns.Add(TokenType.BANG, this.ParsePrefixExpression);
            this.PrefixParseFns.Add(TokenType.MINUS, this.ParsePrefixExpression);
            this.PrefixParseFns.Add(TokenType.TRUE, this.ParseBooleanLiteral);
            this.PrefixParseFns.Add(TokenType.FALSE, this.ParseBooleanLiteral);
            this.PrefixParseFns.Add(TokenType.LPAREN, this.ParseGroupedExpression);
            this.PrefixParseFns.Add(TokenType.IF, this.ParseIfExpression);
            this.PrefixParseFns.Add(TokenType.FUNCTION, this.ParseFunctionLiteral);
            this.PrefixParseFns.Add(TokenType.MSG, this.ParseSystemFunctionLiteral);
            this.PrefixParseFns.Add(TokenType.TALK, this.ParseSystemFunctionLiteral);
            this.PrefixParseFns.Add(TokenType.CHOICE, this.ParseChoiceLiteral);
            this.PrefixParseFns.Add(TokenType.DIALOG, this.ParseDialogLiteral);
            this.PrefixParseFns.Add(TokenType.SELECT, this.ParseDialogSelectLiteral);
            this.PrefixParseFns.Add(TokenType.EVENT, this.ParseEventLiteral);
            this.PrefixParseFns.Add(TokenType.PUSHTURN, this.ParseSystemFunctionLiteral);
            this.PrefixParseFns.Add(TokenType.STOREPLAYERPOWER, this.ParseSystemFunctionLiteral);
            this.PrefixParseFns.Add(TokenType.PUSHCOUNTPOWER, this.ParseSystemFunctionLiteral);
            this.PrefixParseFns.Add(TokenType.PUSHSPOT, this.ParseSystemFunctionLiteral);
            this.PrefixParseFns.Add(TokenType.YET, this.ParseSystemFunctionLiteral);
            this.PrefixParseFns.Add(TokenType.ISALIVE, this.ParseSystemFunctionLiteral);
        }
        private void RegisterInfixParseFns()
        {
            this.InfixParseFns = new Dictionary<TokenType, InfixParseFn>();
            this.InfixParseFns.Add(TokenType.PLUS, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.MINUS, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.SLASH, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.ASTERISK, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.EQ, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.NOT_EQ, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.AndAnd, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.OrOr, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.LT, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.GT, this.ParseInfixExpression);
            this.InfixParseFns.Add(TokenType.LPAREN, this.ParseCallExpression);
        }
        public IExpression ParseGroupedExpression()
        {
            // "(" スキップ
            this.ChangeTokenForNext();

            // 括弧内式解析
            var expression = this.ParseExpression(Precedence.LOWEST);

            //  ")" チェック
            if (!this.ExpectPeek(TokenType.RPAREN))
            {
                throw new Exception();
            };
            if (expression == null)
            {
                throw new Exception();
            }

            return expression;
        }
        public IExpression ParseIfExpression()
        {
            var expression = new IfExpression()
            {
                Token = this.CurrentToken // IF
            };

            // if の後括弧"("チェック
            if (!this.ExpectPeek(TokenType.LPAREN)) throw new Exception();

            // "("チェック
            this.ChangeTokenForNext();

            // if条件式解析
            expression.Condition = this.ParseExpression(Precedence.LOWEST);

            // 閉じ括弧チェック 
            if (!this.ExpectPeek(TokenType.RPAREN)) throw new Exception();
            if (!this.ExpectPeek(TokenType.LBRACE)) throw new Exception();

            // {=現在トークン
            // ブロック文解析
            expression.Consequence = this.ParseBlockStatement();

            // else句があれば解析する
            if (this.NextToken.Type == TokenType.ELSE)
            {
                // else に読み進める
                this.ChangeTokenForNext();
                // else の後 { チェック
                if (!this.ExpectPeek(TokenType.LBRACE))
                {
                    //else ifをするなら
                    //無限ループでガンガン回す
                    //if (!this.ExpectPeek(TokenType.IF))
                    //{
                    //while (true)
                    //{
                    // if条件式解析
                    //expression.Condition = this.ParseExpression(Precedence.LOWEST);
                    //if (!this.ExpectPeek(TokenType.ELSE))
                    //{
                    //}
                    //continue;
                    //}
                    //}
                    throw new Exception();
                }

                // {=現在トークン
                // ブロック文解析
                expression.Alternative = this.ParseBlockStatement();
            }

            return expression;
        }
        public IExpression ParseBooleanLiteral()
        {
            return new BooleanLiteral()
            {
                Token = this.CurrentToken,
                Value = this.CurrentToken.Type == TokenType.TRUE,
            };
        }
        public IExpression ParseInfixExpression(IExpression left)
        {
            var expression = new InfixExpression()
            {
                Token = this.CurrentToken,
                Operator = this.CurrentToken.Literal,
                Left = left,
            };

            var precedence = this.CurrentPrecedence;
            this.ChangeTokenForNext();
            expression.Right = this.ParseExpression(precedence);

            return expression;
        }
        public IExpression ParseIdentifier()
        {
            return new Identifier(this.CurrentToken, this.CurrentToken.Literal);
        }

        public IExpression ParseIntegerLiteral()
        {
            if (int.TryParse(this.CurrentToken.Literal, out int result))
            {
                return new IntegerLiteral()
                {
                    Token = this.CurrentToken,
                    Value = result,
                };
            }

            var message = $"{this.CurrentToken.Literal} を integer に変換できません。";
            this.Errors.Add(message);
            throw new Exception(message);
        }

        public IExpression ParsePrefixExpression()
        {
            var expression = new PrefixExpression()
            {
                Token = this.CurrentToken,
                Operator = this.CurrentToken.Literal
            };

            this.ChangeTokenForNext();

            expression.Right = this.ParseExpression(Precedence.PREFIX);
            return expression;
        }

        public IExpression ParseCallExpression(IExpression fn)
        {
            var expression = new CallExpression()
            {
                Token = this.CurrentToken,
                Function = fn,
                Arguments = this.ParseCallArguments(),
            };

            return expression;
        }

        public List<IExpression> ParseCallArguments()
        {
            var args = new List<IExpression>();

            // ( スキップ
            this.ChangeTokenForNext();

            // 引数なしの場合
            if (this.CurrentToken.Type == TokenType.RPAREN) return args;

            // 引数ありの場合は1つ目の引数を解析
            var a = this.ParseExpression(Precedence.LOWEST);
            if (a != null)
            {
                args.Add(a);
            }
            else
            {
                return args;
            }

            // 2つ目以降の引数解析
            while (this.NextToken.Type == TokenType.COMMA)
            {
                // カンマ直前のトークンとカンマトークンを読み飛ばす
                this.ChangeTokenForNext();
                this.ChangeTokenForNext();
                var b = this.ParseExpression(Precedence.LOWEST);
                if (b != null)
                {
                    args.Add(b);
                }
            }

            // 閉じかっこチェック
            if (!this.ExpectPeek(TokenType.RPAREN)) throw new Exception();

            return args;
        }
        public IExpression ParseFunctionLiteral()
        {
            var fn = new FunctionLiteral()
            {
                Token = this.CurrentToken
            };

            if (!this.ExpectPeek(TokenType.LPAREN)) throw new Exception();

            fn.Parameters = this.ParseParameters();

            if (!this.ExpectPeek(TokenType.LBRACE)) throw new Exception();

            fn.Body = this.ParseBlockStatement();

            return fn;
        }
        public IExpression ParseSystemFunctionLiteral()
        {
            var fn = new SystemFunctionLiteral()
            {
                Token = this.CurrentToken
            };

            if (!this.ExpectPeek(TokenType.LPAREN)) throw new Exception();

            fn.Parameters = this.ParseParameters();

            return fn;
        }
        public IExpression ParseDialogLiteral()
        {
            var fn = new DialogLiteral()
            {
                Token = this.CurrentToken
            };

            if (!this.ExpectPeek(TokenType.LPAREN)) throw new Exception();

            fn.Parameters = this.ParseParameters();

            return fn;
        }
        public IExpression ParseDialogSelectLiteral()
        {
            var fn = new DialogSelectLiteral()
            {
                Token = this.CurrentToken
            };

            if (!this.ExpectPeek(TokenType.LPAREN)) throw new Exception();

            fn.Parameters = this.ParseParameters();

            return fn;
        }
        public IExpression ParseChoiceLiteral()
        {
            var fn = new ChoiceLiteral()
            {
                Token = this.CurrentToken
            };

            if (!this.ExpectPeek(TokenType.LPAREN)) throw new Exception();

            fn.Parameters = this.ParseParameters();

            if (fn.Parameters.Count < 3)
            {
                throw new Exception("Choiceの引数が足りません");
            }

            fn.VaName = fn.Parameters[0].Value;
            fn.Parameters.RemoveAt(0);

            return fn;
        }
        public IExpression ParseEventLiteral()
        {
            var fn = new EventLiteral()
            {
                Token = this.CurrentToken
            };

            if (!this.ExpectPeek(TokenType.LPAREN)) throw new Exception();

            fn.Parameters = this.ParseParameters();

            return fn;
        }


        private void ChangeTokenForNext()
        {
            this.CurrentToken = this.NextToken;
            this.NextToken = this.Lexer.NextToken();
        }

        public Root? ParseRoot()
        {
            return null;
        }

        public Root ParseProgram()
        {
            var root = new Root();
            root.Statements = new List<IStatement>();

            while (this.CurrentToken.Type != TokenType.EOF)
            {
                var statement = this.ParseStatement();
                if (statement != null)
                {
                    root.Statements.Add(statement);
                }
                this.ChangeTokenForNext();
            }
            return root;
        }

        public BlockStatement ParseBlockStatement()
        {
            var block = new BlockStatement()
            {
                Token = this.CurrentToken, // "{"
                Statements = new List<IStatement>(),
            };

            // "{" スキップ
            this.ChangeTokenForNext();

            while (this.CurrentToken.Type != TokenType.RBRACE
                && this.CurrentToken.Type != TokenType.EOF)
            {
                // ブロック解析
                var statement = this.ParseStatement();
                if (statement != null)
                {
                    block.Statements.Add(statement);
                }
                this.ChangeTokenForNext();
            }

            return block;
        }

        public IStatement? ParseStatement()
        {
            switch (this.CurrentToken.Type)
            {
                case TokenType.LET:
                    return this.ParseLetStatement();
                case TokenType.RETURN:
                    return this.ParseReturnStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        public LetStatement? ParseLetStatement()
        {
            var statement = new LetStatement();
            statement.Token = this.CurrentToken;

            if (!this.ExpectPeek(TokenType.IDENT)) return null;
            //左辺
            statement.Name = new Identifier(this.CurrentToken, this.CurrentToken.Literal);
            //=
            if (!this.ExpectPeek(TokenType.ASSIGN)) return null;

            this.ChangeTokenForNext();
            // 式を解析
            statement.Value = this.ParseExpression(Precedence.LOWEST);
            // セミコロンは必須ではない
            if (this.NextToken.Type == TokenType.SEMICOLON) this.ChangeTokenForNext();

            return statement;
        }

        public ReturnStatement ParseReturnStatement()
        {
            var statement = new ReturnStatement();
            statement.Token = this.CurrentToken;
            this.ChangeTokenForNext();

            // 式を解析
            statement.ReturnValue = this.ParseExpression(Precedence.LOWEST);
            // セミコロンは必須ではない
            if (this.NextToken.Type == TokenType.SEMICOLON) this.ChangeTokenForNext();

            return statement;
        }

        public ExpressionStatement ParseExpressionStatement()
        {
            var statement = new ExpressionStatement();
            statement.Token = this.CurrentToken;

            statement.Expression = this.ParseExpression(Precedence.LOWEST);

            if (this.NextToken.Type == TokenType.SEMICOLON) this.ChangeTokenForNext();

            return statement;
        }

        public List<Identifier> ParseParameters()
        {
            var parameters = new List<Identifier>();

            if (this.NextToken.Type == TokenType.RPAREN)
            {
                this.ChangeTokenForNext();
                return parameters;
            }

            // ( スキップ
            this.ChangeTokenForNext();

            parameters.Add(new Identifier(this.CurrentToken, this.CurrentToken.Literal));

            // 2つ目以降のパラメータをカンマが続く限り処理する
            while (this.NextToken.Type == TokenType.COMMA)
            {
                // すでに処理した識別子とその後ろのカンマを飛ばす
                this.ChangeTokenForNext();
                this.ChangeTokenForNext();

                // 識別子を処理
                parameters.Add(new Identifier(this.CurrentToken, this.CurrentToken.Literal));
            }

            if (!this.ExpectPeek(TokenType.RPAREN)) throw new Exception();

            return parameters;
        }

        /// <summary>
        /// 解析を行う
        /// </summary>
        /// <param name="precedence"></param>
        /// <returns></returns>
        public IExpression? ParseExpression(Precedence precedence)
        {
            if (this.PrefixParseFns == null)
            {
                this.AddPrefixParseFnError(this.CurrentToken.Type);
                return null;
            }

            this.PrefixParseFns.TryGetValue(this.CurrentToken.Type, out var prefix);
            if (prefix == null)
            {
                this.AddPrefixParseFnError(this.CurrentToken.Type);
                return null;
            }

            var leftExpression = prefix();

            while (this.NextToken.Type != TokenType.SEMICOLON
                && precedence < this.NextPrecedence)
            {
                if (this.InfixParseFns == null)
                {
                    this.AddPrefixParseFnError(this.CurrentToken.Type);
                    return null;
                }

                this.InfixParseFns.TryGetValue(this.NextToken.Type, out var infix);
                if (infix == null)
                {
                    return leftExpression;
                }

                this.ChangeTokenForNext();
                leftExpression = infix(leftExpression);
            }

            return leftExpression;
        }

        private void AddPrefixParseFnError(TokenType tokenType)
        {
            var message = $"{tokenType.ToString()} に関連付けられた Prefix Parse Function が存在しません。";
            this.Errors.Add(message);
        }

        private bool ExpectPeek(TokenType type)
        {
            if (this.NextToken.Type == type)
            {
                this.ChangeTokenForNext();
                return true;
            }
            this.AddNextTokenError(type, this.NextToken.Type);
            return false;
        }

        private void AddNextTokenError(TokenType expected, TokenType actual)
        {
            this.Errors.Add($"{actual.ToString()} が到来しましたが、正しくは {expected.ToString()} です。");
        }
    }
}
