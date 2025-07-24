namespace Compiler.CodeAnalysis;

public class NumberExpressionSyntax(SyntaxToken numberToken) : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.NumberExpression;
    public SyntaxToken NumberToken => numberToken;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return numberToken;
    }
}
