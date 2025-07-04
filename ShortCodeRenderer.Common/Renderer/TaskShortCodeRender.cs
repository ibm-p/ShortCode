﻿using ShortCodeRenderer.Common.Classes;
using ShortCodeRenderer.Common.Interfaces;
using ShortCodeRenderer.Common.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShortCodeRenderer.Common.Renderer
{   
    public class TaskShortCodeRender : ShortCodeRenderBase, IShortCodeRender
    {
        private Func<ShortCodeInfo, Task<string>> _value;

        public TaskShortCodeRender(Func<ShortCodeInfo, Task<string>> value)
        {
            _value = value;
        }
        public TaskShortCodeRender(Func<ShortCodeInfo, Task<string>> value, ShortCodeOptions options)
        {
            _value = value;
            Options = options;
        }
        public override TaskOr<string> Render(ShortCodeContextBase context, ShortCodeInfo info)
        {
            return _value(info);
        }
        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
