namespace Trs80.Level1Basic.Interpreter.Scanner;

public enum TokenType
{
    // Single character tokens
    Colon,
    Comma,
    LeftParen,
    Minus,
    Plus,
    RightParen,
    Semicolon,
    Slash,
    Star,

    // One or two character tokens,
    Equal,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    NotEqual,

    // Literals
    Identifier,
    Number,
    String,

    // Keywords
    At,
    Cls,
    Cont,
    Data,
    End,
    If,
    For,
    Gosub,
    Goto,
    Input,
    Let,
    List,
    Load,
    Merge,
    N,
    New,
    Next,
    On,
    Print,
    Read,
    Rem,
    Restore,
    Return,
    Run,
    Save,
    Step,
    Stop,
    Tab,
    T,
    Then,
    To,

    // EOL
    EndOfLine,

    // Backup
    Backup
}