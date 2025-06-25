using ShortCodeRenderer.Common.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShortCodeRenderer.Common.Classes
{
    public abstract class BaseCommonRender
    {
        public abstract TaskOr<string> Render(ShortCodeContextBase context, ShortCodeInfo info);
    }
}
