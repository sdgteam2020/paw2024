using Microsoft.AspNetCore.Http;
using swas.BAL;
using swas.BAL.DTO;
using swas.BAL.Helpers;
using System.Net;

namespace swas.Exceptions
{
    class ExHandMW : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                if (ex.InnerException != null)
                {

                    Error.ExceptionHandle(ex.InnerException.Message);
                }
                else
                {

                    Error.ExceptionHandle(ex.Message);
                }

                await context.Response.WriteAsync("Server Under Maintenance. Please contact Administrator.");
            }
        }
    }
}


