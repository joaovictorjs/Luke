namespace Compiler.CodeAnalysis;

public class BinaryExpressionSyntax(
    ExpressionSyntax left,
    SyntaxToken operatorToken,
    ExpressionSyntax right
) : ExpressionSyntax
{
    public ExpressionSyntax Left { get; } = left;
    public SyntaxToken OperatorToken { get; } = operatorToken;
    public ExpressionSyntax Right { get; } = right;

    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}
