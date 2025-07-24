namespace Compiler.CodeAnalysis;

public enum SyntaxKind
{
    NumberToken,
    WhiteSpaceToken,
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    OpenParenthesisToken,
    CloseParenthesisToken,
    BadToken,
    EOFToken,
    BinaryExpression,
    NumberExpression,
    ParenthesizedExpression,
}
