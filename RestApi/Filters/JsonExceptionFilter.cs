
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Filters
{
    //Filters run before after and action asp.net core process a request
    public class JsonExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;


        public JsonExceptionFilter(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            var error = new ApiError();

            if(_env.IsDevelopment())
            {
                error.Message = context.Exception.Message;
                error.Detail = context.Exception.StackTrace;
            }
            else{
                error.Message = "A server error ocurred";
                error.Detail = context.Exception.Message;
            }
            

            context.Result = new ObjectResult(error)//object result will serialze error to json
            {
                StatusCode=500//server error
            };
            
        }
    }
}
