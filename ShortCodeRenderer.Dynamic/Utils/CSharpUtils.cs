using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ShortCodeRenderer.Common.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ShortCodeRenderer.Dynamic.Utils
{
    public class CSharpUtils
    {
        private static List<MetadataReference> _references;
        public static void AddGlobalReference(MetadataReference reference)
        {
            if (_references == null)
                _references = new List<MetadataReference>();
            _references.Add(reference);
        }
        public static List<MetadataReference> GetAllReferences()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Distinct()
                .Select(a => MetadataReference.CreateFromFile(a.Location) as MetadataReference)
                .ToList();
        }
        public static BaseCommonRender CompileAndLoad(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(BaseCommonRender).Assembly.Location), // Interface'in tanımlı olduğu yer
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(HttpClient).Assembly.Location),
        };

            references = GetAllReferences();

            if (_references != null && _references.Count > 0)
                references.AddRange(_references);

            var compilation = CSharpCompilation.Create(
                "DynamicAsm",
                new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using (var ms = new MemoryStream())
            {

                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var errors = string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
                    return null;
                }
                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());


                var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(BaseCommonRender).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                if (pluginType == null)
                    return null;
                return (BaseCommonRender)Activator.CreateInstance(pluginType);
            }


        }
    }
}
