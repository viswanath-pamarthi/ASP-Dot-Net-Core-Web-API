using Microsoft.AspNetCore.Mvc;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Infrastructure
{
    /// <summary>
    /// To generate an absolute url for an instance of the Link class, we need to access an asp.net service called Iurlhelper.
    /// This is the object available under the URL name. it is possible to use this object outside of controller, but it needs to be given proper context 
    /// inorder to work
    /// </summary>
    public class LinkRewriter
    {
        private readonly IUrlHelper _urlHelper;

        public LinkRewriter(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }
        /// <summary>
        /// this method returns link object  and accept link object with route name and route parameters set
        /// and convert or rewrite it to absolute url in href property in return link object
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public Link Rewrite(Link original)
        {
            if (original == null)
                return null;

           // original.Href= _urlHelper.Link(original.RouteName, original.RouteValues);

            return new Link
            {
                Href= _urlHelper.Link(original.RouteName, original.RouteValues),
                Method=original.Method,
                Relations=original.Relations
            };
        }
    }
}
