using ShortCodeRenderer.Common.Classes;
using ShortCodeRenderer.Common.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShortCodeRenderer.Common.Interfaces
{
    public interface IShortCodeCache
    {
        bool IsCached();
        bool CanCached();
        void Flush();
    }
    public interface IShortCodeRender
    {
        ShortCodeOptions Options { get; set; }
        TaskOr<string> Render(ShortCodeContextBase context, ShortCodeInfo info);
    }
    public abstract class ShortCodeRenderBase : IShortCodeRender
    {
        public string Source { get; set; }
        public ShortCodeOptions Options { get; set; } = new ShortCodeOptions();
        public abstract TaskOr<string> Render(ShortCodeContextBase context, ShortCodeInfo info);
        public override string ToString()
        {
            return GetType().Name;
        }
        
    }
}
