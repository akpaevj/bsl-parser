// See https://aka.ms/new-console-template for more information

using BSL.AST.Parsing;
using BSL.AST.Parsing.Nodes;
using BSL.AST.Parsing.Nodes.Expressions.Literals;

var extPath = "/home/akpaev.e@enterprise.corp/Рабочий стол/lims-ext-test";
var bslFiles = Directory.GetFiles(extPath, "*.bsl", SearchOption.AllDirectories);

foreach (var bslFile in bslFiles)
{
    var text = File.ReadAllText(bslFile);
    
    var parser = new BslParser();
    var ast = parser.ParseCommonModule(text, new CommonModuleSettings()
    {
        Server = true
    });

    var module = ast.FirstOrDefault();

    foreach (var node in module?.Children ?? [])
    {
        if (node is MethodNode method)
        {
            if (TryGetOverrideAttr(method, "После", out var overridenMethod))
                WriteOverridenMethod(bslFile, "После", overridenMethod);
            
            if (TryGetOverrideAttr(method, "Перед", out overridenMethod))
                WriteOverridenMethod(bslFile, "Перед", overridenMethod);
            
            if (TryGetOverrideAttr(method, "Вместо", out overridenMethod))
                WriteOverridenMethod(bslFile, "Вместо", overridenMethod);
        }
    }
}

return;

void WriteOverridenMethod(string modulePath, string overrideType, string overridenMethod)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"""{overridenMethod}""");
    
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write(" переопределен как ");
    
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write($"""{overrideType}""");
    
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($@" в {modulePath}");
}

bool TryGetOverrideAttr(MethodNode method, string attrName, out string overridenMethod)
{
    var afterAttr = method.Attributes.FirstOrDefault(x => x.IdentifierToken.Text.Equals(attrName, StringComparison.OrdinalIgnoreCase));

    var firstParam = afterAttr?.Parameters.Items.FirstOrDefault();
    if (firstParam is { ValueExpression: StringLiteralExpressionNode stringLiteral })
    {
        overridenMethod = stringLiteral.Value;
        return true;
    }

    overridenMethod = null!;
    return false;
}