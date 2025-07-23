using System.ComponentModel;

public static class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("> ");

            var line = Console.ReadLine() ?? string.Empty;
            var lexer = new Lexer(line);
            SyntaxToken token;

            while ((token = lexer.NextToken()).Kind != SyntaxKind.EOFToken)
            {
                Console.WriteLine(
                    $"Kind: {token.Kind}, Position: {token.Position}, Text: \"{token.Text}\", Value: {token.Value}"
                );
            }
        }
    }
}

public class Lexer
{
    private readonly string _text;
    private int _position;
    private char _current => _position >= _text.Length ? '\0' : _text[_position];

    public Lexer(string text)
    {
        _text = text;
    }

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

            return new SyntaxToken(SyntaxKind.NumberToken, start, value, int.Parse(value));
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

        return token;
    }
}

public class SyntaxToken
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object value)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }

    public SyntaxKind Kind { get; }
    public int Position { get; }
    public string Text { get; }
    public object Value { get; }
}

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
}
