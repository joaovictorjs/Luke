namespace Compiler.CodeAnalysis;

public sealed class ParenthesizedExpressionSyntax(
    SyntaxToken openParenthesisToken,
    ExpressionSyntax expression,
    SyntaxToken closeParenthesisToken
) : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

    public ExpressionSyntax Expression { get; } = expression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return openParenthesisToken;
        yield return Expression;
        yield return closeParenthesisToken;
    }
}
