using System;
using System.Collections.Generic;
using System.Text;

namespace ShortCodeRenderer.Common.Classes
{
    public class ShortCodeAttribute
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsInnerAttribute { get; set; }
    }
    public class ShortCodeAttributes : Dictionary<string, ShortCodeAttribute>
    {
        public new  ShortCodeAttribute this[string name]
        {
            get => Get(name);
            set => base[name] = value;
        }
        public ShortCodeAttributes() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public string GetString(string name) =>
            Get(name)?.Value as string;

        public object GetObject(string name) =>
            Get(name)?.Value;
        public ShortCodeAttribute Get(string name) =>
            TryGetValue(name, out var attribute) ? attribute : null;
        public ShortCodeAttribute GetOrAdd(string name)
        {
            if (!TryGetValue(name, out var attribute))
            {
                attribute = new ShortCodeAttribute { Name = name };
                this[name] = attribute;
            }
            return attribute;
        }
    }
}
