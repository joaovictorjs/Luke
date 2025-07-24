namespace Compiler.CodeAnalysis;

public sealed class SyntaxTree(
    IReadOnlyList<string> diagnostics,
    ExpressionSyntax root,
    SyntaxToken EOFToken
)
{
    public IReadOnlyList<string> Diagnostics { get; } = diagnostics;
    public ExpressionSyntax Root { get; } = root;
    public SyntaxToken EOFToken { get; } = EOFToken;
}
