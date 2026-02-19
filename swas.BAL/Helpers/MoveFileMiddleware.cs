using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace swas.BAL.Helpers
{
    public class MoveFileMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public MoveFileMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var moveFileProgram = new MoveFileProgram(_configuration);
                moveFileProgram.MoveFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Middleware Error: " + ex.Message);
            }

            await _next(context); // Continue processing the request
        }
    }
}
