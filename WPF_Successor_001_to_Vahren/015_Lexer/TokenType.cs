﻿namespace WPF_Successor_001_to_Vahren._015_Lexer
{
    public enum TokenType
    {
        LPAREN,
        RPAREN,
        LBRACE,
        RBRACE,
        FUNCTION,
        ASSIGN,
        PLUS,
        MINUS,
        ASTERISK,
        SLASH,
        BANG,
        LT,
        GT,
        EQ,
        AndAnd,
        OrOr,
        NOT_EQ,
        ILLEGAL,
        IDENT,
        COMMA,
        INT,
        IF,
        EOF,
        LET,
        SEMICOLON,
        RETURN,
        TRUE,
        FALSE,
        ELSE,
        INITD,
        //関数
        MSG,
        TALK,
        CHOICE,
        DIALOG,
        SELECT,
        EVENT,
        PUSHTURN,
        STOREPLAYERPOWER,
        PUSHCOUNTPOWER,
        PUSHSPOT,
        YET,
        ISALIVE,
        DISPLAYGAMERESULT,
    }
}
