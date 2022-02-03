namespace SvgHelper.Generator;
internal class CompleteInformation
{
    public bool NeededPartial { get; set; }
    public INamedTypeSymbol? MainSymbol { get; set; }
    public BasicList<IPropertySymbol> Properties { get; set; } = new();
    public bool HasChildren { get; set; } //if there is children, will be here.
    public bool HasCapturedSymbol { get; set; }
}