namespace WPF_Successor_001_to_Vahren._015_Lexer
{
    public class Token
    {
        public Token(TokenType tokenType, string literal)
        {
            this.Type = tokenType;
            this.Literal = literal;
        }
        public TokenType Type { get; set; }
        public string Literal { get; set; }
    }
}
