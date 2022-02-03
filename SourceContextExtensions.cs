namespace SvgHelper.Generator;
internal static class SourceContextExtensions
{
    public static void RaiseNoPartialClassException(this SourceProductionContext context, string className)
    {
        string information = $"Needs to have partial class for class name was {className}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoClass"), Location.None));
    }
    private static DiagnosticDescriptor RaiseException(string information, string id) => new(id,
        "Could not create helpers",
        information,
        "CustomID",
        DiagnosticSeverity.Error,
        true);
}