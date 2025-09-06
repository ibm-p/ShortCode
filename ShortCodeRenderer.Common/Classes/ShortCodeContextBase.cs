using ShortCodeRenderer.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ShortCodeRenderer.Common.Classes
{
    public abstract class ShortCodeContextBase
    {
        public TextWriter Writer { get; internal set; }
        public object Model { get; internal set; }
        public object ModelContext { get; internal set; }
        public T GetModel<T>()
        {
            if (Model is T)
                return (T)Model;
            return default;
        }
        public void SetModel<T>(T model)
        {
            Model = model;
        }

        public T GetModelContext<T>()
        {
            if (ModelContext is T)
                return (T)ModelContext;
            return default;
        }
        public void SetModelContext<T>(T modelContext)
        {
            ModelContext = modelContext;
        }
        public Dictionary<string, object> Variables { get; internal set; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public abstract ShortCodeContextBase Register<T>(T instance);
        public void SetWriter(TextWriter writer) => Writer = writer;
        public abstract T GetVariable<T>(string key);
        public abstract ShortCodeContextBase SetVariable<T>(string key, T value);
        public abstract T Resolve<T>();
        public abstract bool IsRegistered<T>();
        public abstract ShortCodeContextBase Unregister<T>();
        public abstract void Clear();
        public abstract string Render(ShortCodeContextBase ctx, string code, Dictionary<string, IShortCodeRender> tempRenderers);
        public abstract string Render(ShortCodeInfo info);
        public abstract string Render(ShortCodeInfo info, Dictionary<string, IShortCodeRender> tempRenderers);

        public abstract string Render(string input);

        public abstract string Render(string input, Dictionary<string, IShortCodeRender> tempRenderers);

        public abstract Task<string> RenderAsync(ShortCodeInfo info);
        public abstract Task<string> RenderAsync(ShortCodeContextBase ctx, string input);
        public abstract Task<string> RenderAsync(ShortCodeInfo info, Dictionary<string, IShortCodeRender> tempRenderers);
        public abstract Task<string> RenderAsync(string input);

        public abstract Task<string> RenderAsync(string input, Dictionary<string, IShortCodeRender> tempRenderers);

    }
}
