using ShortCodeRenderer.Common.Classes;
using System;
using System.Collections.Generic;
using System.Text;
namespace ShortCodeRenderer
{
    public class ShortCodeBuilder
    {
        ShortCodeContext _ctx;
        ShortCodeInfo _info = new ShortCodeInfo();
        public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
        public ShortCodeBuilder(ShortCodeContext ctx)
        {
            this._ctx = ctx;
        }
        public ShortCodeBuilder WithName(string name)
        {
            _info.Name = name;
            return this;
        }
        public ShortCodeBuilder WithAttr(string name, object value)
        {
            _info.Attributes[name] = new ShortCodeAttribute()
            {
                Name = name,
                Value = value
            };
            return this;
        }
        public ShortCodeBuilder WithContent(string content)
        {
            _info.Content = content;
            return this;
        }
        public ShortCodeBuilder SetVariable<T>(string key, T value)
        {
            _ctx.SetVariable(key, value);
            return this;
        }
        public string Render()
        {
            return _ctx.Render(_info);
        }
    }
}