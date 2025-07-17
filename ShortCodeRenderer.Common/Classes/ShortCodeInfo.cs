using System;
using System.Collections.Generic;
using System.Text;

namespace ShortCodeRenderer.Common.Classes
{
    public class ShortCodeInfo
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsClosed { get; set; }
        public ShortCodeAttributes Attributes { get; set; }
        public ShortCodeInfo()
        {
            Attributes = new ShortCodeAttributes();
        }
        public ShortCodeInfo(string name) : this(name, string.Empty, new ShortCodeAttributes())
        {
        }
        public ShortCodeInfo(string name, string content) : this( name, content, new ShortCodeAttributes())
        {

        }
        public ShortCodeInfo(string name, string content, ShortCodeAttributes attributes) : this(name, content, attributes, false)
        {
        }
        public ShortCodeInfo(string name, string content, ShortCodeAttributes attributes, bool isClosed)
        {
                Name = name;
                Content = content;
                IsClosed = isClosed;
                Attributes = attributes;
        }
        public string GetAttrStr(string name) => Attributes.GetString(name);
        public object GetAttr(string name) => Attributes.GetObject(name);
        public static ShortCodeInfo From(string name)
        {
            return new ShortCodeInfo(name);
        }
        public static ShortCodeInfo From(string name, string content)
        {
            return new ShortCodeInfo(name, content);
        }
        public static ShortCodeInfo From(string name, string content, ShortCodeAttributes attributes)
        {
            return new ShortCodeInfo(name, content, attributes);
        }
        public static ShortCodeInfo From(string name, ShortCodeAttributes attributes)
        {
            return new ShortCodeInfo(name, null, attributes);
        }
    }
}
