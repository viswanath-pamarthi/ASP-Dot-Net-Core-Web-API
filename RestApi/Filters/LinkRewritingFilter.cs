using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using RestApi.Infrastructure;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RestApi.Filters
{
    /// <summary>
    /// Result filter(for nonexception responses) can execute before processing response and after processing response 
    /// </summary>
    public class LinkRewritingFilter : IAsyncResultFilter
    {

        private readonly IUrlHelperFactory _urlHelperFactory;


        /// <summary>
        /// Iurlhelper factory will let us  create iurlhelpers on the fly 
        /// </summary>
        /// <param name="urlHelperFactory"></param>
        public LinkRewritingFilter(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var asObjectResult = context.Result as ObjectResult;

            //in some scenarios this might not be correct or we don't want to serialize any linkin the model
            bool shouldSkip = asObjectResult?.StatusCode >= 400 //then it is some error and won't have links on it
                || asObjectResult?.Value == null//the value of response is null or not an object result
                || asObjectResult?.Value as Resource == null; //or if the response is not of type resource 
                
            if(shouldSkip)
            {
                return next();//execute the next result filter or the result itself(if no filters after)
            }

            var rewriter = new LinkRewriter(_urlHelperFactory.GetUrlHelper(context));

            RewriteAllLinks(asObjectResult.Value, rewriter);

            return next();
        }

        private static void RewriteAllLinks(object model, LinkRewriter rewriter)
        {
            if (model == null) return;//if model null return . else use reflection to read all properties using reflection

            var alProperties = model
                .GetType().GetTypeInfo()
                .GetProperties()
                .Where(p => p.CanRead)
                .ToArray();

            var linkProperties = alProperties
                .Where(p => p.CanWrite && p.PropertyType == typeof(Link));

            foreach(var linkProperty in linkProperties)
            {
                var rewritten = rewriter.Rewrite(linkProperty.GetValue(model) as Link);//getvalue gets value from model 

                if (rewritten == null) continue;

                linkProperty.SetValue(model, rewritten);

                //Special handling of the hidden Self Property

                if (linkProperty.Name == nameof(Resource.Self))
                {
                    alProperties
                        .SingleOrDefault(p => p.Name == nameof(Resource.Href))
                        ?.SetValue(model, rewritten.Href);

                    alProperties
                        .SingleOrDefault(p => p.Name == nameof(Resource.Method))
                        ?.SetValue(model, rewritten.Method);

                    alProperties
                        .SingleOrDefault(p => p.Name == nameof(Resource.Relations))
                        ?.SetValue(model, rewritten.Relations);
                }


            }

            //TODO:in complicated resources that contain arrays and nested objects which themselves contain links, 
            //with some more code we can recursively rewrtite all links

            var arrayProperties = alProperties.Where(p => p.PropertyType.IsArray);
            
            RewriteLinksInArrays(arrayProperties, model, rewriter);

            var objectProperties = alProperties
                .Except(linkProperties).Except(arrayProperties);

            RewriteLinksInNestedObjects(arrayProperties, model, rewriter);
        }


        private static void RewriteLinksInNestedObjects(
           IEnumerable<PropertyInfo> objectProperties,
           object model,
           LinkRewriter rewriter)
        {
            foreach (var objectProperty in objectProperties)
            {
                if (objectProperty.PropertyType == typeof(string))
                {
                    continue;
                }

                var typeInfo = objectProperty.PropertyType.GetTypeInfo();
                if (typeInfo.IsClass)
                {
                    RewriteAllLinks(objectProperty.GetValue(model), rewriter);
                }
            }
        }

        private static void RewriteLinksInArrays(
            IEnumerable<PropertyInfo> arrayProperties,
            object model,
            LinkRewriter rewriter)
        {

            foreach (var arrayProperty in arrayProperties)
            {
                var array = arrayProperty.GetValue(model) as Array ?? new Array[0];

                foreach (var element in array)
                {
                    RewriteAllLinks(element, rewriter);
                }
            }
        }
    }
}
