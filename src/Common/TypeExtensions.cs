using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class TypeExtensions
    {
        public static bool IsImplementationOf(this Type @this, Type baseType)
        {
            return @this.IsClass && !@this.IsAbstract && @this.IsSubclassOf(baseType);
        }
    }
}
