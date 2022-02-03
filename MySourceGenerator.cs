namespace SvgHelper.Generator;
[Generator]
public class MySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> declares = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsSyntaxTarget(s),
            (t, _) => GetTarget(t))
            .Where(m => m != null)!;
        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax>)> compilation
            = context.CompilationProvider.Combine(declares.Collect());
        context.RegisterSourceOutput(compilation, (spc, source) =>
        {
            Execute(source.Item1, source.Item2, spc);
        });
    }
    private bool IsSyntaxTarget(SyntaxNode syntax)
    {
        return syntax is ClassDeclarationSyntax;
    }
    private ClassDeclarationSyntax? GetTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode();
        var symbol = context.GetClassSymbol(ourClass);
        if (symbol.ContainingNamespace.ToDisplayString() == "SvgHelper.Blazor.Logic.Classes.SubClasses")
        {
            return ourClass;
        }
        return null;
    }
    private void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> list, SourceProductionContext context)
    {
        try
        {
            var others = list.Distinct();
            ParserClass parses = new(compilation);
            var results = parses.GetResults(others);
            EmitClass emits = new(context, results);
            emits.Emit();
        }
        catch (Exception ex)
        {
            context.AddSource("errors.g", $"//{ex.Message}");
        }
    }
}