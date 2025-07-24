namespace Compiler.CodeAnalysis;

public class Lexer
{
    private readonly string _text;
    private int _position;
    private char _current => _position >= _text.Length ? '\0' : _text[_position];
    private readonly List<string> _diagnostics = [];

    public Lexer(string text)
    {
        _text = text;
    }

    public IEnumerable<string> Diagnostics => _diagnostics;

    private void IncrementPosition() => _position++;

    public SyntaxToken NextToken()
    {
        if (char.IsDigit(_current))
        {
            var start = _position;

            while (char.IsDigit(_current))
                IncrementPosition();

            var length = _position - start;
            var value = _text.Substring(start, length);

            if (!int.TryParse(value, out var intValue))
                _diagnostics.Add(
                    $"[ERROR] The value {value} at position {start} is not a valid Int32."
                );

            return new SyntaxToken(SyntaxKind.NumberToken, start, value, intValue);
        }

        if (char.IsWhiteSpace(_current))
        {
            var start = _position;

            while (char.IsWhiteSpace(_current))
                IncrementPosition();

            return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, null!, null!);
        }

        var token = _current switch
        {
            '+' => new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null!),
            '-' => new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null!),
            '*' => new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null!),
            '/' => new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null!),
            '(' => new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null!),
            ')' => new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null!),
            '\0' => new SyntaxToken(SyntaxKind.EOFToken, _position, null!, null!),
            _ => new SyntaxToken(
                SyntaxKind.BadToken,
                _position++,
                _text.Substring(_position - 1, 1),
                null!
            ),
        };

        if (token.Kind == SyntaxKind.BadToken)
            _diagnostics.Add($"[ERROR] Bad character '{token.Text}' at position {token.Position}.");

        return token;
    }
}
