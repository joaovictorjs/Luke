using System.ComponentModel;
using System.Linq.Expressions;

public static class Program
{
    static void Main(string[] args)
    {
        while (true)
        {
            Console.Write("> ");

            var line = Console.ReadLine() ?? string.Empty;
            var parser = new Parser(line);

            ExpressionSyntax expressionSyntax = parser.Parse();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            PrettyPrint(expressionSyntax);

            Console.WriteLine();

            if (parser.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                foreach (var item in parser.Diagnostics)
                {
                    Console.WriteLine(item);
                }
            }

            Console.ResetColor();
            Console.WriteLine();
        }
    }

    static void PrettyPrint(SyntaxNode syntaxNode, string indent = "", bool isLast = true)
    {
        var marker = isLast ? "└──" : "├──";

        Console.Write(indent);
        Console.Write(marker);
        Console.Write(syntaxNode.Kind);

        indent += isLast ? "    " : "│   ";
        var children = syntaxNode.GetChildren();

        if (syntaxNode is SyntaxToken t && t.Value != null)
        {
            Console.Write(" -> " + t.Value);
        }

        Console.WriteLine();

        foreach (var child in children)
        {
            PrettyPrint(child, indent, child == children.LastOrDefault());
        }
    }
}

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
            _ = int.TryParse(value, out var intValue);
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

public abstract class SyntaxNode
{
    public abstract SyntaxKind Kind { get; }

    public abstract IEnumerable<SyntaxNode> GetChildren();
}

public class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object value)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }

    public override SyntaxKind Kind { get; }
    public int Position { get; }
    public string Text { get; }
    public object Value { get; }

    public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();
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
    BinaryExpression,
    NumberExpression,
}

public abstract class ExpressionSyntax : SyntaxNode { }

public class NumberExpressionSyntax(SyntaxToken numberToken) : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.NumberExpression;
    public SyntaxToken NumberToken => numberToken;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return numberToken;
    }
}

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

    private ExpressionSyntax ParsePrimaryExpression()
    {
        var numberToken = Match(SyntaxKind.NumberToken);
        return new NumberExpressionSyntax(numberToken);
    }

    public ExpressionSyntax Parse()
    {
        var left = ParsePrimaryExpression();

        while (_current.Kind == SyntaxKind.PlusToken || _current.Kind == SyntaxKind.MinusToken)
        {
            var operatorToken = ConsumeToken();
            var right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }
}
