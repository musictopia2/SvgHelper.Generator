namespace SvgHelper.Generator;
internal class EmitClass
{
    private readonly SourceProductionContext _context;
    private readonly BasicList<CompleteInformation> _list;
    public EmitClass(SourceProductionContext context, BasicList<CompleteInformation> list)
    {
        _context = context;
        _list = list;
    }
    private void ProcessErrors()
    {
        foreach (var item in _list)
        {
            if (item.NeededPartial)
            {
                _context.RaiseNoPartialClassException(item.MainSymbol!.Name);
            }
        }
    }
    public void Emit()
    {
        ProcessErrors();
        foreach (var item in _list)
        {
            if (item.NeededPartial == true)
            {
                continue;
            }
            SourceCodeStringBuilder builder = new();
            builder.WriteLine("#nullable enable")
                .WriteLine(w =>
                {
                    w.Write("namespace ")
                    .Write(item.MainSymbol!.ContainingNamespace)
                    .Write(";");
                })
            .WriteLine(w =>
            {
                w.Write("public partial class ")
                .Write(item.MainSymbol!.Name)
                .Write(" : ")
                .AppendInterface();
            })
            .WriteCodeBlock(w =>
            {
                w.PopulateCaptureRef(item)
                .PopulateChildren(item)
                .PopulateType(item)
                .PopulateIStartProperties(item)
                .PopulateHasSpecificProperty(item)
                .PopulateGetProperty(item);
            });
            _context.AddSource($"{item.MainSymbol!.Name}.g", builder.ToString());
        }
    }
}