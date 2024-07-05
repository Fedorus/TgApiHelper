using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using LoxSmoke.DocXml;
using MongoDB.Driver;

namespace TelegramFaqBotHost.TelegramFaq;

public static class MethodInfoExtensions
{
    /// <summary>
    /// Return the method signature as a string.
    /// </summary>
    /// <param name="method">The Method</param>
    /// <param name="callable">Return as an callable string(public void a(string b) would return a(b))</param>
    /// <returns>Method signature</returns>
    public static MethodDescription GetSignature(this MethodInfo method, DocXmlReader reader,  bool callable = false)
    {
        var methodDesc = new MethodDescription();
        var firstParam = true;
        var sigBuilder = new StringBuilder();
        var comments = reader.GetMethodComments(method);
        
        methodDesc.Name = method.Name;
        
        sigBuilder.Append(TypeName(method.ReturnType));
        sigBuilder.Append(' ');
        sigBuilder.Append(method.Name);
        
        // Add method generics
        if (method.IsGenericMethod)
        {
            sigBuilder.Append("<");
            foreach (var g in method.GetGenericArguments())
            {
                if (firstParam)
                    firstParam = false;
                else
                    sigBuilder.Append(", ");
                sigBuilder.Append(TypeName(g));
            }

            sigBuilder.Append(">");
        }

        sigBuilder.Append('(');
        if (!string.IsNullOrWhiteSpace(comments.Summary))
        {
            methodDesc.Description = FilterXmlReferences(comments.Summary).Replace("\\_", "_\\__");
        }
        if (!string.IsNullOrWhiteSpace(comments.Returns))
        {
            methodDesc.Returns = FilterXmlReferences(comments.Returns);
        }
        firstParam = true;
        var secondParam = false;
        foreach (var param in method.GetParameters())
        {
            if (firstParam)
            {
                firstParam = false;
                secondParam = true;
                if (method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
                {
                    secondParam = true;
                    
                    continue;
                }
            }
            else if (secondParam == true)
                secondParam = false;
            else
                sigBuilder.Append(", ");

            if (param.ParameterType.IsByRef)
                sigBuilder.Append("ref ");
            else if (param.IsOut)
                sigBuilder.Append("out ");
            if (!callable)
            {
                sigBuilder.Append(TypeName(param.ParameterType));
                sigBuilder.Append(' ');
            }

            sigBuilder.Append(param.Name);
            if (param.HasDefaultValue)
            {
                sigBuilder.Append("=" + (param.DefaultValue ??  "default"));
            }

            methodDesc.Params.Add(param.Name, FilterXmlReferences(comments.Parameters.FirstOrDefault(x => x.Name == param.Name).Text) + (param.HasDefaultValue ? " (optional)" : ""));
        }

        sigBuilder.Append(")");
        methodDesc.MethodFullDesc = sigBuilder.ToString();
        return methodDesc;
    }

    private static string FilterXmlReferences(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        // dumb char escaping
        input = input.Replace("_", "\\_");
        
        //remove unused
        input = input.Replace("\r", "").Replace('\n', ' ')
                .Replace("<c>", "").Replace("</c>", "")
                .Replace("<b>", "").Replace("</b>", "")
                .Replace("<see langword=\"false\" />", "false").Replace("<see langword=\"true\" />", "true")
                .Replace("<i>", "").Replace("</i>", "")
                .Replace("</para>", "").Replace("<para>", "")
                .Replace("<see langword=\"null\" />", "null")
                .Replace("<list type=\"bullet\">", "").Replace("</list>", "")
                .Replace("<item>", "•").Replace("</item>", "\t")
            //.Replace("", "").Replace("", "")
            ;

        Regex regex = new Regex("<[^>]*>");
        MatchCollection matches = regex.Matches(input);

        if (matches.Count > 0)
        {
            foreach (Match match in matches)
            {
                try
                {
                    if (match.Value.StartsWith("<see cref=\"T") || match.Value.StartsWith("<see cref=\"P") ||
                        match.Value.StartsWith("<see cref=\"F"))
                    {
                        var replacement = match.Value.Substring(match.Value.LastIndexOf('.') + 1,
                            match.Value.LastIndexOf('"') - match.Value.LastIndexOf('.') - 1) + " ";
                        input = input.Replace(match.Value, replacement);
                    }
                    else if (match.Value.StartsWith("<see cref=\"M"))
                    {
                        var metName = match.Value.Substring(0, match.Value.LastIndexOf('('));
                        var replacement = metName.Substring(metName.LastIndexOf('.') + 1) + " ";
                        input = input.Replace(match.Value, replacement);
                    }
                    else if (match.Value.StartsWith("<paramref name="))
                    {
                        if (!match.Value.EndsWith("/>"))
                        {
                            var toReplace = input.Substring(input.IndexOf(match.Value),
                                input.IndexOf("</paramref>") - input.IndexOf(match.Value) + 11);
                            input = input.Replace(toReplace,
                                match.Value.Substring(match.Value.IndexOf('"') + 1,
                                    match.Value.LastIndexOf('"') - match.Value.IndexOf('"') - 1));
                            continue;
                        }

                        input = input.Replace(match.Value,
                            match.Value.Substring(match.Value.IndexOf('"') + 1,
                                match.Value.LastIndexOf('"') - match.Value.IndexOf('"') - 1));
                    }
                    else if (match.Value.StartsWith("<param name="))
                    {
                        if (!match.Value.EndsWith("/>"))
                        {
                            input = input.Replace("</param>", "");
                        }

                        input = input.Replace(match.Value, match.Value.Substring(match.Value.IndexOf('"') + 1,
                            match.Value.LastIndexOf('"') - match.Value.IndexOf('"') - 1));
                    }
                    else if (match.Value.StartsWith("<a href=\""))
                    {
                        var link = match.Value.Substring(match.Value.IndexOf('"') + 1,
                            match.Value.LastIndexOf('"') - match.Value.IndexOf('"')-1);
                        if (match.Value.EndsWith("/>"))
                        {
                            var text2 = "link";
                            input = input.Replace(match.Value, $"[{text2}]({link})");
                            continue;

                        }

                        var text = input.Substring(input.IndexOf(match.Value) + match.Value.Length,
                            input.IndexOf("</a>") - (input.IndexOf(match.Value) + match.Value.Length));

                        var aftermatch = input.Substring(input.IndexOf('>') + 1,
                            input.IndexOf("</a>") - input.IndexOf('>') + 3);
                        input = input.Replace(match.Value, $"[{text}]({link})");
                        input = input.Replace(aftermatch, "");
                    }
                    else if (match.Value == "</a>" || match.Value == "</paramref>" || match.Value == "</param>")
                        continue;
                    else if (match.Value == "</see>")
                        input = input.Replace("</see>", "");
                    else
                        Console.WriteLine(match);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        return input;
    }

    public static object GetDefault(this Type t)
    {
        return t.IsValueType ? Activator.CreateInstance(t) : null;
    }
    /// <summary>
    /// Get full type name with full namespace names
    /// </summary>
    /// <param name="type">Type. May be generic or nullable</param>
    /// <returns>Full type name, fully qualified namespaces</returns>
    public static string TypeName(Type type)
    {
        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
            return nullableType.Name + "?";

        if (!(type.IsGenericType && type.Name.Contains('`')))
            switch (type.Name)
            {
                case "String": return "string";
                case "Int32": return "int";
                case "Decimal": return "decimal";
                case "Object": return "object";
                case "Void": return "void";
                case "String?": return "string?";
                case "Int32?": return "int?";
                case "Decimal?": return "decimal?";
                case "Object?": return "object?";
                default:
                {
                    return string.IsNullOrWhiteSpace(type.Name) ? type.FullName : type.Name  ;
                }
            }

        var sb = new StringBuilder(type.Name.Substring(0,
            type.Name.IndexOf('`'))
        );
        sb.Append('<');
        var first = true;
        foreach (var t in type.GetGenericArguments())
        {
            if (!first)
                sb.Append(',');
            sb.Append(TypeName(t));
            first = false;
        }

        sb.Append('>');
        return sb.ToString();
    }
}