using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using LoxSmoke.DocXml;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramFaqBotHost.TelegramFaq;

public static class GetAllByReflection
{
    private static List<MethodDescription> cache = null; 
    public static List<MethodDescription> GetAllMethods()
    {
        return typeof(TelegramBotClientExtensions).GetExtensionMethods();
    }

    public static List<string> GetAllClasses()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Contains("Telegram.Bot"))
            .GetTypes();

        return types.Select(x => x.Name).ToList();
    }
    
    public static List<MethodDescription> GetExtensionMethods(this Type t)
    {
        if (cache != null)
        {
            return cache;
        }
        var methodSource = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Contains("Telegram.Bot")).GetTypes().FirstOrDefault(x=>x.Name==nameof(TelegramBotClientExtensions));
        var methods = methodSource.GetMethods().Where(x=>(x.Attributes & MethodAttributes.Static) != 0 );
        var result = new List<MethodDescription>();
        
        
        var reader = new DocXmlReader(Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf('\\')+1)+"Telegram.Bot.xml", true);
        foreach (var method in methods)
        {
            result.Add( method.GetSignature(reader));
        }

        cache = result;
        return result;
    }
}