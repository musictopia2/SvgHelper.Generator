namespace SvgHelper.Generator;
internal static class WriterExtensions
{
    public static IWriter AppendInterface(this IWriter w)
    {
        w.Write("IStart");
        return w;
    }
    public static ICodeBlock PopulateChildren(this ICodeBlock w, CompleteInformation item)
    {
        if (item.HasChildren)
        {
            w.WriteLine("BasicList<IStart> IStart.GetChildren => Children;");
        }
        else
        {
            w.WriteLine("BasicList<IStart> IStart.GetChildren => new();");
        }
        return w;
    }
    public static ICodeBlock PopulateType(this ICodeBlock w, CompleteInformation item)
    {
        w.WriteLine(w =>
        {
            w.Write("string ")
            .AppendInterface()
            .Write(".TypeUsed => ")
            .AppendDoubleQuote(w =>
            {
                w.Write(item.MainSymbol!.Name);
            }).Write(";");
        });
        return w;
    }
    public static ICodeBlock PopulateCaptureRef(this ICodeBlock w, CompleteInformation item)
    {
        if (item.HasCapturedSymbol)
        {
            w.WriteLine("bool IStart.GetCapturedRef => CaptureRef;");
        }
        else
        {
            w.WriteLine(w =>
            {
                w.Write("bool ")
                .AppendInterface()
                .Write(".GetCapturedRef => ")
                .CustomExceptionLine(w =>
                {
                    w.Write("There was no property for GetCapturedRef.  Try running GetSpecificProperty");
                });
            });
        }
        return w;
    }
    public static ICodeBlock PopulateGetProperty(this ICodeBlock w, CompleteInformation item)
    {
        w.WriteLine("string IStart.GetSpecificProperty(string name)")
            .WriteCodeBlock(w =>
            {
                foreach (var p in item.Properties)
                {
                    w.PopulateGetProperty(p);
                }
                w.WriteLine(w =>
                {
                    w.CustomExceptionLine(w =>
                    {
                        w.Write("Nothing found with property name {name}");
                    });
                });
            });

        return w;
    }
    private static void PopulateGetProperty(this ICodeBlock w, IPropertySymbol p)
    {
        if (p.Name == "EventData")
        {
            return;
        }
        w.WriteLine(w =>
        {
            w.Write("if (name == ")
            .AppendDoubleQuote(p.Name)
            .Write(")");
        })
        .WriteCodeBlock(w =>
        {
            w.WriteLine(w =>
            {
                w.Write("return ")
                .Write(p.Name)
                .Write(".ToString();");
            });
        });
    }
    public static ICodeBlock PopulateHasSpecificProperty(this ICodeBlock w, CompleteInformation item)
    {
        w.WriteLine("bool IStart.HasSpecificProperty(string name)")
            .WriteCodeBlock(w =>
            {
                w.WriteLine(w =>
                {
                    w.Write("if (name == ")
                    .AppendDoubleQuote("CaptureRef")
                    .Write(")");
                })
               .WriteCodeBlock(w =>
               {
                   w.WriteLine(w =>
                   {
                       w.Write("return ")
                       .Write(item.HasCapturedSymbol.ToString().ToLower())
                       .Write(";");
                   });
               });
                foreach (var p in item.Properties)
                {
                    w.PopulateHasSpecificProperty(p);
                }
                w.WriteLine("return false;");
            });
        return w;
    }
    private static void PopulateHasSpecificProperty(this ICodeBlock w, IPropertySymbol p)
    {
        if (p.Name == "EventData")
        {
            return;
        }
        w.WriteLine(w =>
        {
            w.Write("if (name == ")
            .AppendDoubleQuote(p.Name)
            .Write(")");
        })
        .WriteCodeBlock(w =>
        {
            w.WriteLine("return true;");
        });
    }
    public static ICodeBlock PopulateIStartProperties(this ICodeBlock w, CompleteInformation item)
    {
        w.WriteLine("BasicList<CustomProperty> IStart.Properties()")
            .WriteCodeBlock(w =>
            {
                w.WriteLine("BasicList<CustomProperty> output = new();");
                if (item.Properties.Count > 0)
                {
                    w.WriteLine("CustomProperty item;");
                }
                foreach (var p in item.Properties)
                {
                    w.PopulateIStartSingleProperty(p);
                }
                w.WriteLine("return output;");
            });
        return w;
    }
    private static void PopulateIStartSingleProperty(this ICodeBlock w, IPropertySymbol p)
    {
        w.WriteLine("item = new()")
            .WriteCodeBlock(w =>
            {
                bool rets = p.Type.Name == "Double";
                string name = p.Name;
                w.WriteLine(w =>
                {
                    w.Write("IsDouble = ")
                    .Write(rets.ToString().ToLower())
                    .Write(",");
                })
                .WriteLine(w =>
                {
                    w.Write("AttributeName = ")
                    .AppendDoubleQuote(name)
                    .Write(",");
                })
                .WriteLine(w =>
                {
                    w.Write("Value = ")
                    .Write(name);
                });
            }, true)
            .WriteLine("output.Add(item);");
    }
}