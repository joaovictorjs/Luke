using Compiler.CodeAnalysis;

public static class Program
{
    static void Main(string[] args)
    {
        bool showTree = false;

        while (true)
        {
            Console.Write("> ");
            var line = Console.ReadLine() ?? string.Empty;

            if (line == ".showTree" || line == ".st")
            {
                showTree = !showTree;
                Console.WriteLine();
                Console.WriteLine("Show Tree: " + (showTree ? "On" : "Off"));
                Console.WriteLine();
                continue;
            }

            if (line == ".clear" || line == ".cls")
            {
                Console.Clear();
                continue;
            }

            var parser = new Parser(line);
            SyntaxTree syntaxTree = parser.Parse();

            if (showTree)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrettyPrint(syntaxTree.Root);
            }

            Console.WriteLine();

            if (!syntaxTree.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                var evaluator = new Evaluator(syntaxTree.Root);
                Console.WriteLine(evaluator.Evaluate());
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                foreach (var item in syntaxTree.Diagnostics)
                    Console.WriteLine(item);
                Console.WriteLine();
            }

            Console.ResetColor();
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
