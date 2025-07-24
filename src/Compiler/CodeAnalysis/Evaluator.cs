namespace Compiler.CodeAnalysis;

public class Evaluator(ExpressionSyntax root)
{
    public int Evaluate() => EvaluateExpression(root);

    private int EvaluateExpression(ExpressionSyntax node)
    {
        if (node is NumberExpressionSyntax numberExpression)
            return (int)numberExpression.NumberToken.Value;

        if (node is BinaryExpressionSyntax binaryExpression)
        {
            var left = EvaluateExpression(binaryExpression.Left);
            var right = EvaluateExpression(binaryExpression.Right);

            return binaryExpression.OperatorToken.Kind switch
            {
                SyntaxKind.PlusToken => left + right,
                SyntaxKind.MinusToken => left - right,
                SyntaxKind.StarToken => left * right,
                SyntaxKind.SlashToken => left / right,
                _ => throw new Exception(
                    $"Unexpected operator <{binaryExpression.OperatorToken.Kind}>."
                ),
            };
        }

        if (node is ParenthesizedExpressionSyntax parenthesizedExpression)
            return EvaluateExpression(parenthesizedExpression.Expression);

        throw new Exception($"Unexpected expression <{node.Kind}>.");
    }
}
