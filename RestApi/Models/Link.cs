using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestApi.Models
{
    /// <summary>
    /// As we won't be able to use URL.Link outside controller, to make it possiblle to use link in services etc, we are creating this link model and make
    /// use of the LinkRewritingFiler(right before sending response is sent to client this is called) to genrate the absolute URL to resource and put it in href
    /// </summary>
    public class Link
    {
        public const string GetMethod = "GET";
        public static Link To(string routeName, object routeValues = null) => new Link
        {
            RouteName = routeName,
            RouteValues = routeValues,
            Method=GetMethod,
            Relations=null
        };

        public static Link ToCollection(string routeName, object routeValues = null) => new Link
        {
            RouteName = routeName,
            RouteValues = routeValues,
            Method = GetMethod,
            Relations = new[] { "collection" }
        };

        //order to put this property at top in serialized data/json
        [JsonProperty(Order =-4)]
        public string Href { get; set; }
        
        [JsonProperty(Order = -3, 
            PropertyName ="rel",//serialize this property as rel instead of relations
            NullValueHandling =NullValueHandling.Ignore)]//if value null don't include in serialized data
        public string[] Relations { get; set; }

        [JsonProperty(Order =-2, 
            DefaultValueHandling =DefaultValueHandling.Ignore,
            NullValueHandling =NullValueHandling.Ignore
            )]
        [DefaultValue(GetMethod)]//Default value for property
        public string Method { get; set; }

        //Stores the route the name before being being rewritten by the LinkRewritingFilter
        [System.Text.Json.Serialization.JsonIgnore]//ignore the property when serializing
        public string RouteName { get; set; }

        //Stores the route parameters the name before being being rewritten by the LinkRewritingFilter
        [System.Text.Json.Serialization.JsonIgnore]
        public object RouteValues { get; set; }
    }
}
