namespace Compiler.CodeAnalysis;

public class Parser
{
    private readonly List<SyntaxToken> _tokens = [];
    private readonly List<string> _diagnostics = [];
    private int _position;

    public Parser(string text)
    {
        var lexer = new Lexer(text);
        SyntaxToken token;

        do
        {
            token = lexer.NextToken();
            if (token.Kind == SyntaxKind.WhiteSpaceToken || token.Kind == SyntaxKind.BadToken)
                continue;
            _tokens.Add(token);
        } while (token.Kind != SyntaxKind.EOFToken);

        _diagnostics.AddRange(lexer.Diagnostics);
    }

    public IEnumerable<string> Diagnostics => _diagnostics;

    private SyntaxToken Peek(int offset = 0)
    {
        int index = _position + offset;
        if (index >= _tokens.Count)
            index = _tokens.Count - 1;
        return _tokens[index];
    }

    private SyntaxToken _current => Peek();

    private SyntaxToken ConsumeToken()
    {
        var token = _current;
        _position++;
        return token;
    }

    private SyntaxToken Match(SyntaxKind kind)
    {
        if (_current.Kind == kind)
            return ConsumeToken();

        _diagnostics.Add(
            $"[ERROR] Unexpected token <{_current.Kind}> at position {_current.Position}, expected <{kind}>."
        );
        return new SyntaxToken(kind, _current.Position, null!, null!);
    }

    public SyntaxTree Parse()
    {
        var expression = ParseTerm();
        var EOFToken = Match(SyntaxKind.EOFToken);
        return new SyntaxTree(_diagnostics, expression, EOFToken);
    }

    private ExpressionSyntax ParseTerm()
    {
        var left = ParseFactor();

        while (_current.Kind == SyntaxKind.PlusToken || _current.Kind == SyntaxKind.MinusToken)
        {
            var operatorToken = ConsumeToken();
            var right = ParseFactor();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParseFactor()
    {
        var left = ParsePrimaryExpression();

        while (_current.Kind == SyntaxKind.StarToken || _current.Kind == SyntaxKind.SlashToken)
        {
            var operatorToken = ConsumeToken();
            var right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        if (_current.Kind == SyntaxKind.OpenParenthesisToken)
        {
            var open = ConsumeToken();
            var expression = ParseTerm();
            var close = Match(SyntaxKind.CloseParenthesisToken);
            return new ParenthesizedExpressionSyntax(open, expression, close);
        }

        var numberToken = Match(SyntaxKind.NumberToken);
        return new NumberExpressionSyntax(numberToken);
    }
}
