using ShortCodeRenderer.Common.Classes;
using ShortCodeRenderer.Common.Interfaces;
using ShortCodeRenderer.Common.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShortCodeRenderer.Common.Renderer
{
    public class StringShortCodeRender : ShortCodeRenderBase, IShortCodeRender
    {
        private TaskOr<string> _value;
        public StringShortCodeRender(TaskOr<string> value)
        {
            _value = value;
        }
        public StringShortCodeRender(TaskOr<string> value, ShortCodeOptions options)
        {
            _value = value;
            Options = options;
        }
        public  override TaskOr<string> Render(ShortCodeContextBase context, ShortCodeInfo info)
        {
            return _value;
        }
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
