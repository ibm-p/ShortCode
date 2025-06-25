using ShortCodeRenderer.Common.Classes;
using ShortCodeRenderer.Common.Interfaces;
using ShortCodeRenderer.Common.Renderer;
using ShortCodeRenderer.Common.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShortCodeRenderer.Common
{
    public class ShortCodeGlobals
    {
        public readonly static Dictionary<string, Func<string, IShortCodeRender>> TypeDef = new Dictionary<string, Func<string, IShortCodeRender>>()
        {
            ["text"] = (input) => new StringShortCodeRender(input),
            ["file"] = (input) => new FileShortCodeRender(input)
        };
        public readonly static Dictionary<string, IShortCodeRender> GlobalRenderers = new Dictionary<string, IShortCodeRender>(StringComparer.OrdinalIgnoreCase);
        public static void AddGlobalRenderer(string name, string value)
        {
            GlobalRenderers[name] = new StringShortCodeRender(value);
        }
        public static void AddGlobalRenderer(string name, Func<ShortCodeInfo, TaskOr<string>> value)
        {
            GlobalRenderers[name] = new FuncShortCodeRender(value);
        }
        public static void AddGlobalRenderer(string name, IShortCodeRender renderer)
        {
            GlobalRenderers[name] = renderer;
        }
        public static void ClearGlobalRenderers()
        {
            GlobalRenderers.Clear();
        }
    }
}
