using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    //base class for all resource models to return from API
    public abstract class Resource
    {
        [JsonProperty(Order =-2)]//json.net order means that this property is top in the serialized responses. Just for user to visualize/read this property first
        public string Href { get; set; }//now every resource returned from Api will have absolute uri of resource itself. This will function as unique restful id
    }
}
