namespace Trs80.Level1Basic.VirtualMachine.Scanner;

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
    A,
    And,
    At,
    Cls,
    Cont,
    Data,
    Dim,
    DefDbl,
    DefInt,
    DefSng,
    DefStr,
    End,
    Eqv,
    If,
    For,
    Gosub,
    Goto,
    Imp,
    Input,
    Let,
    List,
    Load,
    Merge,
    Mod,
    N,
    New,
    Next,
    Not,
    On,
    Or,
    Print,
    R,
    Read,
    Rem,
    Restore,
    Return,
    Run,
    Save,
    Step,
    Stop,
    T,
    Then,
    To,
    Xor,

    // EOL
    EndOfLine,

    // Backup
    Backup
}