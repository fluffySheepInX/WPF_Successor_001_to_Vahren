using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Successor_001_to_Vahren._015_Lexer
{
    public class Lexer
    {
        public Lexer(string input)
        {
            this.Input = input;
            this.ReadChar();
        }

        public string Input { get; private set; }
        public char CurrentChar { get; private set; }
        public char NextChar { get; private set; }
        public int Position { get; private set; } = 0;

        #region Keywords
        public static Dictionary<string, TokenType> Keywords
            = new Dictionary<string, TokenType>() {
        { "if", TokenType.IF },
        //{ "else", TokenType.ELSE },
        { "return", TokenType.RETURN },
        { "true", TokenType.TRUE },
        { "false", TokenType.FALSE },
        { "let", TokenType.LET },
        { "msg", TokenType.MSG },
        { "talk", TokenType.TALK },
        { "choice", TokenType.CHOICE },
        { "dialog", TokenType.DIALOG },
        { "select", TokenType.SELECT },
        { "event", TokenType.EVENT },
        { "pushTurn", TokenType.PUSHTURN },
        { "storePlayerPower", TokenType.STOREPLAYERPOWER },
        { "pushCountPower", TokenType.PUSHCOUNTPOWER },
        { "pushSpot", TokenType.PUSHSPOT },
        { "yet", TokenType.YET },
        { "isAlive", TokenType.ISALIVE },
        { "displayGameResult", TokenType.DISPLAYGAMERESULT },
        };
        #endregion

        #region NextToken
        public Token NextToken()
        {
            this.SkipWhiteSpace();
            Token token = new Token(TokenType.ILLEGAL, this.CurrentChar.ToString());
            switch (this.CurrentChar)
            {
                case '=':
                    if (this.NextChar == '=')
                    {
                        token = new Token(TokenType.EQ, "==");
                        this.ReadChar();
                    }
                    else
                    {
                        token = new Token(TokenType.ASSIGN, this.CurrentChar.ToString());
                    }
                    break;
                case '&':
                    if (this.NextChar == '&')
                    {
                        token = new Token(TokenType.AndAnd, "&&");
                        this.ReadChar();
                    }
                    else
                    {
                        token = new Token(TokenType.ILLEGAL, this.CurrentChar.ToString());
                    }
                    break;
                case '|':
                    if (this.NextChar == '|')
                    {
                        token = new Token(TokenType.OrOr, "||");
                        this.ReadChar();
                    }
                    else
                    {
                        token = new Token(TokenType.ILLEGAL, this.CurrentChar.ToString());
                    }
                    break;
                case '+':
                    token = new Token(TokenType.PLUS, this.CurrentChar.ToString());
                    break;
                case '-':
                    token = new Token(TokenType.MINUS, this.CurrentChar.ToString());
                    break;
                case '*':
                    token = new Token(TokenType.ASTERISK, this.CurrentChar.ToString());
                    break;
                case '/':
                    token = new Token(TokenType.SLASH, this.CurrentChar.ToString());
                    break;
                case '!':
                    if (this.NextChar == '=')
                    {
                        token = new Token(TokenType.NOT_EQ, "!=");
                        this.ReadChar();
                    }
                    else
                    {
                        token = new Token(TokenType.BANG, this.CurrentChar.ToString());
                    }
                    break;
                case '>':
                    token = new Token(TokenType.GT, this.CurrentChar.ToString());
                    break;
                case '<':
                    token = new Token(TokenType.LT, this.CurrentChar.ToString());
                    break;
                case ',':
                    token = new Token(TokenType.COMMA, this.CurrentChar.ToString());
                    break;
                case '(':
                    token = new Token(TokenType.LPAREN, this.CurrentChar.ToString());
                    break;
                case ')':
                    token = new Token(TokenType.RPAREN, this.CurrentChar.ToString());
                    break;
                case '{':
                    token = new Token(TokenType.LBRACE, this.CurrentChar.ToString());
                    break;
                case '}':
                    token = new Token(TokenType.RBRACE, this.CurrentChar.ToString());
                    break;
                case (char)0:
                    token = new Token(TokenType.EOF, string.Empty);
                    break;
                default:
                    //if (this.IsLetter(this.CurrentChar))
                    //{
                    //    var identifier = this.ReadIdentifier();
                    //    var type = LookupIdentifier(identifier);
                    //    token = new Token(type, identifier);
                    //}
                    //else if (this.IsDigit(this.CurrentChar))
                    //{
                    //    var number = this.ReadNumber();
                    //    token = new Token(TokenType.INT, number);
                    //}
                    //else
                    //{
                    //    token = new Token(TokenType.ILLEGAL, this.CurrentChar.ToString());
                    //}
                    if (this.IsDigit(this.CurrentChar))
                    {
                        var number = this.ReadNumber();
                        token = new Token(TokenType.INT, number);
                    }
                    else
                    {
                        var identifier = this.ReadIdentifier();
                        var type = LookupIdentifier(identifier);
                        token = new Token(type, identifier);
                    }
                    break;
            }

            this.ReadChar();
            return token;
        }
        #endregion

        #region ReadChar
        private void ReadChar()
        {
            if (this.Position >= this.Input.Length)
            {
                //現在の位置が入力値を超えたら終端
                this.CurrentChar = (char)0;
            }
            else
            {
                //現在の位置の文字をCurrentCharへ
                this.CurrentChar = this.Input[this.Position];
            }

            if (this.Position + 1 >= this.Input.Length)
            {
                //先読みして終わりだったら終端
                this.NextChar = (char)0;
            }
            else
            {
                //先読みして続きがあればそれを手に入れる
                this.NextChar = this.Input[this.Position + 1];
            }

            this.Position += 1;
        }
        #endregion

        #region ReadIdentifier
        private string ReadIdentifier()
        {
            string identifier = this.CurrentChar.ToString();

            if (this.CurrentChar == '"')
            {
                identifier = String.Empty;
                this.ReadChar();
                while (this.CurrentChar != '"')
                {
                    identifier += this.CurrentChar;
                    if (this.NextChar == '"')
                    {
                        string re = identifier;
                        this.ReadChar();
                        return re;
                    }
                    this.ReadChar();
                }

                return identifier;
            }

            while (this.IsLetter(this.NextChar))
            {
                identifier += this.NextChar;
                this.ReadChar();
            }

            return identifier;
        }
        #endregion

        #region IsLetter
        private bool IsLetter(char c)
        {
            return ('a' <= c && c <= 'z')
                || ('A' <= c && c <= 'Z')
                || c == '_';
        }
        #endregion

        #region LookupIdentifier
        public static TokenType LookupIdentifier(string identifier)
        {
            if (Keywords.ContainsKey(identifier))
            {
                return Keywords[identifier];
            }
            return TokenType.IDENT;
        }
        #endregion

        #region ReadNumber
        private string ReadNumber()
        {
            var number = this.CurrentChar.ToString();

            while (this.IsDigit(this.NextChar))
            {
                number += this.NextChar;
                this.ReadChar();
            }

            return number;
        }
        #endregion

        #region IsDigit
        private bool IsDigit(char c)
        {
            return '0' <= c && c <= '9';
        }
        #endregion

        #region SkipWhiteSpace
        private void SkipWhiteSpace()
        {
            while (this.CurrentChar == ' '
                || this.CurrentChar == '\t'
                || this.CurrentChar == '\r'
                || this.CurrentChar == '\n')
            {
                this.ReadChar();
            }
        }
        #endregion
    }
}
