using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using swas.BAL.DTO;


namespace swas.BAL.Utility
{
    public class HandlerSessionMW
    {
        private readonly RequestDelegate _next;

        public HandlerSessionMW(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ISession session)
        {

            var myLogin = new Login();
            string serializedObject = JsonConvert.SerializeObject(myLogin);
            session.SetString("MyLogin", serializedObject);

            await _next(context);
        }
    }
}