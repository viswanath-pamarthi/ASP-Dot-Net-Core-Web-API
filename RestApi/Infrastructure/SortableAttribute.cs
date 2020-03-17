using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple =false)]
    public class SortableAttribute:Attribute
    {
        public bool Default { get; set; }

        public string EntityProperty { get; set; }
    }
}
