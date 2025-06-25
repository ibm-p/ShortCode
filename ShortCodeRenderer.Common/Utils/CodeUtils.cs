using ShortCodeRenderer.Common;
using ShortCodeRenderer.Common.Classes;


//using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShortCodeRenderer.Common.Utils
{
    public class CodeUtils
    {


        public static BaseCommonRender AddReference(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return null;
            }   
            byte[] dllBytes = File.ReadAllBytes(filePath);
            var assembly = Assembly.Load(dllBytes);


            var pluginType = assembly.GetTypes().FirstOrDefault(t => typeof(BaseCommonRender).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            if (pluginType == null)
                return null;
            return Activator.CreateInstance(pluginType) as BaseCommonRender;

        }



    }
}
