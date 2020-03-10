using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Filters
{
    /// <summary>
    /// //require https attribute will redirect http request to https, here we are changing the default behavior to prevent redirection
    /// and return bad request - 400 status code
    /// 
    /// This filter will not be hit if the http requests are rejected at reverse proxy. However, this stays as a fallback
    /// </summary>
    public class RequireHttpsOrCloseAttribute: RequireHttpsAttribute
    {
        protected override void HandleNonHttpsRequest(AuthorizationFilterContext filterContext)
        {
            filterContext.Result = new StatusCodeResult(400);
        }
    }
}
 