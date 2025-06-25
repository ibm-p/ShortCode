using ShortCodeRenderer.Common;
using ShortCodeRenderer.Common.Classes;
using ShortCodeRenderer.Common.Interfaces;
using ShortCodeRenderer.Common.Tasks;
using ShortCodeRenderer.Common.Utils;
using ShortCodeRenderer.Dynamic.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace ShortCodeRenderer.Renderer
{
    public class CSharpCodeRender : ShortCodeRenderBase, IShortCodeCache
    {
        private string _content;
        private bool _isFile = false;
        BaseCommonRender csharpCode = null;
        public static string DefaultPath = "";

        public CSharpCodeRender(string content) : this(content, null)
        {
        }
        public CSharpCodeRender(string content, ShortCodeOptions options = null)
        {
            bool isFile = false;
            //dll, txt and css
            if(!string.IsNullOrEmpty(content))
            {
                isFile = content.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                    content.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || 
                    content.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
            }
            if (isFile)
            {
                if (content.StartsWith("@"))
                    _content = Path.Combine(DefaultPath, content.Substring(1));
                else
                    _content = content;
                _isFile = true;
            }
            else
            {
                _content = content;
            }
            Options = options;
        }

        public override TaskOr<string> Render(ShortCodeContextBase context, ShortCodeInfo info)
        {
            if (csharpCode == null && string.IsNullOrEmpty(_content))
            {
                return string.Empty;
            }
            if (csharpCode == null)
            {
                string content = "";
                if (_isFile)
                {
                    if (System.IO.File.Exists(_content))
                    {
                        bool isDll = _content.EndsWith(".dll", StringComparison.OrdinalIgnoreCase);
                        if(isDll)
                        {
                            CodeUtils.AddReference(_content);
                        }
                        else
                        {
                            using (FileStream fs = new FileStream(_content, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            using (StreamReader reader = new StreamReader(fs))
                            {
                                content = reader.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    content = _content;
                }
                csharpCode = CSharpUtils.CompileAndLoad(content);
                if (csharpCode == null)
                    return string.Empty;

            }
            return csharpCode.Render(context, info);
        }
        public bool IsCached()
        {
            return csharpCode != null;
        }
        public bool CanCached()
        {
            return true;
        }
        public void Flush()
        {
            csharpCode = null;
        }
        public static void RegisterType()
        {
            ShortCodeGlobals.TypeDef["csharp"] = (input) => new CSharpCodeRender(input);
        }
    }
}
