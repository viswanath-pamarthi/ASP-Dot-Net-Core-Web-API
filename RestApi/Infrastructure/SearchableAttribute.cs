using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple =false)]
    public class SearchableAttribute:Attribute
    {
        public ISearchExpressionProvider ExpressionProvider { get; set; } 
        = new DefaultSearchExpressionProvider();//In this way each instance of searchable attribute can declare its own expression provider, this should give us the flexibility to handle many different scenarios 
    }
}
