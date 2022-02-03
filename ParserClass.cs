namespace SvgHelper.Generator;
internal class ParserClass
{
    private readonly Compilation _compilation;
    public ParserClass(Compilation compilation)
    {
        _compilation = compilation;
    }
    public BasicList<CompleteInformation> GetResults(IEnumerable<ClassDeclarationSyntax> list)
    {
        BasicList<CompleteInformation> output = new();
        foreach (var item in list)
        {
            SemanticModel compilationSemanticModel = item.GetSemanticModel(_compilation);
            INamedTypeSymbol symbol = (INamedTypeSymbol)compilationSemanticModel.GetDeclaredSymbol(item)!;
            CompleteInformation info = new();
            info.MainSymbol = symbol;
            if (item.IsPartial() == false)
            {
                info.NeededPartial = true;
                output.Add(info);
                continue;
            }
            var properties = symbol.GetAllPublicProperties();
            if (properties.First().Name == "CaptureRef")
            {
                info.HasCapturedSymbol = true;
                properties.RemoveFirstItem();
            }
            var child = properties.SingleOrDefault(x => x.Name == "Children");
            if (child is not null)
            {
                properties.RemoveSpecificItem(child);
                info.HasChildren = true;
            }
            IPropertySymbol? content = null;
            foreach (var p in properties)
            {
                if (p.Name == "Content")
                {
                    content = p;
                }
            }
            if (content is not null)
            {
                properties.MoveItem(content, properties.Count - 1);
            }
            info.Properties = properties;
            output.Add(info);
        }
        return output;
    }
}